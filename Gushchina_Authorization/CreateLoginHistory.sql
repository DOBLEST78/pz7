USE [rest];
GO

IF OBJECT_ID(N'dbo.LoginHistory', N'U') IS NULL
BEGIN
    CREATE TABLE dbo.LoginHistory
    (
        Id INT IDENTITY(1,1) PRIMARY KEY,
        UserLogin NVARCHAR(50) NOT NULL,
        AttemptTime DATETIME NOT NULL,
        IsSuccess BIT NOT NULL,
        FailReason NVARCHAR(200) NULL
    );
END;
GO
