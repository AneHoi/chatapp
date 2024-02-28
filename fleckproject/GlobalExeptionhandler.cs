using System.ComponentModel.DataAnnotations;
using System.Security.Authentication;
using System.Text.Json;
using Fleck;
using fleckproject.Exeptions;
using lib;

namespace fleckproject;

public static class GlobalExeptionhandler
{
    public static void Handle(
        Exception exeption,
        IWebSocketConnection socket,
        string? message)
    {
        Console.WriteLine(exeption.Message, "Global exeption handler");

        //the different exeptions are sent to the user in these "if"-statements
        if (exeption is AuthenticationException)
        {
            socket.Send(JsonSerializer.Serialize(new ServerSendsErrorMessageToClient()
            {
                errorMessage = exeption.Message
            }));
        }

        if (exeption is ValidationException)
        {
            socket.Send(JsonSerializer.Serialize(new ServerSendsErrorMessageToClient()
            {
                errorMessage = exeption.Message
            }));
        }

        if (exeption is TooMuchHateSpeechExeption)
        {
            socket.Send(JsonSerializer.Serialize(new TooMuchHateSpeech()
            {
                errorMessage = exeption.Message
            }));
        }
    }
}

public class TooMuchHateSpeech : BaseDto
{
    public string errorMessage { get; set; }
}

public class ServerSendsErrorMessageToClient : BaseDto
{
    public string errorMessage { get; set; }
}

