using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Tutorial8.Models.DTOs;
using Tutorial8.Services;

namespace Tutorial8.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ClientsController : ControllerBase
    {
        private readonly IClientsService _clientsService;

        public ClientsController(IClientsService clientsService)
        {
            _clientsService = clientsService;
        }

        [HttpGet]
        public async Task<IActionResult> GetClients()
        {
            var clients = await _clientsService.GetClients();
            return Ok(clients);
        }

        [HttpGet("{id}/trips")]
        public async Task<IActionResult> GetClientsTrips(int id)
        {
            var clientsTrips = await _clientsService.GetClientsTrips(id);
            if (clientsTrips != null) return Ok(clientsTrips);
            return NotFound("Entry does not exist");
        }
        
        [HttpPost]
        public async Task<IActionResult> AddClient([FromBody] ClientDTO clientDto)
        {
            var client = await _clientsService.AddClient(clientDto);
            if (client == null) return BadRequest("Client not added");
            return Ok("Client added");
        }
    }
}