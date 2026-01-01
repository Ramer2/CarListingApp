-- Created by Redgate Data Modeler (https://datamodeler.redgate-platform.com)
-- Last modification date: 2026-01-01 16:18:48.442

-- tables
-- Table: Car
CREATE TABLE Car (
                     Id integer NOT NULL CONSTRAINT Car_pk PRIMARY KEY,
                     Price real NOT NULL,
                     Brand text NOT NULL,
                     Model text NOT NULL,
                     Color text,
                     Year integer NOT NULL,
                     VIN text UNIQUE,
                     EngineDisplacement real,
                     EnginePower real,
                     Mileage integer,
                     Seller integer NOT NULL,
                     Status integer NOT NULL,
                     Description text,
                     CONSTRAINT Car_Status FOREIGN KEY (Status)
                         REFERENCES Status (Id),
                     CONSTRAINT Car_User FOREIGN KEY (Seller)
                         REFERENCES User (Id)
);

-- Table: Role
CREATE TABLE Role (
                      Id integer NOT NULL CONSTRAINT Role_pk PRIMARY KEY,
                      RoleName text NOT NULL
);

-- Table: Status
CREATE TABLE Status (
                        Id integer NOT NULL CONSTRAINT Status_pk PRIMARY KEY,
                        StatusName text NOT NULL
);

-- Table: User
CREATE TABLE User (
                      Id integer NOT NULL CONSTRAINT User_pk PRIMARY KEY,
                      Username text NOT NULL UNIQUE,
                      Email text NOT NULL UNIQUE,
                      PasswordHash text NOT NULL,
                      CreatedAt datetime NOT NULL DEFAULT CURRENT_TIMESTAMP,
                      IsBlocked integer NOT NULL DEFAULT 0,
                      Role integer NOT NULL,
                      CONSTRAINT User_Roles FOREIGN KEY (Role)
                          REFERENCES Role (Id)
);

-- Table: UserFavorites
CREATE TABLE UserFavorites (
                               User_Id integer NOT NULL,
                               Car_Id integer NOT NULL,
                               CreatedAt datetime NOT NULL DEFAULT CURRENT_TIMESTAMP,
                               PriceChangeNotify integer NOT NULL DEFAULT 0,
                               CONSTRAINT UserFavorites_pk PRIMARY KEY (User_Id,Car_Id),
                               CONSTRAINT UserFavorites_User FOREIGN KEY (User_Id)
                                   REFERENCES User (Id),
                               CONSTRAINT UserFavorites_Car FOREIGN KEY (Car_Id)
                                   REFERENCES Car (Id)
);

INSERT INTO Status (StatusName) VALUES ('Active'), ('Sold');

INSERT INTO Role (RoleName) VALUES ('User'), ('Admin'), ('Dealer');

-- End of file.

