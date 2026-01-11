-- Table: Role
CREATE TABLE Role (
    Id INT IDENTITY(1,1) NOT NULL CONSTRAINT Role_pk PRIMARY KEY,
    RoleName NVARCHAR(50) NOT NULL
);

-- Table: Status
CREATE TABLE Status (
    Id INT IDENTITY(1,1) NOT NULL CONSTRAINT Status_pk PRIMARY KEY,
    StatusName NVARCHAR(50) NOT NULL
);

-- Table: [User]
CREATE TABLE "User" (
    Id INT IDENTITY(1,1) NOT NULL CONSTRAINT User_pk PRIMARY KEY,
    Username NVARCHAR(100) NOT NULL UNIQUE,
    Email NVARCHAR(255) NOT NULL UNIQUE,
    PasswordHash NVARCHAR(255) NOT NULL,
    CreatedAt DATETIME2 NOT NULL DEFAULT SYSDATETIME(),
    IsBlocked BIT NOT NULL DEFAULT 0,
    Role INT NOT NULL,
    CONSTRAINT User_Roles FOREIGN KEY (Role)
    REFERENCES Role (Id)
    );

-- Table: Car
CREATE TABLE Car (
     Id INT IDENTITY(1,1) NOT NULL CONSTRAINT Car_pk PRIMARY KEY,
     Price DECIMAL(18,2) NOT NULL,
     Brand NVARCHAR(100) NOT NULL,
     Model NVARCHAR(100) NOT NULL,
     Color NVARCHAR(50),
    [Year] INT NOT NULL,
     VIN NVARCHAR(50) UNIQUE,
     EngineDisplacement DECIMAL(4,2),
     EnginePower INT,
     Mileage INT,
     Seller INT NOT NULL,
     Status INT NOT NULL,
     Description NVARCHAR(MAX),
     CONSTRAINT Car_Status FOREIGN KEY (Status)
         REFERENCES Status (Id),
     CONSTRAINT Car_User FOREIGN KEY (Seller)
         REFERENCES [User] (Id)
);

-- Table: UserFavorites
CREATE TABLE UserFavorites (
       User_Id INT NOT NULL,
       Car_Id INT NOT NULL,
       CreatedAt DATETIME2 NOT NULL DEFAULT SYSDATETIME(),
       PriceChangeNotify BIT NOT NULL DEFAULT 0,
       CONSTRAINT UserFavorites_pk PRIMARY KEY (User_Id, Car_Id),
       CONSTRAINT UserFavorites_User FOREIGN KEY (User_Id)
           REFERENCES [User] (Id),
       CONSTRAINT UserFavorites_Car FOREIGN KEY (Car_Id)
           REFERENCES Car (Id)
);

-- Table: ServiceRecord
CREATE TABLE ServiceRecord (
       Id INT IDENTITY(1,1) NOT NULL CONSTRAINT ServiceRecord_pk PRIMARY KEY,
       MileageAtService INT NOT NULL,
       ServiceDate DATETIME2 NOT NULL DEFAULT SYSDATETIME(),
       Grade DECIMAL(3,2) NOT NULL,
       Car INT NOT NULL,
       CONSTRAINT ServiceRecord_Car FOREIGN KEY (Car)
           REFERENCES Car (Id)
);
