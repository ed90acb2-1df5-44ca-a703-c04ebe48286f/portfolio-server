using Portfolio.Server.Net;
using Portfolio.Server.Security;

namespace Portfolio.Server.Filters;

public class AuthenticationConnectionFilter : IConnectionFilter
{
    private readonly Authentication _authentication;

    public AuthenticationConnectionFilter(Authentication authentication)
    {
        _authentication = authentication;
    }

    public bool Filter(Connection connection)
    {
        return _authentication.IsAuthenticated(connection);
    }
}
