﻿using Dapper;
using infrastructure.datamodels;
using infrastructure.Models;
using Npgsql;

namespace infrastructure;

public class ChatRepository
{
    private readonly NpgsqlDataSource _dataSource;

    public ChatRepository(NpgsqlDataSource dataSource)
    {
        _dataSource = dataSource;
    }

    public void CreateUser(string username)
    {
        const string sql = $@"
            INSERT INTO chatroomies.users (username)
            VALUES (@username)
            ";
        using (var connection = _dataSource.OpenConnection())
        {
            connection.Execute(sql, new { username });
        }
    }

    public User FindByName(string username)
    {
        const string sql = $@"
            SELECT
                id as {{nameof(User.id)}},
                username as {{nameof(User.username)}},
                tlfnumber as {{nameof(User.tlfnumber)}},
                email as {{nameof(User.email)}}
            FROM taxapp.users
            WHERE id = @id;
         ";
        using (var connection = _dataSource.OpenConnection())
        {
            return connection.QueryFirst<User>(sql, new { username });
        }
    }

    public void SaveChat(ChatMessage message)
    {
        const string sql = $@"
            INSERT INTO chatroomies.chats(username, messagecontent, room)
            VALUES (@username, @messagecontent, @room)
            ";

        using (var connection = _dataSource.OpenConnection())
        {
            connection.Execute(sql, new { message.username, message.messageContent, message.room });
        }
    }

    public IEnumerable<ChatMessage> getAllChatsFromRoom(int roomnr)
    {
        const string sql = $@"
            SELECT 
                messagecontent as {nameof(ChatMessage.messageContent)},
                username as {nameof(ChatMessage.username)},
                room as {nameof(ChatMessage.room)}
            FROM chatroomies.chats 
            WHERE room = @roomnr
            ";
        using (var connection = _dataSource.OpenConnection())
        {
            return connection.Query<ChatMessage>(sql, new {roomnr});
        }
    }
}
