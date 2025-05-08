using Microsoft.AspNetCore.Mvc;
using Tutorial8.Models.DTOs;

namespace Tutorial8.Services;

public interface IClientsService
{
    Task<List<ClientDTO>> GetClients();
    Task<List<ClientTripDTO>> GetClientTrips(int id);
    Task<ClientDTO> AddClient(ClientDTO clientDto);
    Task<int> RegisterClientTrip(int id, int tripid);
    Task<int> DeleteClientTrip(int id, int tripid);
}