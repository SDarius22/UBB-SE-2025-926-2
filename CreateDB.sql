-- run one by one

DROP DATABASE HospitalManagement;
--create db
CREATE DATABASE HospitalManagement;

-- for creating tables
USE HospitalManagement;

CREATE TABLE Users (
	UserID INT PRIMARY KEY IDENTITY(1, 1),
	Username VARCHAR(32) NOT NULL,
	Mail VARCHAR(255) NOT NULL,
	Password VARCHAR(255) NOT NULL,
	Role VARCHAR(255),	-- Patient, Doctor, Admin
	Name VARCHAR(255) NOT NULL,
	Birthdate DATE NOT NULL,
	Cnp VARCHAR(13) NOT NULL,
	Address VARCHAR(255) NOT NULL,
	PhoneNumber VARCHAR(10) NOT NULL,
	RegistrationDate DATETIME NOT NULL,
);

CREATE TABLE Admins (
	AdminID INT PRIMARY KEY IDENTITY(1, 1),
	UserID INT FOREIGN KEY REFERENCES Users(UserID) ON DELETE CASCADE,
);

CREATE TABLE Departments (
	DepartmentID INT PRIMARY KEY IDENTITY(1, 1),
	Name VARCHAR(255) NOT NULL,
);

CREATE TABLE Doctors (
	DoctorID INT PRIMARY KEY IDENTITY(1, 1),
	UserId INT FOREIGN KEY REFERENCES Users(UserID) ON DELETE CASCADE UNIQUE,
	Experience FLOAT,
	Rating FLOAT,
	DepartmentId INT FOREIGN KEY REFERENCES Departments(DepartmentID) ON DELETE CASCADE,
	LicenseNumber VARCHAR(255) NOT NULL,
);

CREATE TABLE Drugs (
	DrugID INT PRIMARY KEY IDENTITY(1, 1),
	Name VARCHAR(255) NOT NULL,
	Administration VARCHAR(255) NOT NULL,
	Supply INT,
	Specification VARCHAR(255) NOT NULL,
);

CREATE TABLE Equipments (
	EquipmentID INT PRIMARY KEY IDENTITY(1, 1) NOT NULL,
	Name VARCHAR(255) NOT NULL,
	Specification VARCHAR(255) NOT NULL,
	Type VARCHAR(255) NOT NULL,
	Stock INT,
);

CREATE TABLE Patients (
    UserId INT NOT NULL, -- Foreign key reference to Users table
    PatientId INT IDENTITY(1,1) PRIMARY KEY, -- Auto-increment primary key
    BloodType NVARCHAR(3) NOT NULL CHECK (BloodType IN ('A+', 'A-', 'B+', 'B-', 'AB+', 'AB-', 'O+', 'O-')), -- Enum-like constraint
    EmergencyContact NVARCHAR(20) NOT NULL, -- Phone number for emergency contact
    Allergies NVARCHAR(255) NULL, -- Can be NULL if no allergies
    Weight FLOAT NOT NULL CHECK (Weight > 0), -- Prevent invalid weight values
    Height INT NOT NULL CHECK (Height > 0), -- Height in cm, must be positive

    CONSTRAINT FK_Patients_Users FOREIGN KEY (UserId) REFERENCES Users(UserId) ON DELETE CASCADE
);

CREATE TABLE Rooms (
	RoomID INT PRIMARY KEY IDENTITY(1, 1),
	Capacity INT,
	DepartmentID INT FOREIGN KEY REFERENCES Departments(DepartmentID) ON DELETE CASCADE,
	EquipmentID INT FOREIGN KEY REFERENCES Equipments(EquipmentID) ON DELETE SET NULL, -- equipment is optional
);

CREATE TABLE Shifts (
	ShiftID INT PRIMARY KEY IDENTITY(1, 1),
	Date DATE NOT NULL,
	StartTime TIME NOT NULL,
	EndTime TIME NOT NULL,
);

