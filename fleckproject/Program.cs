using System.Reflection;
using System.Text.Json;
using Fleck;
using lib;

//A class with a startup method for testing 
namespace fleckproject;

public static class Startup
{
    public static void Main(string[] args)
    {
        Statup(args);
        WebApplication.CreateBuilder(args).Build().Run();
    }

    public static void Statup(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);


        var clientEventHandlers = builder.FindAndInjectClientEventHandlers(Assembly.GetExecutingAssembly());
        var app = builder.Build();


        var server = new WebSocketServer("ws://0.0.0.0:8181");
        var counter = 1;
        server.Start(socket =>
        {
            socket.OnOpen = () =>
            {
                counter++;
                Connections.AddConnection(socket);
                foreach (var webSocetWithMetaData in Connections.connectionsDictionary)
                {
                    webSocetWithMetaData.Value.Connection.Send(JsonSerializer.Serialize(new PeopleCounter()
                        {
                            numOfPeopleValue = Connections.connectionsDictionary.Count,
                            infoMessage = "A user has joined the chat"
                        })
                    );
                    Connections.AddToRoom(socket, counter);
                    List<int> rooms = new List<int>();
                    foreach (var chatRoom in Connections.chatRooms)
                    {
                        rooms.Add(chatRoom.Key);
                    }

                    webSocetWithMetaData.Value.Connection.Send(JsonSerializer.Serialize(new AllRooms()
                        {
                            roomIds = rooms
                        })
                    );
                }
            };

            socket.OnMessage = async message =>
            {
                //This is my global exception handler, and it works, because ALL the traffic goes though the app.InvokeClientEventHandler()
                try
                {
                    await app.InvokeClientEventHandler(clientEventHandlers, socket, message);
                }
                catch (Exception e)
                {
                    Console.WriteLine("An error has occored");
                    socket.Send(e.Message);
                    Console.WriteLine(e.Message);
                    Console.WriteLine(e.InnerException);
                    Console.WriteLine(e.StackTrace);
                }
            };
            socket.OnClose = () =>
            {
                Connections.connectionsDictionary.Remove(socket.ConnectionInfo.Id);
                Console.WriteLine("Currently in the chat " + Connections.connectionsDictionary.Count);
                foreach (var webSocetWithMetaData in Connections.connectionsDictionary)
                {
                    webSocetWithMetaData.Value.Connection.Send(JsonSerializer.Serialize(new PeopleCounter()
                        {
                            numOfPeopleValue = Connections.connectionsDictionary.Count,
                            infoMessage = "A user has left the chat"
                        })
                    );
                }
            };
        });
    }
}