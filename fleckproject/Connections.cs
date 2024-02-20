using Fleck;

namespace fleckproject;

public class WebSocetWithMetaData(IWebSocketConnection connection)
{
    public IWebSocketConnection Connection { get; set; } = connection;
    public string Username { get; set; }
}

public static class Connections
{

    public static Dictionary<Guid, WebSocetWithMetaData> connectionsDictionary = new ();
    //HashSets has really fast lookup time compared to List
    public static Dictionary<int, HashSet<Guid>> chatRooms = new ();
    public static bool AddConnection(IWebSocketConnection socket)
    {
        Console.WriteLine("A new user has joined, id: " + socket.ConnectionInfo.Id);
        return connectionsDictionary.TryAdd(socket.ConnectionInfo.Id, new WebSocetWithMetaData(socket));
    }

    public static bool AddToRoom(IWebSocketConnection socket, int room)
    {
        if(!chatRooms.ContainsKey(room))
            chatRooms.Add(room, new HashSet<Guid>());
        return chatRooms[room].Add(socket.ConnectionInfo.Id);
        
    }

    public static void BroadcastToRooms(int room, string message)
    {
        if (chatRooms.TryGetValue(room, out var guids))
        {
            foreach (var guid in guids)
            {
                if (connectionsDictionary.TryGetValue(guid, out var socket))
                {
                    socket.Connection.Send(message);
                }
            }
        }
    }
}