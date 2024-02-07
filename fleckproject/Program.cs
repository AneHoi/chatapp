using System.Reflection;
using System.Text.Json;
using Fleck;
using fleckproject;
using lib;

var builder = WebApplication.CreateBuilder(args);

var clientEventHandlers = builder.FindAndInjectClientEventHandlers(Assembly.GetExecutingAssembly());
var app = builder.Build();


var server = new WebSocketServer("ws://0.0.0.0:8181");
var wsAllConnections = new List<IWebSocketConnection>();
var wsMovieConnections = new List<IWebSocketConnection>();

server.Start(socket =>
{
    socket.OnOpen = () =>
    {
        Connections.AddConnection(socket);
        /*Connections.ConnectionsInMemory.Add(socket);
        Connections.ConnectionsInMemory.ForEach(ws =>
        {
            if (ws != socket)
            {
                ws.Send(JsonSerializer.Serialize(new PeopleCounter()
                {
                    numOfPeopleValue = Connections.ConnectionsInMemory.Count,
                    infoMessage = "A new user has joined the chat"
                }));
            }
            else
            {
                ws.Send(JsonSerializer.Serialize(new PeopleCounter()
                {
                    numOfPeopleValue = Connections.ConnectionsInMemory.Count,
                    infoMessage = "Welcome to the chat"
                }));
            }
        });*/
    };

    socket.OnMessage = async message =>
    {
        try
        {
            app.InvokeClientEventHandler(clientEventHandlers, socket, message);
        }
        catch (Exception e)
        {
            Console.WriteLine(e.Message);
            Console.WriteLine(e.InnerException);
            Console.WriteLine(e.StackTrace);
        }
    };
    socket.OnClose = () =>
    {
        Connections.ConnectionsInMemory.Remove(socket);
        Connections.ConnectionsInMemory.ForEach(ws =>
        {
            ws.Send(JsonSerializer.Serialize(new PeopleCounter()
            {
                numOfPeopleValue = Connections.ConnectionsInMemory.Count,
                infoMessage = "A user has left the chat"
            }));
        });
    };
});


WebApplication.CreateBuilder(args).Build().Run();