using System.Text.Json;
using Fleck;
using lib;

namespace fleckproject;


public class ClientWantsToBroardCastDto : BaseDto
{
    public string messageContent { get; set; }

}


public class ClientWantsToBroardCast : BaseEventHandler<ClientWantsToBroardCastDto>
{
    public override Task Handle(ClientWantsToBroardCastDto dto, IWebSocketConnection socket)
    {
        var message = new ServerSendsMessage()
        {
            message = dto.messageContent
        };
        var messageToClients = JsonSerializer.Serialize(message);
        foreach (var s in Connections.connectionsDictionary.Values)
        {
            s.Connection.Send(messageToClients);
        }
        return Task.CompletedTask;
    }
}
public class ServerSendsMessage : BaseDto
{
    public string message { get; set;}
}