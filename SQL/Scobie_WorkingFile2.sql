DROP TABLE tblUsers;

USE FFLTool;
CREATE TABLE tblUsers (
	UserId INT IDENTITY(100000, 1) NOT NULL,
	FirstName VARCHAR(100) NOT NULL,
	LastName VARCHAR(100) NOT NULL,
	EmailAddress VARCHAR (100) NOT NULL,
	Pw VARCHAR(100) NOT NULL,
	Administrator BIT NOT NULL,
	CONSTRAINT PK_UserId PRIMARY KEY (UserId));

INSERT INTO tblUsers
VALUES ('Jacob', 'Snover', 'mruser1@aol.com', 'admin1234', 1),
	   ('Jason', 'Kubo', 'mruser2@aol.com', 'admin1234', 1),
	   ('Beni', 'Rajanish', 'mrsuser1@aol.com', 'admin1234', 1),
	   ('Stephen', 'Scobie', 'mruser3@aol.com', 'admin1234', 1)

SELECT *
FROM tblUsers;

DROP tblTeams;

CREATE TABLE tblTeams (
	TeamId BIGINT NOT NULL,
	UserId INT NOT NULL,
	TeamName VARCHAR(256) NOT NULL,
	PlayerId INT NOT NULL,
	CONSTRAINT PK_TeamId PRIMARY KEY (TeamId));

INSERT INTO tblTeams
VALUES (RAND() * 10000000000, 100000, 'Team 1', 5000),
	   (RAND() * 10000000000, 100000, 'Team 2', 5001),
	   (RAND() * 10000000000, 100000, 'Team 3', 5002),
	   (RAND() * 10000000000, 100001, 'Team A', 6000),
	   (RAND() * 10000000000, 100001, 'Team B', 7000),
	   (RAND() * 10000000000, 100002, 'Team Hers', 15000),
	   (RAND() * 10000000000, 100002, 'Team His', 17000),
	   (RAND() * 10000000000, 100003, 'Team Alpha', 5000),
	   (RAND() * 10000000000, 100003, 'Team Bravo', 5001),
	   (RAND() * 10000000000, 100003, 'Team Charlie', 5002),
	   (RAND() * 10000000000, 100003, 'Team Delta', 5000);

SELECT *
FROM tblTeams
ORDER BY UserId, TeamName