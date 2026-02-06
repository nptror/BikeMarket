# Users Search, Filter, and Sort Feature

## ?? Summary
Added comprehensive search, filter, and sort functionality to the Users management page, allowing admins to easily find and organize users.

## ? Changes Made

### 1. **Data Access Layer**

#### Updated: `DataAccess/Interface/IUserRepository.cs`
- Added overload `GetAllAsync` method with parameters:
  - `search` (string) - Search by name or email
  - `ratingAvg` (decimal) - Minimum rating filter
  - `role` (string) - Filter by role (buyer/seller/admin)
  - `sortBy` (string) - Sort field (name/email/ratingavg)
  - `sortOrder` (string) - Sort direction (asc/desc)

#### Updated: `DataAccess/Repository/UserRepository.cs`
- Implemented advanced filtering logic:
  ```csharp
  // Search filter - case insensitive
  if (!string.IsNullOrWhiteSpace(search))
  {
      var searchLower = search.ToLower();
      query = query.Where(u =>
          (u.Name != null && u.Name.ToLower().Contains(searchLower)) ||
          (u.Email != null && u.Email.ToLower().Contains(searchLower)));
  }
  
  // Rating filter
  if (ratingAvg > 0)
  {
      query = query.Where(u => u.RatingAvg >= ratingAvg);
  }
  
  // Role filter
  if (!string.IsNullOrWhiteSpace(role) && role != "all")
  {
      query = query.Where(u => u.Role == role);
  }
  
  // Dynamic sorting with switch expression
  query = sortBy?.ToLower() switch
  {
      "name" => sortOrder?.ToLower() == "desc"
          ? query.OrderByDescending(u => u.Name)
          : query.OrderBy(u => u.Name),
      "email" => sortOrder?.ToLower() == "desc"
          ? query.OrderByDescending(u => u.Email)
          : query.OrderBy(u => u.Email),
      "ratingavg" => sortOrder?.ToLower() == "desc"
          ? query.OrderByDescending(u => u.RatingAvg)
          : query.OrderBy(u => u.RatingAvg),
      _ => sortOrder?.ToLower() == "desc"
          ? query.OrderByDescending(u => u.Email)
          : query.OrderBy(u => u.Email)
  };
  ```

### 2. **Business Layer**

#### Already Exists: `DTO/User/UserProfileDTO.cs`
```csharp
public class UserProfileDTO
{
    public int Id { get; set; }
    public string Name { get; set; }
    public string Email { get; set; }
    public string? Phone { get; set; }
    public string Role { get; set; }
    public decimal RatingAvg { get; set; }
}
```

#### Updated: `Business/Interface/IUserService.cs`
- Added `GetAllUserAsync` method with same filter parameters
- Returns `List<UserProfileDTO>` instead of `List<User>`

#### Updated: `Business/Service/UserService.cs`
- Implemented `GetAllUserAsync`:
  ```csharp
  public async Task<List<UserProfileDTO>> GetAllUserAsync(...)
  {
      var users = await _userRepository.GetAllAsync(search, ratingAvg, role, sortBy, sortOrder);
      
      return users.Select(u => new UserProfileDTO
      {
          Id = u.Id,
          Name = u.Name,
          Email = u.Email,
          Phone = u.Phone,
          Role = u.Role,
          RatingAvg = u.RatingAvg ?? 0
      }).ToList();
  }
  ```

### 3. **Controller**

#### Updated: `BikeMarket/Controllers/UsersController.cs`
- Updated Index action to accept query parameters:
  ```csharp
  public async Task<IActionResult> Index(
      string? search = null,
      decimal ratingAvg = 0,
      string? role = null,
      string? sortBy = "email",
      string? sortOrder = "asc")
  {
      var users = await _userService.GetAllUserAsync(search, ratingAvg, role, sortBy, sortOrder);
      
      // Pass filter values to view via ViewBag
      ViewBag.Search = search;
      ViewBag.RatingAvg = ratingAvg;
      ViewBag.Role = role;
      ViewBag.SortBy = sortBy;
      ViewBag.SortOrder = sortOrder;
      
      return View(users);
  }
  ```

### 4. **View**

#### Updated: `BikeMarket/Views/Users/Index.cshtml`
- Changed model to `IEnumerable<DTO.User.UserProfileDTO>`
- Added comprehensive search/filter form with Bootstrap styling
- Form fields:
  1. **Search** - Text input for name/email search
  2. **Min Rating** - Number input (0-5, step 0.1)
  3. **Role Filter** - Dropdown (All/Buyer/Seller/Admin)
  4. **Sort By** - Dropdown (Email/Name/Rating)
  5. **Sort Order** - Dropdown (Ascending/Descending)
- Apply Filters and Clear buttons
- Removed Status and Created Date columns (not in DTO)
- Updated statistics to work with DTO
- Empty state message updated for filtered results

## ?? Features

### 1. **Search Functionality**
- Search by Name OR Email
- Case-insensitive
- Partial matching (contains)
- Example: "john" matches "John Doe" or "john@example.com"

### 2. **Rating Filter**
- Filter users with rating >= specified value
- Range: 0 to 5
- Decimal support (e.g., 4.5)
- 0 = show all users

### 3. **Role Filter**
- Filter by specific role:
  - All Roles (default)
  - Buyer
  - Seller
  - Admin

### 4. **Dynamic Sorting**
- Sort by:
  - **Email** (default)
  - **Name**
  - **Rating**
- Sort order:
  - **Ascending** (default)
  - **Descending**

### 5. **Form State Preservation**
- All filter values preserved after submit
- ViewBag passes values back to form
- Selected options remain selected

## ?? UI Components

