using Portfolio.Server.Entities;

namespace Portfolio.Server.Repositories;

public interface IUserRepository
{
    Task<IEnumerable<User>> FindAll();

    Task<User?> GetByIdAsync(int id);

    Task<User?> GetByLoginAsync(string login);
}
