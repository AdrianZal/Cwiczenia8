using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using Tutorial8.Models.DTOs;

namespace Tutorial8.Services;

public class ClientsService : IClientsService
{
    private readonly string _connectionString = "Data Source=db-mssql;Initial Catalog=2019SBD;Integrated Security=True;Trust Server Certificate=True";
    
    public Task<List<ClientDTO>> GetClients()
    {
        throw new NotImplementedException();
    }
    
    
    //Wyświetl wycieczki danego klienta
    public async Task<List<ClientTripDTO>> GetClientsTrips(int id)
    {
        using (SqlConnection conn1 = new SqlConnection(_connectionString))
            {
                await conn1.OpenAsync();
                var clientCheckQuery = $"SELECT 1 FROM Client WHERE IdClient = {id}";
                using (var clientCheckCommand = new SqlCommand(clientCheckQuery, conn1))
                {
                    var clientExists = await clientCheckCommand.ExecuteScalarAsync();
                    if (clientExists == null)
                    {
                        return null;
                    }
                }
                var query = $@"
                    SELECT 
                        t.IdTrip, t.Name, t.Description, t.DateFrom, t.DateTo, t.MaxPeople,
                        ct.RegisteredAt, ct.PaymentDate
                    FROM Trip t
                    JOIN Client_Trip ct ON t.IdTrip = ct.IdTrip
                    WHERE ct.IdClient = {id}
                    ORDER BY t.DateFrom";
                var trips = new List<ClientTripDTO>();
                using (var command = new SqlCommand(query, conn1))
                {
                    using (var reader = await command.ExecuteReaderAsync())
                    {
                        while (await reader.ReadAsync())
                        {
                            var trip = new ClientTripDTO
                            {
                                IdTrip = reader.GetInt32(0),
                                Name = reader.GetString(1),
                                Description = reader.GetString(2),
                                DateFrom = reader.GetDateTime(3),
                                DateTo = reader.GetDateTime(4),
                                MaxPeople = reader.GetInt32(5),
                                RegisteredAt = reader.GetInt32(6),
                                PaymentDate = reader.IsDBNull(7) ? null : reader.GetInt32(7),
                            };
                            trips.Add(trip);
                        }
                    }
                }
                if (trips.Count == 0)
                {
                    return null;
                }
                return trips;
            }
    }

    public Task<ClientTripDTO> AddClient([FromBody] ClientDTO clientDto)
    {
        return null;
    }
}