CREATE TABLE Schedules (
	ScheduleID INT PRIMARY KEY IDENTITY(1, 1),
	DoctorID INT FOREIGN KEY REFERENCES Doctors(DoctorID) ON DELETE CASCADE,
	ShiftID INT FOREIGN KEY REFERENCES Shifts(ShiftID) ON DELETE CASCADE,
);

CREATE TABLE Reviews (
	ReviewID INT PRIMARY KEY IDENTITY (1, 1),
	NrStars INT NOT NULL,
	Text VARCHAR(MAX),
	MedicalRecordID INT,
);


CREATE TABLE Procedures (
    ProcedureId INT IDENTITY(1,1) PRIMARY KEY,
    ProcedureName VARCHAR(100) NOT NULL,
    ProcedureDuration TIME NOT NULL,
    DepartmentId INT NOT NULL,
    CONSTRAINT FK_Procedures_Departments
        FOREIGN KEY (DepartmentId) REFERENCES Departments(DepartmentId)
);


CREATE TABLE Appointments (
    AppointmentId INT IDENTITY(1,1) PRIMARY KEY,
    DoctorId INT NOT NULL,
    PatientId INT NOT NULL,
    DateAndTime DATETIME2 NOT NULL,
    Finished BIT NOT NULL,
    ProcedureId INT NOT NULL,
    CONSTRAINT FK_Appointments_Doctors
        FOREIGN KEY (DoctorId) REFERENCES Doctors(DoctorId),
    CONSTRAINT FK_Appointments_Patients
        FOREIGN KEY (PatientId) REFERENCES Patients(PatientId),
    CONSTRAINT FK_Appointments_Procedures
        FOREIGN KEY (ProcedureId) REFERENCES Procedures(ProcedureId)
);

CREATE TABLE MedicalRecords (
    MedicalRecordId INT IDENTITY(1,1) PRIMARY KEY,
    PatientId INT NOT NULL,
    DoctorId INT NOT NULL,
    DateAndTime DATETIME2 NOT NULL,
    ProcedureId INT NOT NULL,
    Conclusion NVARCHAR(255) NULL,
    CONSTRAINT FK_MedicalRecords_Patients
        FOREIGN KEY (PatientId) REFERENCES Patients(PatientId),
    CONSTRAINT FK_MedicalRecords_Doctors
        FOREIGN KEY (DoctorId) REFERENCES Doctors(DoctorId),
    CONSTRAINT FK_MedicalRecords_Procedures
        FOREIGN KEY (ProcedureId) REFERENCES Procedures(ProcedureId)
);

CREATE TABLE Documents (
    DocumentId INT IDENTITY(1,1) PRIMARY KEY,
	MedicalRecordId INT NOT NULL,
	CONSTRAINT FK_Documents_MedicalRecords
		FOREIGN KEY (MedicalRecordId) REFERENCES MedicalRecords(MedicalRecordID),
    Files VARCHAR(100),
);


--adding constraints

-- USERS
IF EXISTS (SELECT * FROM sys.check_constraints WHERE name = 'CK_Role')
BEGIN
    ALTER TABLE Users DROP CONSTRAINT CK_Role;
END
ALTER TABLE Users
ADD CONSTRAINT CK_Role CHECK (Role IN ('Patient', 'Doctor', 'Admin')); --role can be only Patient, Doctor or Admin

IF EXISTS (SELECT * FROM sys.check_constraints WHERE name = 'CK_PhoneNumber')
BEGIN
    ALTER TABLE Users DROP CONSTRAINT CK_PhoneNumber;
END
ALTER TABLE Users
ADD CONSTRAINT CK_PhoneNumber CHECK (PhoneNumber NOT LIKE '%[^0-9]%' AND LEN(PhoneNumber) = 10); --contains only digits

IF EXISTS (SELECT * FROM sys.check_constraints WHERE name = 'CK_Cnp')
BEGIN
    ALTER TABLE Users DROP CONSTRAINT CK_Cnp;
END
ALTER TABLE Users
ADD CONSTRAINT CK_Cnp CHECK (Cnp NOT LIKE '%[^0-9]%' AND LEN(Cnp) = 13); --contains only digits

