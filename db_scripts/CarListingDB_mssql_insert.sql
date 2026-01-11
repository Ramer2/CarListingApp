-- Statuses
INSERT INTO Status (StatusName)
    VALUES ('Active'), ('Sold');

-- Roles
INSERT INTO Role (RoleName)
    VALUES ('User'), ('Admin'), ('Dealer');

-- Users
INSERT INTO "User" (Username, Email, PasswordHash, IsBlocked, Role)
VALUES
    ('user_john', 'john@site.com', 'HASHED_PASSWORD', 0, 1),
    ('user_jane', 'jane@site.com', 'HASHED_PASSWORD', 0, 1),
    ('user_blocked', 'blocked@site.com', 'HASHED_PASSWORD', 1, 1),
    ('dealer_mike', 'mike@dealer.com', 'HASHED_PASSWORD', 0, 3),
    ('dealer_blocked', 'blocked_dealer@dealer.com', 'HASHED_PASSWORD', 1, 3),
    ('admin_root', 'admin@site.com', 'HASHED_PASSWORD', 0, 2),
    ('admin_blocked', 'blocked_admin@site.com', 'HASHED_PASSWORD', 1, 2);

-- Cars
INSERT INTO Car
(Price, Brand, Model, Color, [Year], VIN, EngineDisplacement, EnginePower, Mileage, Seller, Status, Description)
VALUES
    (12000, 'Toyota', 'Corolla', 'White', 2018, 'VIN001', 1.60, 132, 85000, 1, 1, 'Reliable daily car'),
    (9000, 'Honda', 'Civic', 'Black', 2016, 'VIN002', 1.80, 140, 110000, 2, 1, 'Well maintained'),
    (25000, 'BMW', 'X3', 'Gray', 2020, 'VIN003', 2.00, 248, 40000, 4, 1, 'Dealer vehicle'),
    (32000, 'Audi', 'A6', 'Blue', 2021, 'VIN004', 2.00, 261, 30000, 4, 1, 'Premium sedan'),
    (7000, 'Ford', 'Focus', 'Red', 2015, 'VIN005', 1.60, 120, 130000, 1, 2, 'Already sold');

-- User Favorites
INSERT INTO UserFavorites (User_Id, Car_Id, PriceChangeNotify)
VALUES
    (1, 3, 1),
    (2, 1, 0),
    (4, 4, 1),
    (6, 2, 0);

-- Service Records
INSERT INTO ServiceRecord (MileageAtService, ServiceDate, Grade, Car)
VALUES
    (30000, '2020-05-10', 4.50, 1),
    (60000, '2022-06-18', 4.70, 1),
    (50000, '2019-04-02', 4.20, 2),
    (90000, '2022-09-11', 4.00, 2),
    (20000, '2021-03-22', 4.80, 3),
    (15000, '2022-01-15', 4.90, 4),
    (28000, '2023-07-09', 4.80, 4);