using System.Text.Json;
using Fleck;
using fleckproject.EventFilters;
using lib;

namespace fleckproject;

public class ClientWansToEchoServerDto : BaseDto
{
    public string messageContent { get; set; }
}

[Auth]
[RateLimit(2,10)]
//For deserializing objects'
public class ClientWansToEchoServer : BaseEventHandler<ClientWansToEchoServerDto> 
{
    //Everything inside here, wil be run on the trigger(event) from the client
    public override Task Handle(ClientWansToEchoServerDto dto, IWebSocketConnection socket)
    {
        var echo = new ServerEchosClient()
        {
            message = dto.messageContent
        };
        var messagToClient = JsonSerializer.Serialize(echo);
        
        socket.Send(messagToClient);
        return Task.CompletedTask;
    }
}

public class ServerEchosClient : BaseDto
{
    public string message { get; set;}
}