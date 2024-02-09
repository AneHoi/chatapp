using System.Reflection;
using System.Text.Json;
using Fleck;
using fleckproject;
using lib;

var builder = WebApplication.CreateBuilder(args);


var clientEventHandlers = builder.FindAndInjectClientEventHandlers(Assembly.GetExecutingAssembly());
var app = builder.Build();


var server = new WebSocketServer("ws://0.0.0.0:8181");

server.Start(socket =>
{
    socket.OnOpen = () =>
    {
        Connections.AddConnection(socket);
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
            socket.Send(e.Message);
            Console.WriteLine(e.Message);
            Console.WriteLine(e.InnerException);
            Console.WriteLine(e.StackTrace);
        }
    };
    socket.OnClose = () =>
    {
        Connections.connectionsDictionary.Remove(socket.ConnectionInfo.Id);
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


WebApplication.CreateBuilder(args).Build().Run();