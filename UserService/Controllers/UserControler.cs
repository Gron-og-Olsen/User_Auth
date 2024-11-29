using Microsoft.AspNetCore.Mvc;
using MongoDB.Driver;
using Models;

namespace UserService.Controllers;

[ApiController]
[Route("[controller]")]
public class UserController : ControllerBase
{
    private readonly IMongoCollection<User> _userCollection;
    private readonly ILogger<UserController> _logger;

    public UserController(IMongoCollection<User> userCollection, ILogger<UserController> logger)
    {
        _userCollection = userCollection;
        _logger = logger;
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

        // Insert the new user into MongoDB
        await _userCollection.InsertOneAsync(newUser);

        // Log information about the user being added
        _logger.LogInformation("New user with ID {ID} added at {DT}", newUser.Id, DateTime.UtcNow.ToLongTimeString());

        // Return the created user with a 201 Created status code
        return CreatedAtRoute("GetUserById", new { UserId = newUser.Id }, newUser);
    }

    // Endpoint to get all users
    [HttpGet(Name = "GetAllUsers")]
    public async Task<ActionResult<List<User>>> GetAllUsers()
    {
        _logger.LogInformation("Method GetAllUsers called at {DT}", DateTime.UtcNow.ToLongTimeString());

        var users = await _userCollection.Find(_ => true).ToListAsync();

        return Ok(users);
    }

    // Endpoint to get a user by ID
    [HttpGet("{UserId}", Name = "GetUserById")]
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
