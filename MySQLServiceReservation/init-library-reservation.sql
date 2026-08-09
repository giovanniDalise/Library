-- ============================================================
-- DB: library_reservation  (ReservationService)
-- Container dedicato: mysql-reservation
--
-- Nessuna FK verso user/book: sono in database/container diversi.
-- La validazione di esistenza di user_id/book_id avviene a livello
-- applicativo (API sincrone verso IdentityService/BookService oppure
-- eventi RabbitMQ), non a livello di schema.
-- ============================================================

CREATE TABLE reservation (
    reservation_id BIGINT AUTO_INCREMENT PRIMARY KEY,
    user_id BIGINT NOT NULL,
    book_id BIGINT NOT NULL,
    reservation_date DATE NOT NULL,
    due_date DATE NOT NULL
);

INSERT INTO reservation (user_id, book_id, reservation_date, due_date)
VALUES (1, 1, '2024-12-01', '2024-12-15');
INSERT INTO reservation (user_id, book_id, reservation_date, due_date)
VALUES (2, 2, '2024-12-05', '2024-12-20');
