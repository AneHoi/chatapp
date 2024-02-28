using System.Text.Json;
using Fleck;
using fleckproject.EventFilters;
using lib;

namespace fleckproject;

public class ClientWantsToSignInDto : BaseDto
{
    public string Username { get; set; }
}

[RateLimit(1, 30)]
public class ClientWantsToSignIn : BaseEventHandler<ClientWantsToSignInDto>
{
    public override Task Handle(ClientWantsToSignInDto dto, IWebSocketConnection socket)
    {
        var webWithMeta = Connections.connectionsDictionary[socket.ConnectionInfo.Id].Username;
        var name = dto.Username;
        Connections.connectionsDictionary[socket.ConnectionInfo.Id].Username = dto.Username;
        socket.Send(JsonSerializer.Serialize(new ServerWelcomesUser
        {
            message = "Welcome to the chat application"
        }));
        return Task.CompletedTask;
    }
}

public class ServerWelcomesUser : BaseDto
{
    public string message { get; set; }
}