namespace fleckproject.Exeptions;

public class TooMuchHateSpeechExeption : Exception
{
    public TooMuchHateSpeechExeption()
    {
    }

    public TooMuchHateSpeechExeption(string message)
        : base(message)
    {
    }

    public TooMuchHateSpeechExeption(string message, Exception inner)
        : base(message, inner)
    {
    }
}