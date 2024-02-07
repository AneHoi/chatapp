using infrastructure;
using infrastructure.Models;
using Microsoft.Extensions.Logging;

namespace service;

public class ChatService
{

    private readonly ILogger<ChatService> _logger;
    private readonly ChatRepository _chatReository;

    public ChatService(ILogger<ChatService> logger, ChatRepository chatRepository)
    {
        _logger = logger;
        _chatReository = chatRepository;
    }

    public User CreateUser(string username)
    {
        _chatReository.CreateUser(username);
        return _chatReository.FindByName(username);
    }

    public void SaveChatMessage(ChatMessage message)
    {
        _chatReository.SaveChat(message);
    }

    public IEnumerable<ChatMessage> getAllChatsFromRoom(int roomNr)
    {
        return _chatReository.getAllChatsFromRoom();
    }
    
}
