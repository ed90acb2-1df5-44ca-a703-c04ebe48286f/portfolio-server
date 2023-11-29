using Portfolio.Server.Net;

namespace Portfolio.Server.Filters;

public interface IConnectionFilter
{
    bool Filter(Connection connection, object request);
}
