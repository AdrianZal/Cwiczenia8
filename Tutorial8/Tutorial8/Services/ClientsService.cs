using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.Data.SqlClient;
using Tutorial8.Models.DTOs;

namespace Tutorial8.Services;

public class ClientsService : IClientsService
{
    private readonly string _connectionString =
        "Data Source=db-mssql;Initial Catalog=2019SBD;Integrated Security=True;Trust Server Certificate=True";

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
            using (SqlCommand clientCheckCommand = new SqlCommand(clientCheckQuery, conn1))
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
            using (SqlCommand command = new SqlCommand(query, conn1))
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
    
    
    //Dodanie nowego klienta do bazy
    public async Task<ClientDTO> AddClient(ClientDTO clientDto)
    {
        if (string.IsNullOrEmpty(clientDto.FirstName))
            throw new ArgumentException("First name is required");
        if (string.IsNullOrEmpty(clientDto.LastName))
            throw new ArgumentException("Last name is required");
        if (string.IsNullOrEmpty(clientDto.Email))
            throw new ArgumentException("Email is required");
        if (!clientDto.Email.Contains('@'))
            throw new ArgumentException("Email is not in correct format");
        if (!string.IsNullOrEmpty(clientDto.Email) && clientDto.Pesel.Length!=11)
            throw new ArgumentException("Pesel is invalid");
        if (!string.IsNullOrEmpty(clientDto.Telephone) && !clientDto.Telephone.StartsWith("+"))
            throw new ArgumentException("Telephone is not in correct format");
        using (SqlConnection conn = new SqlConnection(_connectionString))
        {
            await conn.OpenAsync();
            var checkEmailQuery = "SELECT 1 FROM Client WHERE Email = @Email";
            using (SqlCommand checkEmailCmd = new SqlCommand(checkEmailQuery, conn))
            {
                checkEmailCmd.Parameters.AddWithValue("@Email", clientDto.Email);
                var exists = await checkEmailCmd.ExecuteScalarAsync();
                if (exists != null)
                {
                    throw new InvalidOperationException("Given email is already in use");
                }
            }
            var insertQuery = @"
            INSERT INTO Client (FirstName, LastName, Email, Telephone, Pesel)
            VALUES (@FirstName, @LastName, @Email, @Telephone, @Pesel)";
            using (SqlCommand insertCmd = new SqlCommand(insertQuery, conn))
            {
                insertCmd.Parameters.AddWithValue("@FirstName", clientDto.FirstName);
                insertCmd.Parameters.AddWithValue("@LastName", clientDto.LastName);
                insertCmd.Parameters.AddWithValue("@Email", clientDto.Email);
                insertCmd.Parameters.AddWithValue("@Telephone", 
                    string.IsNullOrEmpty(clientDto.Telephone) ? DBNull.Value : clientDto.Telephone);
                insertCmd.Parameters.AddWithValue("@Pesel", 
                    string.IsNullOrEmpty(clientDto.Pesel) ? DBNull.Value : clientDto.Pesel);
                await insertCmd.ExecuteNonQueryAsync();
                return clientDto;
            }
        }
    }
}
