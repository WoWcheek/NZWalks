﻿using System.ComponentModel.DataAnnotations;

namespace NZWalks.API.Models.DTO;

public class UpdateWalkRequestDto
{
    [Required]
    [MaxLength(100, ErrorMessage = "Walk name should be at most 100 characters long")]
    public string Name { get; set; }

    [Required]
    [MaxLength(1000, ErrorMessage = "Walk description should be at most 100 characters long")]
    public string Description { get; set; }

    [Required]
    [Range(0, 50)]
    public double LengthInKm { get; set; }

    public string? WalkImageUrl { get; set; }

    [Required]
    public Guid DifficultyId { get; set; }

    [Required]
    public Guid RegionId { get; set; }
}
