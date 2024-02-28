using System.ComponentModel.DataAnnotations;
using System.Data;
using System.Text.Json;
using Fleck;
using fleckproject.EventFilters;
using lib;
using System.Net.Http;
using System.Net.Http.Headers;

namespace fleckproject;

public class ClientWantsToSendToChatRoomDto : BaseDto
{
    public string messageContent { get; set; }
    public int roomId { get; set; }
}
[Auth]
[userBannedFilter]
[RateLimit(5,10)]
public class ClientWantsToSendToChatRoom : BaseEventHandler<ClientWantsToSendToChatRoomDto>
{
    public override async Task Handle(ClientWantsToSendToChatRoomDto dto, IWebSocketConnection socket)
    {
        await isMessageToxic(dto.messageContent, socket);
        var message = new ServerBroardcastsMessageWithUsername()
        {
            message = dto.messageContent,
            username = Connections.connectionsDictionary[socket.ConnectionInfo.Id].Username
        };
        
        Connections.BroadcastToRooms(dto.roomId, JsonSerializer.Serialize(message));
    }

    public record RequestModel(string text, List<string> categories, string outputType)
    {
        public override string ToString()
        {
            return $"{{ text = {text}, categories = {categories}, outputType = {outputType} }}";
        }
    }

    //Not returning a value, it is just performing a task
    private async Task isMessageToxic(string message, IWebSocketConnection socket)
    {
        HttpClient client = new HttpClient();

        HttpRequestMessage request = new HttpRequestMessage(HttpMethod.Post, "https://chatfilterane.cognitiveservices.azure.com/contentsafety/text:analyze?api-version=2023-10-01");

        request.Headers.Add("accept", "application/json");
        request.Headers.Add("Ocp-Apim-Subscription-Key", Environment.GetEnvironmentVariable("KEY"));

        var req = new RequestModel(message, new List<string>() { "Hate", "Violence" }, "FourSeverityLevels");
        
        request.Content = new StringContent(JsonSerializer.Serialize(req));
        request.Content.Headers.ContentType = new MediaTypeHeaderValue("application/json");

        HttpResponseMessage response = await client.SendAsync(request);
        response.EnsureSuccessStatusCode();
        string responseBody = await response.Content.ReadAsStringAsync();
        var obj = JsonSerializer.Deserialize<ContentFilterResponse>(responseBody);
        var isToxic = obj.categoriesAnalysis.Count(e => e.severity > 1) >=  1;
        if (isToxic){
            //Get the websocket with metadata
            Connections.connectionsDictionary[socket.ConnectionInfo.Id].Warnings++;
            throw new ValidationException("Bad speech is not allowed");
        }
    }
}

public class ServerBroardcastsMessageWithUsername : BaseDto
{
    public string message { get; set; }
    public string username { get; set; }
}
public class CategoriesAnalysis
{
    public string category { get; set; }
    public int severity { get; set; }
}

public class ContentFilterResponse
{
    public List<object> blocklistsMatch { get; set; }
    public List<CategoriesAnalysis> categoriesAnalysis { get; set; }
}

