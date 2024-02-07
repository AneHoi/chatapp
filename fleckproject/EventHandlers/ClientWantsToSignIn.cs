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
        return Task.CompletedTask;
    }
}