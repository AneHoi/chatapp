using System.Text.Json;
using Fleck;
using lib;
using HostingEnvironmentExtensions = Microsoft.Extensions.Hosting.HostingEnvironmentExtensions;

namespace fleckproject;

public class PeopleCounter : BaseDto
{
    public int numOfPeopleValue { get; set; }
    public string infoMessage { get; set; }
}

public class AllRooms : BaseDto
{
    public List<int> roomIds { get; set; }
}