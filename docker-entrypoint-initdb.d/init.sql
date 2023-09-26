CREATE DATABASE todolist;
CREATE USER 'testuser'@'%' IDENTIFIED BY 'testuserpassword';
GRANT ALL PRIVILEGES ON todolist.* TO 'testuser'@'%';
FLUSH PRIVILEGES;