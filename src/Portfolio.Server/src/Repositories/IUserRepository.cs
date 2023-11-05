using System.Threading.Tasks;
using Portfolio.Server.Models;

namespace Portfolio.Server.Repositories;

public interface IUserRepository : IRepository<User>
{
    Task<User?> FindByIdAsync(int id);

    Task<User?> FindByLoginAsync(string login);

    Task SaveAsync(User user);
}
