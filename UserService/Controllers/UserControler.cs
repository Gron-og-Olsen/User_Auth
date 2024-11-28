using Microsoft.AspNetCore.Mvc;
using Models;

namespace UserService.Controllers;

[ApiController]
[Route("[controller]")]
public class UserController : ControllerBase
{

    private static List<User> _user = new List<User>()
    {
        new User()
        {
            Id = new Guid("c9fcbc4b-d2d1-4664-9079-dae78a1de446"),
            Name = "Æ Fiskehandler",
            Address1 = "Søndergade 3",
            City = "Harboøre",
            PostalCode = 7673,
            ContactName = "Jens Peter Olesen",
            TaxNumber = "133466789"
        },
        new User()
        {
            Id = Guid.NewGuid(),
            Name = "Jyske Bank",
            Address1 = "Strandvejen 100",
            City = "Århus",
            PostalCode = 8000,
            ContactName = "Søren Sørensen",
            TaxNumber = "155567890"
        },
        new User()
        {
            Id = Guid.NewGuid(),
            Name = "Nordea Bank",
            Address1 = "Enghave Plads 20",
            City = "København",
            PostalCode = 1620,
            ContactName = "Mette Hansen",
            TaxNumber = "123456789"
        }
    };

    private readonly ILogger<UserController> _logger;

    public UserController(ILogger<UserController> logger)
    {
        _logger = logger;
    }

    // Endpoint for at hente en specifik kunde via ID
    [HttpGet("{UserId}", Name = "GetUserById")]
    public ActionResult<User> GetUserById(Guid UserId)
    {
        _logger.LogInformation("Metode GetUserById called at {DT}", DateTime.UtcNow.ToLongTimeString());

        var user = _user.FirstOrDefault(c => c.Id == UserId);
        if (user == null)
        {
            return NotFound();
        }
        return Ok(user);
    }

    // Endpoint for at hente alle kunder
    [HttpGet(Name = "GetAllUsers")]
    public ActionResult<List<User>> GetAllCustomers()
    {
        _logger.LogInformation("Metode GetAllUsers called at {DT}", DateTime.UtcNow.ToLongTimeString());

        return Ok(_user);
    }

    // Endpoint for at tilføje en ny kunde via POST
    [HttpPost(Name = "AddUser")]
    public ActionResult<User> AddCustomer([FromBody] User newCustomer)
    {
        _logger.LogInformation("Metode AddCustomer called at {DT}", DateTime.UtcNow.ToLongTimeString());

        // Tjek om ID allerede findes i listen
        if (_user.Any(c => c.Id == newCustomer.Id))
        {
            // Hvis ID allerede findes, log en advarsel og returner HTTP 409 Conflict
            _logger.LogWarning("User with ID {ID} already exists. Conflict at {DT}", newCustomer.Id, DateTime.UtcNow.ToLongTimeString());
            return Conflict(new { message = $"User with ID {newCustomer.Id} already exists." });
        }

        // Tildel et nyt ID til kunden, hvis der ikke allerede findes et ID
        newCustomer.Id = Guid.NewGuid();

        // Tilføj den nye kunde til listen
        _user.Add(newCustomer);

        // Log information om at kunden blev tilføjet
        _logger.LogInformation("New user with ID {ID} added at {DT}", newCustomer.Id, DateTime.UtcNow.ToLongTimeString());

        // Returnér den oprettede kunde sammen med en 201 Created statuskode
        return CreatedAtRoute("GetUserById", new { UserId = newCustomer.Id }, newCustomer);
    }
}