DROP TABLE IF EXISTS ChatApp.chats;
DROP TABLE IF EXISTS ChatApp.password_hash;
DROP TABLE IF EXISTS ChatApp.users;

DROP SCHEMA IF EXISTS ChatApp;

CREATE SCHEMA ChatApp;
CREATE TABLE ChatApp.chats
(
    userid          varchar(100) PRIMARY KEY,
    messagecontent  varchar(20) NOT NULL
);
create table ChatApp.users (
    id          SERIAL          PRIMARY KEY,
    username    VARCHAR(50)     NOT NULL,
    tlfnumber   INT,
    email       VARCHAR(50)     NOT NULL UNIQUE
);

create table ChatApp.password_hash (
    user_id         integer,                            
    hash            VARCHAR(350)    NOT NULL,
    salt            VARCHAR(180)    NOT NULL,
    algorithm       VARCHAR(12)     NOT NULL,
    FOREIGN KEY(user_id) REFERENCES ChatApp.users(id)
);

--adding a tester
--the test password is: 12345678
INSERT INTO ChatApp.users (username, tlfnumber, email)
VALUES ('Tester', 12345678, 'test@mail.com');
INSERT INTO ChatApp.password_hash (user_id, hash, salt, algorithm)
VALUES ((SELECT ChatApp.users.id FROM ChatApp.users WHERE ChatApp.users.email = 'test@mail.com'),
        '1EJybmIbon7kimzpBZXA17OxI3/iVLZK8euSAloQgK3W8ibEJ8G/Ql2J4kjtDDMRV5sN71LEgRuL+lXyP9dOHz9IuMXuWjTdFSwkKaDNbiNa9MsWy/dngKWo04jYvG8Tb26UV0Bnxd83V9zQZCPdPSQXENoRvPOhnDZKaayFYuRz4pVkBrooL9Hu9EgrCzE9Z3kExf+w1BwR/hqVip2wj+W3mxBwTWgm5hhsko1TZqr3d+HWPAeaFmaNTmwuG0miPhA8H9C4/V0mUs62V2zJkZEVP3QEipvTvkCyctxq7U89NSLwVIGiEsmFG/sZ1EqXnXpmpbV1PQ7pkDYFad+pzQ==',
        'sMQAck67hWo2asVpqlbmmGVFj3jo6i86oZVTQh3c3wOpKd0LO8oxqSYhveceXkLrXlCKIIVFB+IRPXrcE3ZkFdVKmG5A7gOyvWwkOltwOytSDoPHmT3+aWUS0sFjO89RMbJxCncsghBbtF3a9hHtr/7/NcexUj8wJQz48gq6izw=',
        'argon2id');

SELECT * FROM ChatApp.users;
SELECT * FROM ChatApp.password_hash;
SELECT * FROM ChatApp.chats;