IF EXISTS (SELECT * FROM sys.check_constraints WHERE name = 'CK_Address')
BEGIN
    ALTER TABLE Users DROP CONSTRAINT CK_Address;
END
ALTER TABLE Users
ADD CONSTRAINT CK_Address CHECK (Address NOT LIKE '%[^a-zA-Z0-9 ,.-]%'); --contains only alphanumeric characters, spaces, commas, period, dash


-- DEPARTMENTS
IF EXISTS (SELECT * FROM sys.check_constraints WHERE name = 'CK_DepartmentName')
BEGIN
    ALTER TABLE Departments DROP CONSTRAINT CK_DepartmentName;
END
ALTER TABLE Departments
ADD CONSTRAINT CK_DepartmentName CHECK (Name NOT LIKE '%[^a-zA-Z0-9 ]%'); --contains only alphanumeric characters


-- DOCTORS
IF EXISTS (SELECT * FROM sys.check_constraints WHERE name = 'CK_LicenseNumber')
BEGIN
    ALTER TABLE Doctors DROP CONSTRAINT CK_LicenseNumber;
END
ALTER TABLE Doctors
ADD CONSTRAINT CK_LicenseNumber CHECK (LicenseNumber NOT LIKE '%[^a-zA-Z0-9 ]%'); --contains only alphanumeric characters

IF EXISTS (SELECT * FROM sys.check_constraints WHERE name = 'CK_Experience')
BEGIN
    ALTER TABLE Doctors DROP CONSTRAINT CK_Experience;
END
ALTER TABLE Doctors
ADD CONSTRAINT CK_Experience CHECK (Experience >= 0); --experience must be positive

IF EXISTS (SELECT * FROM sys.check_constraints WHERE name = 'CK_Rating')
BEGIN
    ALTER TABLE Doctors DROP CONSTRAINT CK_Rating;
END
ALTER TABLE Doctors
ADD CONSTRAINT CK_Rating CHECK (Rating >= 0 AND Rating <= 5); --rating must be between 0 and 5


-- DRUGS
IF EXISTS (SELECT * FROM sys.check_constraints WHERE name = 'CK_Supply')
BEGIN
    ALTER TABLE Drugs DROP CONSTRAINT CK_Supply;
END
ALTER TABLE Drugs
ADD CONSTRAINT CK_Supply CHECK (Supply >= 0); --supply must be positive

IF EXISTS (SELECT * FROM sys.check_constraints WHERE name = 'CK_DrugName')
BEGIN
    ALTER TABLE Drugs DROP CONSTRAINT CK_DrugName;
END
ALTER TABLE Drugs
ADD CONSTRAINT CK_DrugName CHECK (Name NOT LIKE '%[^a-zA-Z0-9 ]%'); --contains only alphanumeric characters


-- EQUIPMENTS
IF EXISTS (SELECT * FROM sys.check_constraints WHERE name = 'CK_Stock')
BEGIN
    ALTER TABLE Equipments DROP CONSTRAINT CK_Stock;
END
ALTER TABLE Equipments
ADD CONSTRAINT CK_Stock CHECK (Stock >= 0); --stock must be positive

IF EXISTS (SELECT * FROM sys.check_constraints WHERE name = 'CK_EquipmentName')
BEGIN
    ALTER TABLE Equipments DROP CONSTRAINT CK_EquipmentName;
END
ALTER TABLE Equipments
ADD CONSTRAINT CK_EquipmentName CHECK (Name NOT LIKE '%[^a-zA-Z0-9 ]%'); --contains only alphanumeric characters

IF EXISTS (SELECT * FROM sys.check_constraints WHERE name = 'CK_Type')
BEGIN
    ALTER TABLE Equipments DROP CONSTRAINT CK_Type;
END
ALTER TABLE Equipments
ADD CONSTRAINT CK_Type CHECK (Type NOT LIKE '%[^a-zA-Z0-9 ]%'); --type can be only alphanumeric characters

IF EXISTS (SELECT * FROM sys.check_constraints WHERE name = 'CK_Specification')
BEGIN
    ALTER TABLE Equipments DROP CONSTRAINT CK_Specification;
