CREATE TABLE WEXO_DEPOT (
    ID INT IDENTITY(1,1) PRIMARY KEY,  -- Use IDENTITY for auto-incrementing ID
    Navn VARCHAR(100) NOT NULL,
    Antal INT NOT NULL,
    Lokation VARCHAR(100) NOT NULL
);

INSERT INTO WEXO_DEPOT (Navn, Antal, Lokation) VALUES
('HDMI-kabel', 25, 'Kasse #1'),
('DP-kabel', 15, 'Kasse #2'),
('Tr�dl�s mus', 10, 'Kasse #3'),
('Musem�tte, stor', 10, 'Kasse #4'),
('Musem�tte, lille', 10, 'Kasse #5'),
('Sk�rm, curved', 5, 'Kasse #6'),
('Sk�rm', 10, 'Kasse #7'),
('Str�mforsyning', 20, 'Kasse #8'),
('Str�mforsyning, Mac', 20, 'Kasse #9'),
('Netv�rkskabel', 50, 'Kasse #10'),
('USB-C stik', 15, 'Kasse #11');