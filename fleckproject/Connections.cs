using System.Text.Json;
using System.Threading.RateLimiting;
using Fleck;

namespace fleckproject;

public class WebSocetWithMetaData()
{
    public IWebSocketConnection? connection { get; set; }
    public string Username { get; set; }
    
    public int Warnings { get; set; }
    public Dictionary<string, RateLimiter> RateLimitPerEvent { get; set; } = new();
}

public static class Connections
{
    public static Dictionary<Guid, WebSocetWithMetaData> connectionsDictionary = new();

    //HashSets has really fast lookup time compared to List
    public static Dictionary<int, HashSet<Guid>> chatRooms = new();

    public static bool AddConnection(IWebSocketConnection socket)
    {
        Console.WriteLine("A new user has joined, id: " + socket.ConnectionInfo.Id);
        return connectionsDictionary.TryAdd(socket.ConnectionInfo.Id, new WebSocetWithMetaData
        {
            connection = socket
        });
    }

    public static void AddToRoom(IWebSocketConnection socket, int room)
    {
        Console.WriteLine("room is available " + chatRooms.ContainsKey(room));

        if (!chatRooms.ContainsKey(room))
            chatRooms.Add(room, new HashSet<Guid>());
        Console.WriteLine("Added to room " + room);

        List<int> rooms = new List<int>();
        foreach (var chatRoom in chatRooms)
        {
            rooms.Add(chatRoom.Key);
        }

        socket.Send(JsonSerializer.Serialize(new AllRooms()
        {
            roomIds = rooms
        }));
        
        chatRooms[room].Add(socket.ConnectionInfo.Id);
    }

    public static void BroadcastToRooms(int room, string message)
    {
        Console.WriteLine("Inside broardcast");
        if (chatRooms.ContainsKey(room))
        {
            HashSet<Guid> chatrooms = chatRooms[room];
            foreach (var socetId in chatrooms)
            {
                var socetWithMetaData = Connections.connectionsDictionary[socetId];
                socetWithMetaData.connection.Send(message);
            }
        }
    }
}