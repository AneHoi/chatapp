using System.ComponentModel.DataAnnotations;
using System.Text.Json;
using Fleck;
using lib;
using Microsoft.Net.Http.Headers;
using MediaTypeHeaderValue = System.Net.Http.Headers.MediaTypeHeaderValue;

namespace fleckproject.EventFilters;

public class ContentSafetyFilter : BaseEventFilter
{
    public override Task Handle<T>(IWebSocketConnection socket, T dto)
    {
       var str = (JsonSerializer.Serialize(dto));
        ClientWantsToSendToChatRoomDto d = JsonSerializer.Deserialize<ClientWantsToSendToChatRoomDto>(str);
        isMessageToxic(d.messageContent).Wait();
        return Task.CompletedTask;
    }
    
    private async Task isMessageToxic(string message)
    {
        HttpClient client = new HttpClient();

        HttpRequestMessage request = new HttpRequestMessage(HttpMethod.Post, "https://chatfilterane.cognitiveservices.azure.com/contentsafety/text:analyze?api-version=2023-10-01");

        request.Headers.Add("accept", "application/json");
        request.Headers.Add("Ocp-Apim-Subscription-Key", Environment.GetEnvironmentVariable("KEY"));

        var req = new ClientWantsToSendToChatRoom.RequestModel(message, new List<string>() { "Hate", "Violence" }, "FourSeverityLevels");
        
        request.Content = new StringContent(JsonSerializer.Serialize(req));
        request.Content.Headers.ContentType = new MediaTypeHeaderValue("application/json");

        HttpResponseMessage response = await client.SendAsync(request);
        response.EnsureSuccessStatusCode();
        string responseBody = await response.Content.ReadAsStringAsync();
        var obj = JsonSerializer.Deserialize<ContentFilterResponse>(responseBody);
        var isToxic = obj.categoriesAnalysis.Count(e => e.severity > 1) >=  1;
        if (isToxic){
            throw new ValidationException("Bad speech is not allowed");
        }
    }
}