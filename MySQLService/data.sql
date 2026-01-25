DROP TABLE IF EXISTS books_authors;
DROP TABLE IF EXISTS author;
DROP TABLE IF EXISTS reservation;
DROP TABLE IF EXISTS book;
DROP TABLE IF EXISTS editor;
DROP TABLE IF EXISTS user;
DROP TABLE IF EXISTS role;

CREATE DATABASE IF NOT EXISTS library;
USE library;

CREATE TABLE editor  (
	editor_id BIGINT NOT NULL AUTO_INCREMENT,
    name VARCHAR (255) NOT NULL,
    PRIMARY KEY (editor_id)
);

CREATE TABLE author (
	author_id BIGINT NOT NULL AUTO_INCREMENT,
    name VARCHAR (255) NOT NULL,
    surname VARCHAR (255) NOT NULL,
    PRIMARY KEY (author_id)
);

create table book (
	book_id BIGINT NOT NULL AUTO_INCREMENT,
	title VARCHAR (255) NOT NULL,
	isbn VARCHAR (255) NOT NULL,
	editor BIGINT NOT NULL,
	front_cover_path VARCHAR(500) UNIQUE,
	PRIMARY KEY (book_id),
	FOREIGN KEY (editor) REFERENCES editor(editor_id)
);

CREATE TABLE books_authors (
    id BIGINT AUTO_INCREMENT PRIMARY KEY,  
    book BIGINT,
    author BIGINT,
    FOREIGN KEY (book) REFERENCES book(book_id),
    FOREIGN KEY (author) REFERENCES author(author_id)
);

CREATE TABLE role (
    role_id BIGINT AUTO_INCREMENT PRIMARY KEY,
    name VARCHAR(50) UNIQUE NOT NULL
);

CREATE TABLE user (
    user_id BIGINT AUTO_INCREMENT PRIMARY KEY,
    name VARCHAR(100) NOT NULL,
    surname VARCHAR(100) NOT NULL,
    email VARCHAR(100) NOT NULL UNIQUE,
    password VARCHAR(60) NOT NULL,
    role BIGINT NOT NULL,
	FOREIGN KEY (role) REFERENCES role(role_id)
);

CREATE TABLE reservation (
    reservation_id BIGINT AUTO_INCREMENT PRIMARY KEY,
    user_id BIGINT NOT NULL,
    book_id BIGINT NOT NULL,
    reservation_date DATE NOT NULL,
    due_date DATE NOT NULL,
    FOREIGN KEY (user_id) REFERENCES user(user_id),
    FOREIGN KEY (book_id) REFERENCES book(book_id)
);

INSERT INTO editor (name) VALUES ('John Smith');
INSERT INTO editor (name) VALUES ('Emily Johnson');
INSERT INTO editor (name) VALUES ('Michael Brown');
INSERT INTO editor (name) VALUES ('Sophia Lee');
INSERT INTO editor (name) VALUES ('David Wilson');
INSERT INTO editor (name) VALUES ('Penguin Random House');
INSERT INTO editor (name) VALUES ('HarperCollins');
INSERT INTO editor (name) VALUES ('Bloomsbury Publishing');

