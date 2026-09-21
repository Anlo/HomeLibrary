-- ============ SELECT (получение всех книг) ============
CREATE PROCEDURE dbo.Book_SelectAll
AS
BEGIN
    SET NOCOUNT ON;
    SELECT Id, Title, Author, PublishYear, PageCount, 
           TableOfContents, CreatedAt, UpdatedAt
    FROM dbo.Books
    ORDER BY Title;
END
GO

-- ============ SELECT по Id ============
CREATE PROCEDURE dbo.Book_SelectById
    @Id INT
AS
BEGIN
    SET NOCOUNT ON;
    SELECT Id, Title, Author, PublishYear, PageCount, 
           TableOfContents, CreatedAt, UpdatedAt
    FROM dbo.Books
    WHERE Id = @Id;
END
GO

-- ============ INSERT ============
CREATE PROCEDURE dbo.Book_Insert
    @Title NVARCHAR(200),
    @Author NVARCHAR(150),
    @PublishYear INT,
    @PageCount INT,
    @TableOfContents XML
AS
BEGIN
    SET NOCOUNT ON;
    INSERT INTO dbo.Books (Title, Author, PublishYear, PageCount, TableOfContents)
    VALUES (@Title, @Author, @PublishYear, @PageCount, @TableOfContents);
    
    SELECT SCOPE_IDENTITY() AS NewId;
END
GO

-- ============ UPDATE ============
CREATE PROCEDURE dbo.Book_Update
    @Id INT,
    @Title NVARCHAR(200),
    @Author NVARCHAR(150),
    @PublishYear INT,
    @PageCount INT,
    @TableOfContents XML
AS
BEGIN
    SET NOCOUNT ON;
    UPDATE dbo.Books
    SET Title = @Title,
        Author = @Author,
        PublishYear = @PublishYear,
        PageCount = @PageCount,
        TableOfContents = @TableOfContents,
        UpdatedAt = SYSDATETIME()
    WHERE Id = @Id;
END
GO

-- ============ DELETE ============
CREATE PROCEDURE dbo.Book_Delete
    @Id INT
AS
BEGIN
    SET NOCOUNT ON;
    DELETE FROM dbo.Books WHERE Id = @Id;
END
GO

-- ============ ПОИСК по названию, автору и содержимому оглавления ============
CREATE PROCEDURE dbo.Book_Search
    @SearchString NVARCHAR(200)
AS
BEGIN
    SET NOCOUNT ON;
    DECLARE @pattern NVARCHAR(210) = N'%' + @SearchString + N'%';
    
    SELECT Id, Title, Author, PublishYear, PageCount, 
           TableOfContents, CreatedAt, UpdatedAt
    FROM dbo.Books
    WHERE Title LIKE @pattern
       OR Author LIKE @pattern
       OR CAST(TableOfContents AS NVARCHAR(MAX)) LIKE @pattern
    ORDER BY Title;
END
GO