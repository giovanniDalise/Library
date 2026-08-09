-- ============================================================
-- DB: library_identity  (IdentityService)
-- Container dedicato: mysql-identity
-- ============================================================

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

INSERT INTO role (name) VALUES ('user');
INSERT INTO role (name) VALUES ('admin');

INSERT INTO user (name, surname, email, password, role)
VALUES ('Piero', 'Piermenti', 'pieropiermenti@gmail.com', '$2a$11$80dY40g/NcmaF2fpuSyxUe/id5.KaE0EItZ1waN6jcmM2eGFi5zSG', 1);
INSERT INTO user (name, surname, email, password, role)
VALUES ('Silvia', 'Losinvia', 'losinginvia@pmail.com', '$2a$11$7tmVSv4/BIRzy4NTbKcHLOPZoeeDa/TBrmkwvq2R3CmTaBf83vqeS', 1);
INSERT INTO user (name, surname, email, password, role)
VALUES ('Croc', 'Odillo', 'temagno@dmail.com', '$2a$11$uAiYBCKGlNMLm8EV5aVd7eReocvRPUBUHM3bzTOLqzQsSWuRAp2ky', 2);
