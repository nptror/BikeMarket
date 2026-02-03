-- Script SQL ð? c?p nh?t và qu?n l? tr?ng thái ki?m duy?t tin ðãng

-- 1. Xem t?t c? tin ðãng có status = NULL ho?c r?ng
SELECT Id, Title, Status, CreatedAt, SellerId
FROM Vehicle
WHERE Status IS NULL OR Status = '';

-- 2. C?p nh?t t?t c? tin có status NULL thành 'pending'
UPDATE Vehicle
SET Status = 'pending'
WHERE Status IS NULL OR Status = '';

-- 3. Xem danh sách tin ch? duy?t
SELECT 
    v.Id,
    v.Title,
    v.Price,
    v.Status,
    v.CreatedAt,
    b.Name AS BrandName,
    c.Name AS CategoryName,
    u.Name AS SellerName
FROM Vehicle v
LEFT JOIN Brand b ON v.BrandId = b.Id
LEFT JOIN Category c ON v.CategoryId = c.Id
LEFT JOIN [User] u ON v.SellerId = u.Id
WHERE v.Status = 'pending'
ORDER BY v.CreatedAt DESC;

-- 4. Th?ng kê s? lý?ng tin theo tr?ng thái
SELECT 
    Status,
    COUNT(*) AS TotalVehicles,
    AVG(Price) AS AvgPrice,
    MIN(CreatedAt) AS OldestPost,
    MAX(CreatedAt) AS NewestPost
FROM Vehicle
GROUP BY Status
ORDER BY TotalVehicles DESC;

-- 5. T?m các tin ðãng ð? ch? duy?t quá 7 ngày
SELECT 
    v.Id,
    v.Title,
    v.Status,
    v.CreatedAt,
    DATEDIFF(day, v.CreatedAt, GETDATE()) AS DaysPending,
    u.Name AS SellerName
FROM Vehicle v
LEFT JOIN [User] u ON v.SellerId = u.Id
WHERE v.Status = 'pending'
  AND DATEDIFF(day, v.CreatedAt, GETDATE()) > 7
ORDER BY v.CreatedAt ASC;

-- 6. Duy?t m?t tin ðãng c? th? (thay @VehicleId)
DECLARE @VehicleId INT = 1; -- Thay s? ID ? ðây
UPDATE Vehicle
SET Status = 'available'
WHERE Id = @VehicleId;

-- 7. T? ch?i m?t tin ðãng v?i l? do (thay @VehicleId và @Reason)
DECLARE @VehicleId INT = 1; -- Thay s? ID ? ðây
DECLARE @Reason NVARCHAR(500) = N'H?nh ?nh không r? ràng'; -- Thay l? do ? ðây

UPDATE Vehicle
SET 
    Status = 'rejected',
    Description = CONCAT('[REJECTED: ', @Reason, ']', CHAR(13), CHAR(10), CHAR(13), CHAR(10), Description)
WHERE Id = @VehicleId;

-- 8. T?o b?ng l?ch s? ki?m duy?t (optional - ð? theo d?i)
CREATE TABLE ModerationHistory (
    Id INT PRIMARY KEY IDENTITY(1,1),
    VehicleId INT NOT NULL,
    AdminId INT NULL,
    Action NVARCHAR(50) NOT NULL, -- 'Approved', 'Rejected'
    Reason NVARCHAR(MAX) NULL,
    ModeratedAt DATETIME NOT NULL DEFAULT GETDATE(),
    FOREIGN KEY (VehicleId) REFERENCES Vehicle(Id),
    FOREIGN KEY (AdminId) REFERENCES [User](Id)
);

-- 9. Thêm index ð? tãng hi?u su?t query
CREATE INDEX IX_Vehicle_Status ON Vehicle(Status);
CREATE INDEX IX_Vehicle_CreatedAt ON Vehicle(CreatedAt DESC);

-- 10. Xem top 10 ngý?i bán có nhi?u tin ch? duy?t nh?t
SELECT TOP 10
    u.Id,
    u.Name AS SellerName,
    COUNT(v.Id) AS PendingPosts,
    MIN(v.CreatedAt) AS FirstPendingPost
FROM Vehicle v
INNER JOIN [User] u ON v.SellerId = u.Id
WHERE v.Status = 'pending'
GROUP BY u.Id, u.Name
ORDER BY PendingPosts DESC;

-- 11. Reset t?t c? tin v? tr?ng thái pending (ch? dùng khi testing)
-- C?NH BÁO: Ch? dùng trong môi trý?ng development!
-- UPDATE Vehicle SET Status = 'pending';

-- 12. Backup d? li?u trý?c khi th?c hi?n thay ð?i l?n
-- SELECT * INTO Vehicle_Backup FROM Vehicle;
