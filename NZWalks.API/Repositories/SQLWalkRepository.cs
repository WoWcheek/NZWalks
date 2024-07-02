using Microsoft.EntityFrameworkCore;

using NZWalks.API.Data;
using NZWalks.API.Models.Domain;

namespace NZWalks.API.Repositories;

public class SQLWalkRepository : IWalkRepository
{
    private readonly NZWalksDbContext dbContext;

    public SQLWalkRepository(NZWalksDbContext dbContext)
    {
        this.dbContext = dbContext;
    }

    public async Task<List<Walk>> GetAllAsync(
        string? filterOn = null,
        string? filterQuery = null,
        string? sortBy = null,
        bool isAscending = true,
        int pageNumber = 1,
        int pageSize = 100)
    {
        var walks = dbContext.Walks
            .Include("Difficulty")
            .Include("Region")
            .AsQueryable();

        if (!string.IsNullOrWhiteSpace(filterOn) && !string.IsNullOrWhiteSpace(filterQuery))
        {
            if (filterOn.Equals("Name", StringComparison.OrdinalIgnoreCase))
            {
                walks = walks.Where(x => x.Name.Contains(filterQuery));
            }
        }

        if (!string.IsNullOrWhiteSpace(sortBy))
        {
            if (sortBy.Equals("Name", StringComparison.OrdinalIgnoreCase))
            {
                walks = 
                    isAscending ? 
                    walks.OrderBy(x => x.Name) :
                    walks.OrderByDescending(x => x.Name);
            }
            else if (sortBy.Equals("Length", StringComparison.OrdinalIgnoreCase))
            {
                walks =
                    isAscending ?
                    walks.OrderBy(x => x.LengthInKm) :
                    walks.OrderByDescending(x => x.LengthInKm);
            }
        }

        int skipResultsNumber = (pageNumber - 1) * pageSize;

        var res = await walks
            .Skip(skipResultsNumber)
            .Take(pageSize)
            .ToListAsync();

        return res;
    }

    public async Task<Walk?> GetByIdAsync(Guid id)
    {
        return await dbContext.Walks
                     .Include("Difficulty").Include("Region")
                     .FirstOrDefaultAsync(x => x.Id == id);
    }

    public async Task<Walk> CreateAsync(Walk walk)
    {
        await dbContext.AddAsync(walk);
        await dbContext.SaveChangesAsync();
        return walk;
    }

    public async Task<Walk?> UpdateAsync(Guid id, Walk walk)
    {
        var existingWalk = await dbContext.Walks.FirstOrDefaultAsync(x => x.Id == id);

        if (existingWalk is null)
        {
            return null;
        }

        existingWalk.Name = walk.Name;
        existingWalk.Description = walk.Description;
        existingWalk.LengthInKm = walk.LengthInKm;
        existingWalk.WalkImageUrl = walk.WalkImageUrl;
        existingWalk.DifficultyId = walk.DifficultyId;
        existingWalk.RegionId = walk.RegionId;

        await dbContext.SaveChangesAsync();

        return existingWalk;
    }

    public async Task<Walk?> DeleteAsync(Guid id)
    {
        var existingWalk = await dbContext.Walks.FirstOrDefaultAsync(x => x.Id == id);

        if (existingWalk is null)
        {
            return null;
        }

        dbContext.Walks.Remove(existingWalk);
        await dbContext.SaveChangesAsync();

        return existingWalk;
    }
}
