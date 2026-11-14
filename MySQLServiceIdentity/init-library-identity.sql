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
    confirmation_token VARCHAR(255) NULL,
    token_expires_at DATETIME NULL,
    is_confirmed BOOLEAN NOT NULL DEFAULT FALSE,
    reset_token VARCHAR(255) NULL,
    reset_token_expires_at DATETIME NULL,
	phone_number VARCHAR(20) NULL,
    profile_image_url VARCHAR(500) NULL,
    created_at DATETIME NOT NULL DEFAULT CURRENT_TIMESTAMP,
    FOREIGN KEY (role) REFERENCES role(role_id)
);

INSERT INTO role (name) VALUES ('user');
INSERT INTO role (name) VALUES ('admin');

INSERT INTO user (name, surname, email, password, role, is_confirmed, profile_image_url)
VALUES ('Piero', 'Piermenti', 'pieropiermenti@gmail.com', '$2a$11$80dY40g/NcmaF2fpuSyxUe/id5.KaE0EItZ1waN6jcmM2eGFi5zSG', 1, TRUE, 'profile-images/1/3f2a9c1d7b4e4a8f9c0d5e6b7a1c2d3e.jpg');
INSERT INTO user (name, surname, email, password, role, is_confirmed, profile_image_url)
VALUES ('Silvia', 'Losinvia', 'losinginvia@pmail.com', '$2a$11$7tmVSv4/BIRzy4NTbKcHLOPZoeeDa/TBrmkwvq2R3CmTaBf83vqeS', 1, TRUE, 'profile-images/2/8b1e4d6a2c9f4f3a8d7e0b5c6a9d1f2e.jpg');
INSERT INTO user (name, surname, email, password, role, is_confirmed, profile_image_url)
VALUES ('Croc', 'Odillo', 'temagno@dmail.com', '$2a$11$uAiYBCKGlNMLm8EV5aVd7eReocvRPUBUHM3bzTOLqzQsSWuRAp2ky', 2, TRUE, 'profile-images/3/c4d7a0e1b2f84a6c9e3d5f7b8a0c1e2d.jpeg);