INSERT INTO book (title, ISBN, editor, front_cover_path) 
VALUES ('The Great Gatsby', '9780743273565', 1, 'http://localhost:5282/images/1/3/1/917TF-0LAwL._SL1500_.jpg');
INSERT INTO book (title, ISBN, editor, front_cover_path) 
VALUES ('To Kill a Mockingbird', '9780061120084', 2, 'http://localhost:5282/images/2/4/2/9788807892790_0_0_536_0_75.jpg');
INSERT INTO book (title, ISBN, editor, front_cover_path) 
VALUES ('1984', '9780451524935', 3, 'http://localhost:5282/images/3/2/3/71uGFEVcEkL._SL1500_.jpg');
INSERT INTO book (title, ISBN, editor, front_cover_path) 
VALUES ('Pride and Prejudice', '9780141439518', 4, 'http://localhost:5282/images/4/1/4/71o0INZqSsL._SL1422_.jpg');
INSERT INTO book (title, ISBN, editor, front_cover_path) 
VALUES ('Harry Potter and the Sorcerer''s Stone', '9780590353427', 5, 'http://localhost:5282/images/5/5/5/81q77Q39nEL._SL1500_.jpg');
INSERT INTO book (title, ISBN, editor, front_cover_path)
VALUES ('The Hobbit', '9780547928227', 8, 'http://localhost:5282/images/8/6/6/81hylMcxa3L._SL1500_.jpg');
INSERT INTO book (title, ISBN, editor, front_cover_path)
VALUES ('The Lord of the Rings', '9780618640157', 8, 'http://localhost:5282/images/8/6/7/913sMwNHsBL._SY425_.jpg');
INSERT INTO book (title, ISBN, editor, front_cover_path)
VALUES ('The Alchemist', '9780062315007', 7, 'http://localhost:5282/images/7/7/8/71HNLzGQgjL._SL1500_.jpg');
INSERT INTO book (title, ISBN, editor, front_cover_path)
VALUES ('The Da Vinci Code', '9780307474278', 6, 'http://localhost:5282/images/6/8/9/71y4X5150dL._SL1500_.jpg');
INSERT INTO book (title, ISBN, editor, front_cover_path)
VALUES ('Animal Farm', '9780451526342', 3, 'http://localhost:5282/images/3/2/10/71JUJ6pGoIL._SL1500_.jpg');
INSERT INTO book (title, ISBN, editor, front_cover_path)
VALUES ('Sense and Sensibility', '9780141439662', 4, 'http://localhost:5282/images/4/1/11/818mKxj9pAL._SL1500_.jpg');


INSERT INTO author (name, surname) VALUES ('Jane', 'Austen');
INSERT INTO author (name, surname) VALUES ('George', 'Orwell');
INSERT INTO author (name, surname) VALUES ('F. Scott', 'Fitzgerald');
INSERT INTO author (name, surname) VALUES ('Harper', 'Lee');
INSERT INTO author (name, surname) VALUES ('J.K.', 'Rowling');
INSERT INTO author (name, surname) VALUES ('J.R.R.', 'Tolkien');
INSERT INTO author (name, surname) VALUES ('Paulo', 'Coelho');
INSERT INTO author (name, surname) VALUES ('Dan', 'Brown');

INSERT INTO books_authors (book, author) VALUES (1, 3); 
INSERT INTO books_authors (book, author) VALUES (2, 4); 
INSERT INTO books_authors (book, author) VALUES (3, 2); 
INSERT INTO books_authors (book, author) VALUES (4, 1); 
INSERT INTO books_authors (book, author) VALUES (5, 5); 
INSERT INTO books_authors (book, author) VALUES (6, 6);  
INSERT INTO books_authors (book, author) VALUES (7, 6);  
INSERT INTO books_authors (book, author) VALUES (8, 7); 
INSERT INTO books_authors (book, author) VALUES (9, 8);  
INSERT INTO books_authors (book, author) VALUES (10, 2); 
INSERT INTO books_authors (book, author) VALUES (11, 1);


INSERT INTO role (name) VALUES ('user');
INSERT INTO role (name) VALUES ('admin');

INSERT INTO user (name, surname, email, password, role) VALUES ('Piero', 'Piermenti', 'pieropiermenti@gmail.com', '$2a$11$80dY40g/NcmaF2fpuSyxUe/id5.KaE0EItZ1waN6jcmM2eGFi5zSG', 1);
INSERT INTO user (name, surname, email, password, role) VALUES ('Silvia', 'Losinvia', 'losinginvia@pmail.com', '$2a$11$7tmVSv4/BIRzy4NTbKcHLOPZoeeDa/TBrmkwvq2R3CmTaBf83vqeS', 1);
INSERT INTO user (name, surname, email, password, role) VALUES ('Croc',  'Odillo', 'temagno@dmail.com', '$2a$11$uAiYBCKGlNMLm8EV5aVd7eReocvRPUBUHM3bzTOLqzQsSWuRAp2ky', 2);

INSERT INTO reservation (user_id, book_id, reservation_date, due_date) VALUES (1, 1, '2024-12-01', '2024-12-15');
INSERT INTO reservation (user_id, book_id, reservation_date, due_date) VALUES (2, 2, '2024-12-05', '2024-12-20');