### Search/Filter Card:
```html
<div class="card shadow-sm mb-4">
    <div class="card-body">
        <form method="get" class="row g-3">
            <!-- Search field -->
            <!-- Rating filter -->
            <!-- Role dropdown -->
            <!-- Sort by dropdown -->
            <!-- Sort order dropdown -->
            <!-- Buttons -->
        </form>
    </div>
</div>
```

### Form Layout:
- **Row 1**: 
  - Search (4 cols)
  - Min Rating (2 cols)
  - Role (2 cols)
  - Sort By (2 cols)
  - Sort Order (2 cols)
- **Row 2**:
  - Apply Filters button (primary)
  - Clear button (secondary, links to Index without params)

### Icons:
- Search: `bi-search`
- Rating: `bi-star`
- Role: `bi-shield`
- Sort By: `bi-sort-down`
- Sort Order: `bi-arrow-down-up`
- Apply: `bi-funnel`
- Clear: `bi-x-circle`

## ?? Query String Format

### Example URLs:
```
/Users/Index
/Users/Index?search=john
/Users/Index?ratingAvg=4.5
/Users/Index?role=seller
/Users/Index?sortBy=name&sortOrder=desc
/Users/Index?search=admin&role=admin&sortBy=email&sortOrder=asc
```

### Parameters:
| Parameter | Type | Default | Description |
|-----------|------|---------|-------------|
| search | string | null | Search term |
| ratingAvg | decimal | 0 | Minimum rating |
| role | string | null | Role filter |
| sortBy | string | "email" | Sort field |
| sortOrder | string | "asc" | Sort direction |

## ?? Filter Logic

### Search Priority:
1. Matches Name (case-insensitive)
2. OR Matches Email (case-insensitive)

### Multiple Filters:
Filters are combined with AND logic:
```
Users WHERE 
    (Name LIKE '%search%' OR Email LIKE '%search%')
    AND RatingAvg >= ratingAvg
    AND Role = role
ORDER BY sortBy sortOrder
```

## ? Benefits

### 1. **User Management**
- Quick user lookup by name/email
- Filter by role for targeted management
- Sort by rating to find top/low performers

### 2. **Admin Efficiency**
- No need to scroll through all users
- Find specific users instantly
- Analyze users by role

### 3. **Data Analysis**
- Sort by rating to identify issues
- Filter sellers/buyers separately
- Search for specific accounts

### 4. **User Experience**
- Clean, intuitive interface
- All filters on one page
- Form state preserved
- Clear button for reset

## ?? Use Cases

### 1. Find Specific User:
```
Search: "john.doe@gmail.com"
? Returns exact match
```

### 2. Find Top Sellers:
```
Role: Seller
Sort By: Rating
Sort Order: Descending
? Returns sellers sorted by highest rating
```

### 3. Find Low-Rated Users:
```
Min Rating: 0
Sort By: Rating
Sort Order: Ascending
? Returns all users, lowest rated first
```

### 4. Find Admins:
```
Role: Admin
? Returns only admin users
```

### 5. Complex Filter:
```
Search: "bike"
Role: Seller
Min Rating: 4.0
Sort By: Name
Sort Order: Ascending
? Returns sellers with "bike" in name/email, rating >= 4.0, sorted by name
```

## ?? Technical Details

### Database Query:
- Uses `IQueryable<User>` for deferred execution
- Filters applied before query execution
- Only one database round-trip
- Efficient with indexes on Name, Email, Role, RatingAvg

### Performance:
- **Before**: `SELECT * FROM Users`
- **After**: `SELECT * FROM Users WHERE ... ORDER BY ...`
- Filtered at database level (not in-memory)
- Scales well with large datasets

### Null Handling:
- All filter parameters optional (nullable)
- Default values provided
- Null checks before filtering
- Empty string treated as null

## ?? Simplified Table View

### Removed Columns:
- ~~Status~~ (not in UserProfileDTO)
- ~~Created Date~~ (not in UserProfileDTO)
- ~~Email Verified badge~~ (not in UserProfileDTO)

### Kept Columns:
- ? Name (with avatar)
- ? Email
- ? Phone
- ? Role (badge)
- ? Rating (stars)
- ? Actions

**Why simplified?**
- UserProfileDTO focuses on core user info
- Search/filter functionality is more important
- Cleaner, faster table rendering
- Can be expanded later if needed

## ?? Future Enhancements (Optional)

1. **Pagination**
   - Add page size and page number
   - Display "Showing X-Y of Z users"

2. **Export**
   - Export filtered results to CSV/Excel

3. **Advanced Search**
   - Date range filter (created between dates)
   - Multiple role selection
   - Status filter

4. **Saved Filters**
   - Save common filter combinations
   - Quick filter buttons

5. **Bulk Actions**
   - Select multiple users
   - Bulk role change
   - Bulk status update

## ? Testing Checklist

- [x] Search by name works
- [x] Search by email works
- [x] Rating filter works (>=)
- [x] Role filter works (all roles)
- [x] Sort by email works (asc/desc)
- [x] Sort by name works (asc/desc)
- [x] Sort by rating works (asc/desc)
- [x] Multiple filters combine correctly
- [x] Form state preserved after submit
- [x] Clear button resets filters
- [x] Empty state shows when no results
- [x] Statistics update with filtered data
- [x] Build successful

## ?? Result

Users management now has **powerful search, filter, and sort capabilities**, making it easy for admins to find and manage users efficiently! ??

The implementation is:
- ? **Performant** - Database-level filtering
- ? **User-friendly** - Intuitive UI
- ? **Flexible** - Multiple filter combinations
- ? **Maintainable** - Clean architecture
- ? **Scalable** - Works with large datasets

Perfect for production use! ?
