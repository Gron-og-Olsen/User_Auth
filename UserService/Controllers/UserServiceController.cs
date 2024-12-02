using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace UserServiceController.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class OrderController : ControllerBase
    {
        private readonly ILogger<OrderController> _logger;

        // Injicer loggeren via konstruktoren
        public OrderController(ILogger<OrderController> logger)
        {
            _logger = logger;
        }

        [HttpGet("/order")]
        public IActionResult GetOrder()
        {
            _logger.LogInformation("Modtaget anmodning om ordre.");
            return Ok("Ordren er modtaget!");
        }
    }
}
