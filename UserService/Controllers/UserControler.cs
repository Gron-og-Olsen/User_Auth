using Microsoft.AspNetCore.Mvc;
using MongoDB.Driver;
using Models;
using Microsoft.AspNetCore.Identity; // Add the Identity namespace for password hashing

namespace UserService.Controllers;

[ApiController]
[Route("[controller]")]
public class UserController : ControllerBase
{
    private readonly IMongoCollection<User> _userCollection;
    private readonly ILogger<UserController> _logger;
    private readonly IPasswordHasher<User> _passwordHasher;

    public UserController(IMongoCollection<User> userCollection, ILogger<UserController> logger, IPasswordHasher<User> passwordHasher)
    {
        _userCollection = userCollection;
        _logger = logger;
        _passwordHasher = passwordHasher;
    }

    // Endpoint for adding a new user via POST
    [HttpPost(Name = "AddUser")]
    public async Task<ActionResult<User>> AddUser([FromBody] User newUser)
    {
        _logger.LogInformation("Method AddUser called at {DT}", DateTime.UtcNow.ToLongTimeString());

        // Assign a new ID if not provided
        if (newUser.Id == Guid.Empty)
        {
            newUser.Id = Guid.NewGuid();
        }

        // Hash the password before storing it
        newUser.Password = _passwordHasher.HashPassword(newUser, newUser.Password);

        // Insert the new user into MongoDB
        await _userCollection.InsertOneAsync(newUser);

        // Log information about the user being added
        _logger.LogInformation("New user with ID {ID} added at {DT}", newUser.Id, DateTime.UtcNow.ToLongTimeString());

        // Return the created user with a 201 Created status code
        return CreatedAtRoute("GetUserById", new { UserId = newUser.Id }, newUser);
    }

    [HttpGet("Username/{username}", Name = "GetUserByUsername")]
    public async Task<ActionResult<User>> GetUserByUsername(string username)
    {
        _logger.LogInformation("Method GetUserByUsername called for username {Username} at {DT}", username, DateTime.UtcNow.ToLongTimeString());

        // Search for the user by username
        var user = await _userCollection.Find(u => u.Username == username).FirstOrDefaultAsync();

        if (user == null)
        {
            return NotFound(new { message = $"User with username {username} not found." });
        }

        return Ok(user);
    }


    // Endpoint to get a user by ID
    [HttpGet("id/{UserId}", Name = "GetUserById")]
    public async Task<ActionResult<User>> GetUserById(Guid UserId)
    {
        _logger.LogInformation("Method GetUserById called at {DT}", DateTime.UtcNow.ToLongTimeString());

        var user = await _userCollection.Find(u => u.Id == UserId).FirstOrDefaultAsync();
        if (user == null)
        {
            return NotFound(new { message = $"User with ID {UserId} not found." });
        }

        return Ok(user);
    }
}
