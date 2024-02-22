using System.Security.Authentication;
using fleckproject;
using lib;
using NUnit.Framework.Internal;

namespace test;

public class Tests
{
    [SetUp]
    public void Setup()
    {
        //We want to start the api from the test code
        Startup.Statup(null);
    }

    //[Repeat(3)] //Testing the ratelimiter
    [Test]
    public async Task LoginClient()
    {
        var ws = await new WebSocketTestClient().ConnectAsync();
        await ws.DoAndAssert(new ClientWantsToSignInDto()
        {
            Username = "Bob"
        }, response => response.Count(dto => dto.eventType == nameof(ServerWelcomesUser)) == 1);
    }


    [Test]
    public async Task ClientsWansToEchoServerWithNoAuth()
    {
        var ws = await new WebSocketTestClient().ConnectAsync();
        await ws.DoAndAssert(new ClientWansToEchoServerDto()
        {
            messageContent = "Hello"
        }, response => response.Count(dto => dto.eventType == nameof(ServerSendsErrorMessageToClient)) == 1);
        //authentication is missing, so the expected value is the Errormessage
    }

    [TestCase(2)]
    public async Task ClientsWansToEchoServerAndTriggerRateLimiter(int expectedTop)
    {
        var ws = await new WebSocketTestClient().ConnectAsync();
        await ws.DoAndAssert(new ClientWantsToSignInDto()
        {
            Username = "Bob"
        }, response => response.Count(dto => dto.eventType == nameof(ServerWelcomesUser)) == 1);

        for (int i = 1; i < 10; i++)
        {
            if (i <= expectedTop)
            { 
                await ws.DoAndAssert(new ClientWansToEchoServerDto()
                {
                    messageContent = "This should work"
                }, response => response.Count(dto => checkForeventType(dto, expectedTop, i)) == i);
            }
            else
            {
                await ws.DoAndAssert(new ClientWansToEchoServerDto()
                {
                    messageContent = "This should be rejected"
                }, response => response.Count(dto => checkForeventType(dto, expectedTop, i)) == i-expectedTop);
            }
        }
    }

    private bool checkForeventType(BaseDto dto, int top, int counter)
    {
        if (counter > top)
        {
            return dto.eventType == nameof(ServerSendsErrorMessageToClient);
        }
        else
        {
            return dto.eventType == nameof(ServerEchosClient);
        }
    }

    [Test]
    public async Task EnterRoom()
    {
        var ws = await new WebSocketTestClient().ConnectAsync();
        await ws.DoAndAssert(new ClientWantsToEnterRoomDto()
        {
            roomId = 1
        }, response => response.Count(dto => dto.eventType == nameof(ServerAddsClientToRoom)) == 1);
    }

    [Test]
    public async Task ClientWantsToBroardcastToRoom()
    {
        var ws = await new WebSocketTestClient().ConnectAsync();
        var ws2 = await new WebSocketTestClient().ConnectAsync();


        await ws.DoAndAssert(new ClientWantsToSendToChatRoomDto()
        {
            messageContent = "Hey Alice",
            roomId = 1
        }, response => response.Count(dto => dto.eventType == nameof(ServerBroardcastsMessageWithUsername)) == 0);
        //Expecting not to get any response, because username is missing

        await ws2.DoAndAssert(new ClientWantsToSendToChatRoomDto()
        {
            messageContent = "Hey Bob",
            roomId = 1
        }, response => response.Count(dto => dto.eventType == nameof(ServerBroardcastsMessageWithUsername)) == 0);
        //Expecting not to get any response, because username is missing
    }


    [Test]
    public async Task TwoClientsChatting()
    {
        var ws = await new WebSocketTestClient().ConnectAsync();
        var ws2 = await new WebSocketTestClient().ConnectAsync();
        //Takes in an action, and an assertion
        await ws.DoAndAssert(new ClientWantsToSignInDto()
        {
            Username = "Bob"
        }, response => response.Count(dto => dto.eventType == nameof(ServerWelcomesUser)) == 1);

        await ws2.DoAndAssert(new ClientWantsToSignInDto()
        {
            Username = "Alice"
        }, response => response.Count(dto => dto.eventType == nameof(ServerWelcomesUser)) == 1);

        await ws.DoAndAssert(new ClientWantsToEnterRoomDto()
        {
            roomId = 1
        }, response => response.Count(dto => dto.eventType == nameof(ServerAddsClientToRoom)) == 1);

        await ws2.DoAndAssert(new ClientWantsToEnterRoomDto()
        {
            roomId = 1
        }, response => response.Count(dto => dto.eventType == nameof(ServerAddsClientToRoom)) == 1);


        await ws.DoAndAssert(new ClientWantsToSendToChatRoomDto()
        {
            messageContent = "Hey Alice",
            roomId = 1
        }, response => response.Count(dto => dto.eventType == nameof(ServerBroardcastsMessageWithUsername)) == 1);

        await ws2.DoAndAssert(new ClientWantsToSendToChatRoomDto()
        {
            messageContent = "Hey Bob",
            roomId = 1
        }, response => response.Count(dto => dto.eventType == nameof(ServerBroardcastsMessageWithUsername)) == 2);
    }
}