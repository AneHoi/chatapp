drop table if exists chatroomies.users;
drop table if exists chatroomies.chats;
drop schema if exists chatroomies;


create schema chatroomies

create table chatroomies.users(
    username        varchar(40)
);

create table chatroomies.chats(
    username        varchar(120),
    messageContent  varchar(200),
    room            int
);