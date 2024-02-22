using System.Data;
using System.Text.Json;
using Fleck;
using lib;

namespace fleckproject;

public class ClientWantsToSendToChatRoomDto : BaseDto
{
    public string messageContent { get; set; }
    public int roomId { get; set; }
}

[RateLimit(5,10)]
public class ClientWantsToSendToChatRoom : BaseEventHandler<ClientWantsToSendToChatRoomDto>
{
    public override Task Handle(ClientWantsToSendToChatRoomDto dto, IWebSocketConnection socket)
    {
        var message = new ServerBroardcastsMessageWithUsername()
        {
            message = dto.messageContent,
            username = Connections.connectionsDictionary[socket.ConnectionInfo.Id].Username
        };
        Connections.BroadcastToRooms(dto.roomId, JsonSerializer.Serialize(message));
        return Task.CompletedTask;
    }
}

public class ServerBroardcastsMessageWithUsername : BaseDto
{
    public string message { get; set; }
    public string username { get; set; }
}