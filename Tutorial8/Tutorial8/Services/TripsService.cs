using Microsoft.Data.SqlClient;
using Tutorial8.Models.DTOs;

namespace Tutorial8.Services;

public class TripsService : ITripsService
{
    private readonly string _connectionString = "Data Source=db-mssql;Initial Catalog=2019SBD;Integrated Security=True;Trust Server Certificate=True";
    
    // Zwraca dane wszystkich wycieczek wraz z krajami
    public async Task<List<TripDTO>> GetTrips()
    {
        var trips = new List<TripDTO>();
        string command1 = "SELECT * FROM Trip";
        using (SqlConnection conn1 = new SqlConnection(_connectionString))
        using (SqlCommand cmd1 = new SqlCommand(command1, conn1))
        {
            await conn1.OpenAsync();
            using (var reader1 = await cmd1.ExecuteReaderAsync())
            {
                while (await reader1.ReadAsync())
                {   
                    int idOrdinal = reader1.GetOrdinal("IdTrip");
                    
                    trips.Add(new TripDTO()
                    {
                        Id = reader1.GetInt32(idOrdinal),
                        Name = reader1.GetString(1),
                        Description = reader1.IsDBNull(2) ? null : reader1.GetString(2),
                        DateFrom = reader1.GetDateTime(3),
                        DateTo = reader1.GetDateTime(4),
                        MaxPeople = reader1.GetInt32(5),
                        Countries = new List<CountryDTO>()
                    });
                }
            }
        }
        using (SqlConnection conn2 = new SqlConnection(_connectionString))
        {
            await conn2.OpenAsync();
            foreach (var trip in trips)
            {
                string command2 = @"
                        SELECT c.IdCountry, c.Name
                        FROM Country c
                        JOIN Country_Trip ct ON c.IdCountry = ct.IdCountry
                        WHERE ct.IdTrip = " + trip.Id;
                using (SqlCommand cmd2 = new SqlCommand(command2, conn2))
                {
                    using (var reader2 = await cmd2.ExecuteReaderAsync())
                    {
                        while (await reader2.ReadAsync())
                        {
                            trip.Countries.Add(new CountryDTO()
                            {
                                Name = reader2.GetString(1)
                            });
                        }
                    }
                }
            }
        }
        return trips;
    }
}