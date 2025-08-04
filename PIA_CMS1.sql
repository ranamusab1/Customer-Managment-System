-- Create Database
CREATE DATABASE PIA_CMS;
GO

USE PIA_CMS;
GO

-- Admins Table (Updated with Name column)
CREATE TABLE Admins (
    AdminID INT IDENTITY PRIMARY KEY,
    Username NVARCHAR(50),
    Name NVARCHAR(100), -- Added Name field
    Email NVARCHAR(100),
    Password NVARCHAR(50)
);

-- EmailsSent Table
CREATE TABLE EmailsSent (
    EmailID INT IDENTITY PRIMARY KEY,
    UserID NVARCHAR(50),
    EmailFrom NVARCHAR(100),
    EmailTo NVARCHAR(100),
    EmailSubject NVARCHAR(200),
    EmailBody NVARCHAR(MAX),
    SentDate DATETIME,
    RefNo NVARCHAR(50),
    MembershipNo NVARCHAR(50)
);

-- Complaints Table
CREATE TABLE Complaints (
    ComplaintID INT IDENTITY PRIMARY KEY,
    TicketNo NUMERIC(10, 0),
    MembershipNo NVARCHAR(250),
    RequestDate SMALLDATETIME,
    Category NVARCHAR(50),
    Subject NVARCHAR(1600),
    Status NVARCHAR(1), -- 'O' for Open, 'C' for Closed
    UpdateDate SMALLDATETIME,
    UpdatedBy NVARCHAR(50),
    ForwardedTo NVARCHAR(50),
    ForwardedDate SMALLDATETIME,
    ForwardRemarks NVARCHAR(250),
    ForwardedBy NVARCHAR(50),
    Email NVARCHAR(200),
    TopCategory NVARCHAR(30),
    CorporateDetails NVARCHAR(12),
    Urgent NVARCHAR(5),
    ReceivedFrom NVARCHAR(25),
    PointsExp NVARCHAR(1),
    Tier NVARCHAR(1),
    DownloadDateTime DATETIME,
    HititRefNo NVARCHAR(50),
    Body NVARCHAR(MAX)
);

-- Insert Sample Data
INSERT INTO Admins (Username, Name, Email, Password)
VALUES 
('admin1', 'Rana Musab', 'admin1@pia.com', 'pass123'),
('admin2', 'Abdullah Rashid', 'admin2@pia.com', 'pass456');

SELECT * FROM Admins;

INSERT INTO Complaints (TicketNo, MembershipNo, RequestDate, Category, Subject, Status, Email, ReceivedFrom, Body)
VALUES 
(1001, 'PIA123456', GETDATE()-10, 'Missing Miles', 'Missing miles on flight PK123', 'O', 'customer@pia.com', 'Customer', 'Dear PIA, I did not receive miles for my recent flight.'),
(1002, 'PIA789012', GETDATE()-5, 'Redemption', 'Upgrade issue', 'O', 'customer2@pia.com', 'Customer', 'Unable to upgrade my ticket.');

SELECT * FROM Complaints;

INSERT INTO EmailsSent (UserID, EmailFrom, EmailTo, EmailSubject, EmailBody, SentDate, MembershipNo)
VALUES 
('admin1', 'admin@pia.com', 'customer@pia.com', 'Re: Missing Miles', 'We are looking into your issue.', GETDATE()-1, 'PIA123456');

SELECT * FROM EmailsSent;
drop table Complaints;