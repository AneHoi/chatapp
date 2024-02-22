using System.Text.Json;
using Fleck;
using fleckproject.EventFilters;
using lib;

namespace fleckproject;

public class ClientWantsToSignInDto : BaseDto
{
    public string Username { get; set; }
}

[RateLimit(2, 30)]
public class ClientWantsToSignIn : BaseEventHandler<ClientWantsToSignInDto>
{
    public override Task Handle(ClientWantsToSignInDto dto, IWebSocketConnection socket)
    {
        var webWithMeta = Connections.connectionsDictionary[socket.ConnectionInfo.Id].Username;
        var name = dto.Username;
        Connections.connectionsDictionary[socket.ConnectionInfo.Id].Username = dto.Username;
        Console.WriteLine("the username for the given person is: " + dto.Username + "\nBut current is: " + webWithMeta);
        socket.Send(JsonSerializer.Serialize(new ServerWelcomesUser()));
        return Task.CompletedTask;
    }
}

public class ServerWelcomesUser : BaseDto
{
}