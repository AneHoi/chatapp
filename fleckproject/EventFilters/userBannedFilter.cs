using fleckproject.Exeptions;

namespace fleckproject.EventFilters;

using System.ComponentModel.DataAnnotations;
using System.Threading.RateLimiting;
using Fleck;
using fleckproject;
using lib;

[AttributeUsage(AttributeTargets.Class, AllowMultiple = true)]
public class userBannedFilter() : BaseEventFilter
{
    public override async Task Handle<T>(IWebSocketConnection socket, T dto)
    {
        //Get the websocket with metadata
        var metadata = Connections.connectionsDictionary[socket.ConnectionInfo.Id];
        if (metadata.Warnings >= 3)
        {
            throw new TooMuchHateSpeechExeption("You have been sending too much hate speech in this application");
        }
    }
}