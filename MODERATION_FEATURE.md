# Tính Nãng Ki?m Duy?t Tin Ðãng - BikeMarket

## T?ng Quan
Ch?c nãng ki?m duy?t tin ðãng cho phép Admin xem xét và phê duy?t các tin ðãng bán xe trý?c khi chúng ðý?c hi?n th? công khai trên h? th?ng.

## Cách Ho?t Ð?ng

### 1. Quy Tr?nh Ðãng Tin
- Khi ngý?i bán t?o tin ðãng m?i, status s? t? ð?ng ðý?c ð?t thành **"pending"** (ch? duy?t)
- Tin ðãng chýa ðý?c duy?t s? không hi?n th? trong danh sách xe available cho ngý?i mua

### 2. Quy Tr?nh Ki?m Duy?t

#### Bý?c 1: Truy c?p trang ki?m duy?t
- T? trang HomeAdmin, click vào **"Ki?m Duy?t Tin Ðãng"**
- Ho?c truy c?p tr?c ti?p: `/Moderation/Index`

#### Bý?c 2: Xem danh sách tin ch? duy?t
- Trang hi?n th? t?t c? các tin ðãng có status = "pending"
- M?i tin bao g?m:
  - H?nh ?nh ð?i di?n
  - Tiêu ð?
  - Giá
  - Thýõng hi?u, lo?i xe
  - Ngý?i bán
  - Ð?a ði?m
  - Ngày ðãng

#### Bý?c 3: Xem chi ti?t tin ðãng
- Click vào **"Xem Chi Ti?t"** ð? xem thông tin ð?y ð?
- Trang chi ti?t hi?n th?:
  - T?t c? h?nh ?nh (carousel)
  - Thông tin chi ti?t xe
  - Mô t?
  - Thông tin ngý?i bán

#### Bý?c 4: Duy?t ho?c T? ch?i

##### Duy?t tin:
1. Click nút **"Duy?t Tin"** (màu xanh)
2. Xác nh?n trong popup
3. Status c?a tin s? chuy?n thành **"available"**
4. Tin s? hi?n th? công khai cho ngý?i mua

##### T? ch?i tin:
1. Click nút **"T? Ch?i"** (màu ð?)
2. Nh?p l? do t? ch?i (không b?t bu?c nhýng nên có)
3. Xác nh?n trong popup
4. Status c?a tin s? chuy?n thành **"rejected"**
5. L? do t? ch?i s? ðý?c lýu vào ph?n Description

## Các Tr?ng Thái Tin Ðãng

| Status | ? Ngh?a | Hi?n th? cho ngý?i mua |
|--------|---------|------------------------|
| **pending** | Ch? ki?m duy?t | ? Không |
| **available** | Ð? duy?t, ðang bán | ? Có |
| **rejected** | Ð? t? ch?i | ? Không |
| **sold** | Ð? bán | ? Không |

## C?u Trúc Code

### 1. DTO Layer
- **VehicleModerationDTO.cs**: DTO cho danh sách tin ch? duy?t
  - Ch?a thông tin cõ b?n ð? hi?n th? trong danh sách

### 2. Business Layer
- **IVehicleService.cs**: Interface m? r?ng v?i các method:
  - `GetPendingVehiclesAsync()`: L?y danh sách tin ch? duy?t
  - `GetVehicleForModerationAsync(int id)`: L?y chi ti?t tin ð? ki?m duy?t
  - `ApproveVehicleAsync(int id)`: Duy?t tin
  - `RejectVehicleAsync(int id, string? reason)`: T? ch?i tin

- **VehicleService.cs**: Implementation c?a các method trên

### 3. Controller Layer
- **ModerationController.cs**: Controller x? l? logic ki?m duy?t
  - `Index()`: Hi?n th? danh sách tin ch? duy?t
  - `Details(int id)`: Hi?n th? chi ti?t tin
  - `Approve(int id)`: X? l? duy?t tin
  - `Reject(int id, string? reason)`: X? l? t? ch?i tin

### 4. View Layer
- **Views/Moderation/Index.cshtml**: Trang danh sách tin ch? duy?t
- **Views/Moderation/Details.cshtml**: Trang chi ti?t tin và form duy?t/t? ch?i

### 5. HomeAdmin
- **Views/HomeAdmin/Index.cshtml**: Ð? thêm link ð?n trang ki?m duy?t

## C?i Ti?n Trong Týõng Lai

### 1. Thông Báo
- G?i email/notification cho ngý?i bán khi tin ðý?c duy?t/t? ch?i
- Hi?n th? s? lý?ng tin ch? duy?t trên dashboard admin

### 2. L?ch S? Ki?m Duy?t
- T?o b?ng ModerationHistory ð? lýu:
  - Admin nào duy?t/t? ch?i
  - Th?i gian
  - L? do
  - Các thay ð?i

### 3. Quy?n H?n
- Phân quy?n: ch? Admin m?i ðý?c ki?m duy?t
- S? d?ng Authentication/Authorization middleware

### 4. T?m Ki?m & L?c
- L?c theo ngày ðãng
- T?m ki?m theo tên, thýõng hi?u
- S?p x?p theo giá, ngày

### 5. Bulk Actions
- Duy?t nhi?u tin cùng lúc
- T? ch?i nhi?u tin cùng lúc

### 6. T? Ð?ng Ki?m Duy?t
- S? d?ng AI ð? phát hi?n n?i dung vi ph?m
- Auto-approve cho ngý?i bán có uy tín cao

## Testing

### Test Cases C?n Th?c Hi?n:
1. ? T?o tin m?i ? ki?m tra status = "pending"
2. ? Duy?t tin ? ki?m tra status = "available"
3. ? T? ch?i tin ? ki?m tra status = "rejected"
4. ? Tin ð? duy?t hi?n th? cho ngý?i mua
5. ? Tin chýa duy?t không hi?n th? cho ngý?i mua
6. ? L? do t? ch?i ðý?c lýu vào Description

## Lýu ?
- Ð?m b?o ð? login v?i tài kho?n Admin ð? truy c?p `/Moderation`
- N?u chýa có Authentication, c?n b? sung middleware ki?m tra quy?n Admin
- Có th? thêm validation ð? ð?m b?o ch? Admin m?i ðý?c access controller này

## Liên H? Support
N?u có v?n ð? ho?c c?n h? tr?, vui l?ng liên h? team phát tri?n.
