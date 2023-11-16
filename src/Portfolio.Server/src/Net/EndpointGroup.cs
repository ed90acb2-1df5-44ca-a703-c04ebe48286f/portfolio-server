using System.Collections.Generic;
using Portfolio.Server.Filters;

namespace Portfolio.Server.Net;

public readonly struct EndpointGroup
{
    private readonly List<Endpoint> _endpoints = new();

    public EndpointGroup()
    {
    }

    public EndpointGroup Add(Endpoint endpoint)
    {
        _endpoints.Add(endpoint);
        return this;
    }

    public EndpointGroup Filter(IConnectionFilter filter)
    {
        foreach (var endpoint in _endpoints)
        {
            endpoint.Filter(filter);
        }

        return this;
    }
}
