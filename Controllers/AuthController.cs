using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using System;
using Microsoft.Graph;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using Microsoft.Graph.Models.ExternalConnectors;
using Microsoft.Graph.Models;


[ApiController]
[Route("api/auth")]
public class AuthController : ControllerBase
{
    private readonly IConfiguration _configuration;
    private readonly IUserRepository _userRepository;

    public AuthController(IConfiguration configuration, IUserRepository userRepository)
    {
        _configuration = configuration;
        _userRepository = userRepository;
    }

    [HttpPost("signin")]
    public IActionResult SignIn([FromBody] SignInRequest request)
    {
        var user = _userRepository.GetUser(request.Username, request.Password);

        if (user != null)
        {
            // User found, generate and return a token
            var token = GenerateJwtToken(user.Username);
            return Ok(new { Token = token });
        }

        // User not found, return unauthorized status
        return Unauthorized();
    }

    [HttpPost("register")]
    public IActionResult Register([FromBody] RegisterRequest request)
    {
        // Create a new user entity
        var user = new User
        {
            Username = request.Username,
            Password = request.Password
        };
        try
        {
            // Add the user to the Users DbSet
            _userRepository.AddUser(user);

            // Save the changes to the database
            _userRepository.SaveChanges();

            // Return a success response
            return Ok();
        }
        catch (Exception ex)
        {
            // Handle any exceptions that occur during the database operation
            return StatusCode(500, "An error occurred while registering the user.");
        }
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
        var key = Encoding.UTF8.GetBytes(_configuration["JwtSettings:SecretKey"]);
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
