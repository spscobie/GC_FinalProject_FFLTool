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

DROP TABLE tblUserWatchlists;
DELETE FROM tblUserWatchlists;
DROP TABLE tblWatchlists;

SELECT *
FROM tblUsers;

SELECT *
FROM tblTeams;

CREATE TABLE tblUserWatchlists (
	WatchlistId BIGINT IDENTITY(1000000, 1) NOT NULL,
	UserId NVARCHAR(128) NOT NULL,
	CONSTRAINT PK_UserIdWatchlistId PRIMARY KEY (UserId, WatchlistId),
	FOREIGN KEY (UserId) REFERENCES AspNetUsers(Id),
	CONSTRAINT UK_WatchlistId UNIQUE (WatchlistId));

CREATE TABLE tblWatchlists (
	WatchlistId BIGINT NOT NULL,
	WatchlistName VARCHAR(100) NOT NULL,
	PlayerId INT NOT NULL,
	FOREIGN KEY (WatchlistId) REFERENCES tblUserWatchlists (WatchlistId),
	CONSTRAINT PK_WatchlistPLayerID PRIMARY KEY (WatchlistId, PlayerId));

SELECT *
FROM tblUserWatchlists;

SELECT *
FROM tblWatchlists;

SELECT *
FROM AspNetUsers;

UPDATE a
SET a.WatchlistName = 'QB Test List'
FROM tblWatchlists a
WHERE a.WatchlistId = 1000047;

UPDATE a
SET a.WatchlistName = 'The Winningest List'
FROM tblWatchlists a
WHERE a.WatchlistId = 1000051;

DELETE FROM tblWatchlists
WHERE WatchlistId = 1000047 AND PlayerId NOT IN (5933, 7274);

INSERT INTO tblWatchlists
VALUES (1000047, 'QB Test List', 6825),
	   (1000047, 'QB Test List', 7274)

INSERT INTO tblWatchlists
VALUES (1000047, 'QB Test List', 9712)