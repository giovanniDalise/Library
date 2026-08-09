-- ============================================================
-- DB: library  (BookService)
-- Container dedicato: mysql-books
-- ============================================================

CREATE TABLE editor (
    editor_id BIGINT NOT NULL AUTO_INCREMENT,
    name VARCHAR(255) NOT NULL,
    PRIMARY KEY (editor_id)
);

CREATE TABLE author (
    author_id BIGINT NOT NULL AUTO_INCREMENT,
    name VARCHAR(255) NOT NULL,
    surname VARCHAR(255) NOT NULL,
    PRIMARY KEY (author_id)
);

CREATE TABLE book (
    book_id BIGINT NOT NULL AUTO_INCREMENT,
    title VARCHAR(255) NOT NULL,
    isbn VARCHAR(255) NOT NULL,
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

INSERT INTO editor (name) VALUES ('John Smith');
INSERT INTO editor (name) VALUES ('Emily Johnson');
INSERT INTO editor (name) VALUES ('Michael Brown');
INSERT INTO editor (name) VALUES ('Sophia Lee');
INSERT INTO editor (name) VALUES ('David Wilson');
INSERT INTO editor (name) VALUES ('Penguin Random House');
INSERT INTO editor (name) VALUES ('HarperCollins');
INSERT INTO editor (name) VALUES ('Bloomsbury Publishing');

INSERT INTO book (title, ISBN, editor, front_cover_path)
VALUES ('The Great Gatsby', '9780743273565', 1, 'http://localhost:5282/images/1/917TF-0LAwL._SL1500_.jpg');
INSERT INTO book (title, ISBN, editor, front_cover_path)
VALUES ('To Kill a Mockingbird', '9780061120084', 2, 'http://localhost:5282/images/2/9788807892790_0_0_536_0_75.jpg');
INSERT INTO book (title, ISBN, editor, front_cover_path)
VALUES ('1984', '9780451524935', 3, 'http://localhost:5282/images/3/71uGFEVcEkL._SL1500_.jpg');
INSERT INTO book (title, ISBN, editor, front_cover_path)
VALUES ('Pride and Prejudice', '9780141439518', 4, 'http://localhost:5282/images/4/71o0INZqSsL._SL1422_.jpg');
INSERT INTO book (title, ISBN, editor, front_cover_path)
VALUES ('Harry Potter and the Sorcerer''s Stone', '9780590353427', 5, 'http://localhost:5282/images/5/81q77Q39nEL._SL1500_.jpg');
INSERT INTO book (title, ISBN, editor, front_cover_path)
VALUES ('The Hobbit', '9780547928227', 8, 'http://localhost:5282/images/6/81hylMcxa3L._SL1500_.jpg');
INSERT INTO book (title, ISBN, editor, front_cover_path)
VALUES ('The Lord of the Rings', '9780618640157', 8, 'http://localhost:5282/images/7/913sMwNHsBL._SY425_.jpg');
INSERT INTO book (title, ISBN, editor, front_cover_path)
VALUES ('The Alchemist', '9780062315007', 7, 'http://localhost:5282/images/8/71HNLzGQgjL._SL1500_.jpg');
INSERT INTO book (title, ISBN, editor, front_cover_path)
VALUES ('The Da Vinci Code', '9780307474278', 6, 'http://localhost:5282/images/9/71y4X5150dL._SL1500_.jpg');
INSERT INTO book (title, ISBN, editor, front_cover_path)
VALUES ('Animal Farm', '9780451526342', 3, 'http://localhost:5282/images/10/71JUJ6pGoIL._SL1500_.jpg');
INSERT INTO book (title, ISBN, editor, front_cover_path)
VALUES ('Sense and Sensibility', '9780141439662', 4, 'http://localhost:5282/images/11/818mKxj9pAL._SL1500_.jpg');

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