END
ALTER TABLE Equipments
ADD CONSTRAINT CK_Specification CHECK (Specification NOT LIKE '%[^a-zA-Z0-9 ,.-]%'); --contains only alphanumeric characters, spaces, commas, period, dash


-- ROOMS
IF EXISTS (SELECT * FROM sys.check_constraints WHERE name = 'CK_Capacity')
BEGIN
	ALTER TABLE Rooms DROP CONSTRAINT CK_Capacity;
END
ALTER TABLE Rooms
ADD CONSTRAINT CK_Capacity CHECK (Capacity > 0); --capacity must be positive


-- Reviews

-- Add check constraint for NrStars
IF EXISTS (SELECT * FROM sys.check_constraints WHERE name = 'CK_NrStars')
BEGIN
	ALTER TABLE Reviews DROP CONSTRAINT CK_NrStars;
END
ALTER TABLE Reviews
ADD CONSTRAINT CK_NrStars CHECK (NrStars >= 1 AND NrStars <= 5);


-- SHIFTS
IF EXISTS (SELECT * FROM sys.check_constraints WHERE name = 'CK_ShiftTime')
BEGIN
    ALTER TABLE Shifts DROP CONSTRAINT CK_ShiftTime;
END
ALTER TABLE Shifts
ADD CONSTRAINT CK_ShiftTime CHECK (
    (DATEPART(HOUR, StartTime) = 8 OR DATEPART(HOUR, StartTime) = 20) AND
    (DATEPART(HOUR, EndTime) = 8 OR DATEPART(HOUR, EndTime) = 20) AND
    (DATEPART(MINUTE, StartTime) = 0) AND
    (DATEPART(MINUTE, EndTime) = 0)
);



-- inserting data

-- run first, before the exec
GO
CREATE OR ALTER PROCEDURE InsertData
    @nrOfRows INT
AS
BEGIN
    SET NOCOUNT ON;

    DECLARE @i INT = 1;

    WHILE @i <= @nrOfRows
    BEGIN
        -- Insert into Users
        INSERT INTO Users (Username, Mail, Password, Role, Name, Birthdate, Cnp, Address, PhoneNumber, RegistrationDate)
        VALUES (
            CONCAT('user', @i),
            CONCAT('user', @i, '@example.com'),
            'password',
            CASE WHEN @i % 3 = 0 THEN 'Admin' WHEN @i % 2 = 0 THEN 'Doctor' ELSE 'Patient' END,
            CONCAT('User ', @i),
            DATEADD(YEAR, -20, GETDATE()),
            RIGHT(CONCAT('6040322012025', @i), 13),
            CONCAT('Address Nr. ', @i),
            RIGHT(CONCAT('0765432189', @i), 10),
            GETDATE()
        );

        -- Insert into Departments
        INSERT INTO Departments (Name)
        VALUES (CONCAT('Department ', @i));

        -- Insert into Doctors
        INSERT INTO Doctors (UserId, Experience, Rating, DepartmentId, LicenseNumber)
        VALUES (
            @i,
            RAND() * 10,
            RAND() * 5,
            @i,
            CONCAT('License', @i)
        );

        -- Insert into Drugs
        INSERT INTO Drugs (Name, Administration, Supply, Specification)
        VALUES (
            CONCAT('Drug ', @i),
            'Pill',
            @i * 10,
            CONCAT('Specification ', @i)
        );

        -- Insert into Equipments
        INSERT INTO Equipments (Name, Specification, Type, Stock)
        VALUES (
            CONCAT('Equipment ', @i),
            CONCAT('Specification ', @i),
            'Type A',
            @i * 5
        );

        -- Insert into Rooms
        INSERT INTO Rooms (Capacity, DepartmentID, EquipmentID)
        VALUES (
            @i * 2,
            @i,
            @i
        );

        -- Insert into Shifts
        INSERT INTO Shifts (Date, StartTime, EndTime)
        VALUES (
            GETDATE(),
            CASE WHEN @i % 3 = 0 THEN '08:00:00' WHEN @i % 3 = 1 THEN '20:00:00' ELSE '08:00:00' END,
            CASE WHEN @i % 3 = 0 THEN '20:00:00' WHEN @i % 3 = 1 THEN '08:00:00' ELSE '08:00:00' END
        );

        -- Insert into Schedules
        INSERT INTO Schedules (DoctorID, ShiftID)
        VALUES (
            @i,
            @i
        );

        SET @i = @i + 1;
    END
