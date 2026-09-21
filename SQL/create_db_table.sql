-- Создание базы данных
CREATE DATABASE HomeLibrary;
GO

USE HomeLibrary;
GO

-- Таблица книг с XML-полем для оглавления
CREATE TABLE Books (
    Id INT IDENTITY(1,1) PRIMARY KEY,
    Title NVARCHAR(200) NOT NULL,
    Author NVARCHAR(150) NOT NULL,
    PublishYear INT NOT NULL,
    PageCount INT NOT NULL,
    TableOfContents XML NULL, -- Оглавление в виде XML
    CreatedAt DATETIME2 DEFAULT SYSDATETIME(),
    UpdatedAt DATETIME2 DEFAULT SYSDATETIME()
);
GO