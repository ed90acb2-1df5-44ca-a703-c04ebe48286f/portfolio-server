using System.Threading.Tasks;
using Portfolio.Application.Models;

namespace Portfolio.Application.Repositories;

public interface IUserRepository : IRepository<User>
{
    Task<User?> FindByIdAsync(int id);

    Task<User?> FindByLoginAsync(string login);
}
