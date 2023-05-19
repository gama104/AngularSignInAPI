using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using System;
using Microsoft.Graph;

[ApiController]
[Route("api/auth")]
public class AuthController : ControllerBase
{
    private readonly IConfiguration _configuration;

    public AuthController(IConfiguration configuration)
    {
        _configuration = configuration;
    }

    [HttpPost("signin")]
    public IActionResult SignIn([FromBody] SignInRequest request)
    {
        // Validate the user's credentials against your data store
        if (IsValidCredentials(request.Username, request.Password))
        {
            // Generate JWT token
            var token = GenerateJwtToken(request.Username);

            // Return the token as the response
            return Ok(new { Token = token });
        }

        // Return appropriate response for invalid credentials
        return Unauthorized();
    }

    private bool IsValidCredentials(string username, string password)
    {
        bool isValid = false;
        // Implement your logic to validate the user's credentials
        // against your data store (e.g., database)
        // Return true if the credentials are valid, otherwise false
        return isValid;
    }

    private string GenerateJwtToken(string username)
    {
        // Create claims for the token (e.g., username, roles, etc.)
        var claims = new List<Claim>
        {
            new Claim(ClaimTypes.Name, username),
            // Add additional claims as needed
        };

        // Generate token using the claims and your secret key
        var tokenHandler = new JwtSecurityTokenHandler();
        var key = Encoding.UTF8.GetBytes(_configuration["Jwt:SecretKey"]);
        var tokenDescriptor = new SecurityTokenDescriptor
        {
            Subject = new ClaimsIdentity(claims),
            Expires = DateTime.UtcNow.AddDays(7), // Token expiration time
            SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
        };
        var token = tokenHandler.CreateToken(tokenDescriptor);

        // Return the generated token as a string
        return tokenHandler.WriteToken(token);
    }
}
