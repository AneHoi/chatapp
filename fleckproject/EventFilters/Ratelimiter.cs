using System.ComponentModel.DataAnnotations;
using System.Threading.RateLimiting;
using Fleck;
using fleckproject;
using lib;

[AttributeUsage(AttributeTargets.Class, AllowMultiple = true)]
public class RateLimitAttribute(int eventsPerTimeframe, int secondTimeFrame) : BaseEventFilter
{
    public override async Task Handle<T>(IWebSocketConnection socket, T dto)
    {
        var env = Environment.GetEnvironmentVariable("FULLSTACK_SKIP_RATE_LIMITING")!;
        if (env.ToLower().Equals("true"))
        {
            Console.WriteLine("SKIP RATE LIMITER: " + env);
            await Task.CompletedTask;
            return;
        }


        //Get the websocket with metadata
        var metadata = Connections.connectionsDictionary[socket.ConnectionInfo.Id];
        if (!metadata.RateLimitPerEvent.TryGetValue(dto.eventType, out var rateLimiter))
        {
            rateLimiter = new FixedWindowRateLimiter(new FixedWindowRateLimiterOptions
            {
                PermitLimit = eventsPerTimeframe,
                Window = TimeSpan.FromSeconds(secondTimeFrame),
                AutoReplenishment = true
            });
            metadata.RateLimitPerEvent[dto.eventType] = rateLimiter;
        }

        var lease = await rateLimiter.AcquireAsync();
        if (!lease.IsAcquired) throw new ValidationException("Rate limit exceeded for event " + dto.eventType);
    }
}