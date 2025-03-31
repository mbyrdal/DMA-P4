CREATE TABLE WEXO_DEPOT (
    ID INT IDENTITY(1,1) PRIMARY KEY,  -- Use IDENTITY for auto-incrementing ID
    Navn VARCHAR(100) NOT NULL,
    Antal INT NOT NULL,
    Lokation VARCHAR(100) NOT NULL
);

INSERT INTO WEXO_DEPOT (Navn, Antal, Lokation) VALUES
('HDMI-kabel', 25, 'Kasse #1'),
('DP-kabel', 15, 'Kasse #2'),
('Trådløs mus', 10, 'Kasse #3'),
('Musemåtte, stor', 10, 'Kasse #4'),
('Musemåtte, lille', 10, 'Kasse #5'),
('Skærm, curved', 5, 'Kasse #6'),
('Skærm', 10, 'Kasse #7'),
('Strømforsyning', 20, 'Kasse #8'),
('Strømforsyning, Mac', 20, 'Kasse #9'),
('Netværkskabel', 50, 'Kasse #10'),
('USB-C stik', 15, 'Kasse #11');