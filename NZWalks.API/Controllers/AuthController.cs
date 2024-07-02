using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Identity;

using NZWalks.API.Models.DTO;
using NZWalks.API.Repositories;

namespace NZWalks.API.Controllers;

[Route("api/[controller]")]
[ApiController]
public class AuthController : ControllerBase
{
    private readonly ITokenRepository tokenRepository;
    private readonly UserManager<IdentityUser> userManager;

    public AuthController(UserManager<IdentityUser> userManager, ITokenRepository tokenRepository)
    {
        this.userManager = userManager;
        this.tokenRepository = tokenRepository;
    }

    [HttpPost]
    [Route("Register")]
    public async Task<IActionResult> Register([FromBody] RegisterRequestDto registerRequestDto)
    {
        var identityUser = new IdentityUser
        {
            UserName = registerRequestDto.Username,
            Email = registerRequestDto.Username
        };

        var identityResult = await userManager.CreateAsync(identityUser, registerRequestDto.Password);

        if (identityResult.Succeeded)
        {
            if (registerRequestDto.Roles?.Any() ?? false)
            {
                identityResult = await userManager.AddToRolesAsync(identityUser, registerRequestDto.Roles);

                if (identityResult.Succeeded)
                {
                    return Ok("User was successfully registered. Please log in.");
                }
            }
        }

        return BadRequest("Something went wrong");
    }

    [HttpPost]
    [Route("Login")]
    public async Task<IActionResult> Login([FromBody] LoginRequestDto loginRequestDto)
    {
        var user = await userManager.FindByEmailAsync(loginRequestDto.Username);

        if (user is null)
        {
            return Unauthorized("No user exists with given username");
        }

        var isPasswordCorrect = await userManager.CheckPasswordAsync(user, loginRequestDto.Password);

        if (!isPasswordCorrect)
        {
            return Unauthorized("Password is incorrect");
        }

        var roles = await userManager.GetRolesAsync(user);

        if (roles is null)
        {
            return Unauthorized("User has no roles");
        }

        var token = tokenRepository.CreateToken(user, roles.ToList());

        var response = new LoginResponseDto
        {
            Token = token
        };

        return Ok(response);
    }
}
