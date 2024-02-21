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
    public async Task ClientsWansToEchoServer()
    {
        var ws = await new WebSocketTestClient().ConnectAsync();
        await ws.DoAndAssert(new ClientWansToEchoServerDto()
        {
            messageContent = "Hello"
        }, response => response.Count(dto => dto.eventType == nameof(ServerEchosClient)) == 1);
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