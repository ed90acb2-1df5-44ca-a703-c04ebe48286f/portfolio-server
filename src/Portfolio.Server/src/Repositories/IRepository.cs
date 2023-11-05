using System.Collections.Generic;
using System.Threading.Tasks;

namespace Portfolio.Server.Repositories;

public interface IRepository<TModel>
{
    Task<IEnumerable<TModel>> FindAll();
}
