CREATE TABLE Countries
(
	CountryID INT PRIMARY KEY IDENTITY(1,1),
	CountryName NVARCHAR(MAX)
)

CREATE TABLE People
(
	PersonID INT PRIMARY KEY IDENTITY(1,1),
	FullName NVARCHAR(255),
	PhoneNumber NVARCHAR(20),
	Email NVARCHAR(100),
	NationalID NVARCHAR(50),
	City NVARCHAR(255),
	Gender BIT,
	Password NVARCHAR(MAX),

	CountryID INT,
	FOREIGN KEY (CountryID)
	REFERENCES Countries(CountryID),


)

CREATE TABLE LicenseClasses
(
	LicensesClassID INT PRIMARY KEY IDENTITY(1,1),
	DefaultFees DECIMAL,
	ClassName NVARCHAR(MAX),
)

CREATE TABLE OrderDetails
(
	OrderID INT PRIMARY KEY IDENTITY(1,1),

	PersonID INT
	FOREIGN KEY (PersonID)
	REFERENCES People(PersonID),

	LicensesClassID INT
	FOREIGN KEY (LicensesClassID)
	REFERENCES LicenseClasses(LicensesClassID),

	AdminID TINYINT,
	PassedTests TINYINT,
	Status TINYINT
)

SELECT * FROM People;
SELECT * FROM OrderDetails

INSERT INTO OrderDetails (PersonID, LicensesClassID, AdminID, PassedTests, Status)
VALUES (13, 1, 1, 0, 1);

DELETE FROM OrderDetails
WHERE PersonID = 13;

