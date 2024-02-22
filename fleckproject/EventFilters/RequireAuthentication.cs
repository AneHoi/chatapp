using System.Security.Authentication;
using api;
using Fleck;
using lib;
using Microsoft.AspNetCore.Mvc.Filters;

namespace fleckproject.EventFilters;

/**
 * Protecting our end-point by using a filter, that we define here
 */
public class Auth : BaseEventFilter
{
    public override Task Handle<T>(IWebSocketConnection socket, T dto)
    {
        //This filter checks for login, and if not logged in, an Exeption is thrown
        var user = Connections.connectionsDictionary[socket.ConnectionInfo.Id].Username;
        if (string.IsNullOrEmpty(user))
            throw new AuthenticationException("You dont have a username");
        return Task.CompletedTask;
    }
}