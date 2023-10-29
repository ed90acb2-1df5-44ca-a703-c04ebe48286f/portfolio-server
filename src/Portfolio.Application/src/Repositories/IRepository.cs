using System.Collections.Generic;
using System.Threading.Tasks;

namespace Portfolio.Application.Repositories;

public interface IRepository<TModel>
{
    Task<IEnumerable<TModel>> FindAll();
}
