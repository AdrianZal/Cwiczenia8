using Microsoft.AspNetCore.Mvc;
using Tutorial8.Models.DTOs;

namespace Tutorial8.Services;

public interface IClientsService
{
    Task<List<ClientDTO>> GetClients();
    Task<List<ClientTripDTO>> GetClientsTrips(int id);
    Task<ClientTripDTO> AddClient([FromBody] ClientDTO clientDto);
}