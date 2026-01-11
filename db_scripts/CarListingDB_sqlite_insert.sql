INSERT INTO User (Username, Email, PasswordHash, IsBlocked, Role) VALUES
-- Regular users
('user_john', 'john@site.com', 'HASHED_PASSWORD', 0, 1),
('user_jane', 'jane@site.com', 'HASHED_PASSWORD', 0, 1),
('user_blocked', 'blocked@site.com', 'HASHED_PASSWORD', 1, 1),

-- Dealers
('dealer_mike', 'mike@dealer.com', 'HASHED_PASSWORD', 0, 3),
('dealer_blocked', 'blocked_dealer@dealer.com', 'HASHED_PASSWORD', 1, 3),

-- Admins
('admin_root', 'admin@site.com', 'HASHED_PASSWORD', 0, 2),
('admin_blocked', 'blocked_admin@site.com', 'HASHED_PASSWORD', 1, 2);

INSERT INTO Car
(Price, Brand, Model, Color, Year, VIN, EngineDisplacement, EnginePower, Mileage, Seller, Status, Description)
VALUES
-- User-owned cars
(12000, 'Toyota', 'Corolla', 'White', 2018, 'VIN001', 1.6, 132, 85000, 1, 1, 'Reliable daily car'),
(9000, 'Honda', 'Civic', 'Black', 2016, 'VIN002', 1.8, 140, 110000, 2, 1, 'Well maintained'),

-- Dealer cars
(25000, 'BMW', 'X3', 'Gray', 2020, 'VIN003', 2.0, 248, 40000, 4, 1, 'Dealer vehicle'),
(32000, 'Audi', 'A6', 'Blue', 2021, 'VIN004', 2.0, 261, 30000, 4, 1, 'Premium sedan'),

-- Sold car
(7000, 'Ford', 'Focus', 'Red', 2015, 'VIN005', 1.6, 120, 130000, 1, 2, 'Already sold');

INSERT INTO UserFavorites (User_Id, Car_Id, PriceChangeNotify) VALUES
-- John favorites dealer car
(1, 3, 1),

-- Jane favorites user car
(2, 1, 0),

-- Dealer favorites another dealer car (yes, allowed)
(4, 4, 1),

-- Admin favorites a car
(6, 2, 0);

INSERT INTO ServiceRecord
(Id, MileageAtService, ServiceDate, Grade, Car)
VALUES
-- Toyota Corolla (Car Id = 1)
(1, 30000, '2020-05-10', 4.5, 1),
(2, 60000, '2022-06-18', 4.7, 1),

-- Honda Civic (Car Id = 2)
(3, 50000, '2019-04-02', 4.2, 2),
(4, 90000, '2022-09-11', 4.0, 2),

-- BMW X3 (Car Id = 3)
(5, 20000, '2021-03-22', 4.8, 3),

-- Audi A6 (Car Id = 4)
(6, 15000, '2022-01-15', 4.9, 4),
(7, 28000, '2023-07-09', 4.8, 4);

-- Ford Focus (Car Id = 5) intentionally has NO service records