END;
GO

exec InsertData @nrOfRows = 10;


GO
CREATE OR ALTER PROCEDURE InsertMoreUsers
    @nrOfRows INT
AS
BEGIN
    SET NOCOUNT ON;

    DECLARE @i INT = 1;

    WHILE @i <= @nrOfRows
    BEGIN
        -- Insert into Users
        INSERT INTO Users (Username, Mail, Password, Role, Name, Birthdate, Cnp, Address, PhoneNumber, RegistrationDate)
        VALUES (
            CONCAT('user', @i),
            CONCAT('user', @i, '@example.com'),
            'password',
            CASE WHEN @i % 3 = 0 THEN 'Admin' WHEN @i % 2 = 0 THEN 'Doctor' ELSE 'Patient' END,
            CONCAT('User ', @i),
            DATEADD(YEAR, -20, GETDATE()),
            RIGHT(CONCAT('6040322012025', @i), 13),
            CONCAT('Address Nr. ', @i),
            RIGHT(CONCAT('0765432189', @i), 10),
            GETDATE()
        );

        SET @i = @i + 1;
    END
END;
GO

EXEC InsertMoreUsers @nrOfRows = 10;

INSERT INTO Patients (UserId, BloodType, EmergencyContact, Allergies, Weight, Height)
VALUES 
(5, 'A+', '111-222-3333', 'Peanuts', 60.5, 165),
(6, 'O-', '222-333-4444', 'None', 80.0, 175),
(7, 'B+', '333-444-5555', 'Pollen', 70.2, 170);


INSERT INTO Procedures (ProcedureName, ProcedureDuration, DepartmentId)
VALUES
    ('ECG', '00:30:00', 1),
    ('Brain MRI', '01:00:00', 2), 
    ('Child Checkup', '00:20:00', 3),
    ('Stress Test', '00:45:00', 1);

INSERT INTO Appointments (PatientId, DoctorId, DateAndTime, ProcedureId, Finished)
VALUES
    (1, 1, '2025-03-23T17:00:00', 1, 0),
    (1, 2, '2025-03-23T13:00:00', 4, 1),
    (2, 3, '2025-03-23T09:30:00', 2, 0),
    (2, 4, '2025-03-23T10:00:00', 3, 0),
    (3, 1, '2025-03-23T18:00:00', 1, 1),
    (3, 2, '2025-03-23T15:00:00', 4, 0);

INSERT INTO MedicalRecords (PatientId, DoctorId, DateAndTime, ProcedureId, Conclusion)
VALUES
    (1, 1, '2023-03-17T12:30:00', 1, 'Normal ECG results'),
    (1, 2, '2023-03-17T13:45:00', 4, 'Stress test results pending'),
    (2, 3, '2023-03-18T10:30:00', 2, 'Brain MRI results normal'),
    (2, 4, '2023-03-19T11:00:00', 3, 'Child checkup results normal'),
    (3, 1, '2023-03-19T15:30:00', 1, 'ECG results pending'),
    (3, 2, '2023-03-20T16:00:00', 4, 'Stress test results normal'),
    (1, 1, '2023-03-17T12:30:00', 1, 'Normal ECG results'),
    (1, 2, '2023-03-17T13:45:00', 4, 'Stress test results pending'),
    (1, 3, '2023-03-18T10:30:00', 2, 'Brain MRI results normal'),
    (1, 4, '2023-03-19T11:00:00', 3, 'Child checkup results normal'),
    (1, 1, '2023-03-19T15:30:00', 1, 'ECG results pending'), 
    (1, 2, '2023-03-20T16:00:00', 4, 'Stress test results normal');

