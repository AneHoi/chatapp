using System.Text.Json;
using Fleck;
using lib;

namespace fleckproject;

public class ClientWantsToEnterRoomDto : BaseDto
{
    public int roomId { get; set; }
}

[RateLimit(5,10)]
public class ClientWantsToEnterRoom : BaseEventHandler<ClientWantsToEnterRoomDto>
{
    public override Task Handle(ClientWantsToEnterRoomDto dto, IWebSocketConnection socket)
    {
        var isSucess = Connections.AddToRoom(socket, dto.roomId);
        socket.Send(JsonSerializer.Serialize(new ServerAddsClientToRoom()
        {
            message = "You are now added to the room " + dto.roomId
        }));
        return Task.CompletedTask;
    }
}

public class ServerAddsClientToRoom : BaseDto
{
    public string message { get; set; }
}