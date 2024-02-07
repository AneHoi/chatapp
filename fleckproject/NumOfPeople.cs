using System.Text.Json;
using Fleck;
using lib;
using HostingEnvironmentExtensions = Microsoft.Extensions.Hosting.HostingEnvironmentExtensions;

namespace fleckproject;

public class NumOfPeopleDto : BaseDto
{
    public int numOfPeople { get; set; }
}


public class NumOfPeople : BaseEventHandler<NumOfPeopleDto>
{
    public override Task Handle(NumOfPeopleDto dto, IWebSocketConnection socket)
    {
        var peopleCounter = new PeopleCounter()
        {
            numOfPeopleValue = Connections.ConnectionsInMemory.Count
        };
        var messageToClient = JsonSerializer.Serialize(peopleCounter);
        foreach (var ws in Connections.ConnectionsInMemory)
        {
            ws.Send(messageToClient);
        }
        return Task.CompletedTask;
    }
}

public class PeopleCounter : BaseDto
{
    public int numOfPeopleValue { get; set; }
    public string infoMessage { get; set; }
}