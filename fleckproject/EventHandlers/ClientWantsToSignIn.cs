using System.Text.Json;
using Fleck;
using lib;

namespace fleckproject;


public class ClientWantsToSignInDto : BaseDto
{
    public string Username { get; set; }
}

public class ClientWantsToSignIn : BaseEventHandler<ClientWantsToSignInDto>
{
    public override Task Handle(ClientWantsToSignInDto dto, IWebSocketConnection socket)
    {
        Connections.connectionsDictionary[socket.ConnectionInfo.Id].Username = dto.Username;
        socket.Send(JsonSerializer.Serialize(new ServerWelcomesUser()));
        return Task.CompletedTask;
    }
}

public class ServerWelcomesUser : BaseDto
{
    
}