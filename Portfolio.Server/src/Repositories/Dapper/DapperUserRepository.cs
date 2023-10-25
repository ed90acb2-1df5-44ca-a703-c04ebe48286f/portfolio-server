using System.Data;
using Dapper;
using Portfolio.Server.Entities;

namespace Portfolio.Server.Repositories.Dapper;

public class DapperUserRepository : IUserRepository
{
    private readonly IDbConnection _connection;

    public DapperUserRepository(IDbConnection connection)
    {
        _connection = connection;
    }

    public async Task<IEnumerable<User>> FindAll()
    {
        const string query = "SELECT * FROM \"Users\"";

        return await _connection.QueryAsync<User>(query);
    }

    public async Task<User?> GetByIdAsync(int id)
    {
        const string query = "SELECT * FROM \"Users\" WHERE \"Id\" = @Id";

        return await _connection.QueryFirstOrDefaultAsync<User>(query, new {Id = id});
    }

    public async Task<User?> GetByLoginAsync(string login)
    {
        const string query = "SELECT * FROM \"Users\" WHERE \"Login\" = @Login";

        return await _connection.QueryFirstOrDefaultAsync<User>(query, new {Login = login});
    }
}
