# Admin Dashboard Layout - BikeMarket

## ?? Overview
A professional admin dashboard with a fixed sidebar navigation menu on the left side, modern UI design, and responsive layout.

## ? Features

### 1. **Sidebar Navigation**
- Fixed left sidebar with gradient blue background
- Collapsible/expandable menu
- Icons for each menu item
- Badge notifications (e.g., "New" for Post Moderation)
- User info section at bottom
- Logout button

### 2. **Main Content Area**
- Top navigation bar with:
  - Sidebar toggle button
  - Notification dropdown
- Page header with breadcrumbs
- Responsive content wrapper
- Clean white background

### 3. **Dashboard Features**
- Statistics cards with icons
- Quick action cards with hover effects
- Recent activity timeline
- Modern card-based design

## ?? File Structure

```
BikeMarket/
??? Views/
?   ??? Shared/
?   ?   ??? _AdminLayout.cshtml          # Main admin layout template
?   ??? HomeAdmin/
?   ?   ??? _ViewStart.cshtml            # Sets layout to _AdminLayout
?   ?   ??? Index.cshtml                 # Dashboard page
?   ??? Moderation/
?       ??? _ViewStart.cshtml            # Sets layout to _AdminLayout
?       ??? Index.cshtml                 # Moderation list page
?       ??? Details.cshtml               # Moderation details page
??? wwwroot/
    ??? css/
        ??? admin.css                     # Admin-specific styles
```

## ?? How It Works

### Layout Structure

The `_AdminLayout.cshtml` consists of:

1. **Sidebar** (`<div class="sidebar">`)
   - Header with logo
   - Navigation menu
   - Footer with user info

2. **Main Content** (`<div class="main-content">`)
   - Top navbar
   - Content wrapper
   - Footer

### Automatic Layout Selection

Pages in the `HomeAdmin` and `Moderation` folders automatically use the admin layout through `_ViewStart.cshtml`:

```csharp
@{
    Layout = "_AdminLayout";
}
```

### Menu Highlighting

Active menu items are automatically highlighted based on the current URL path using JavaScript.

## ?? Styling

### Color Scheme
- **Primary Blue**: `#1e3a8a` to `#1e40af` (gradient)
- **Warning**: `#fbbf24` (for pending posts)
- **Success**: Green
- **Danger**: Red
- **Info**: Light blue

### Sidebar States

**Expanded (Default):**
- Width: 260px
- Shows all text and icons

**Collapsed:**
- Width: 70px
- Shows only icons
- Text hidden

### Responsive Behavior

**Mobile (< 768px):**
- Sidebar automatically collapses to 70px
- Expands on hover
- Main content adjusts accordingly

## ?? Usage

### For New Admin Pages

1. Create a folder for your feature (e.g., `Views/Reports/`)

2. Add `_ViewStart.cshtml` to the folder:
```csharp
@{
    Layout = "_AdminLayout";
}
```

3. Create your view files (they'll automatically use the admin layout)

### Customizing Menu

Edit `Views/Shared/_AdminLayout.cshtml` to add/remove menu items:

```html
<li class="menu-item">
    <a asp-controller="YourController" asp-action="Index" class="menu-link">
        <i class="bi bi-your-icon"></i>
        <span>Menu Label</span>
    </a>
</li>
```

### Adding Badges

```html
<span class="badge bg-warning text-dark ms-auto">New</span>
```

## ?? Dashboard Components

### Statistics Card

```html
<div class="stat-card">
    <div class="stat-icon bg-warning bg-opacity-10 text-warning">
        <i class="bi bi-clipboard-check"></i>
    </div>
    <div class="stat-content">
        <h3>12</h3>
        <p class="mb-0">Pending Posts</p>
        <small class="text-muted">Awaiting review</small>
    </div>
</div>
```

### Action Card

```html
<div class="card dashboard-card border-start border-warning border-4">
    <div class="card-body text-center">
        <i class="bi bi-clipboard-check text-warning"></i>
        <h5 class="card-title mt-3">Card Title</h5>
        <p class="card-text text-muted">Description</p>
        <a href="#" class="btn btn-warning">
            <i class="bi bi-arrow-right-circle"></i> Action
        </a>
    </div>
</div>
```

### Page Header with Breadcrumbs

```html
<div class="page-header">
    <h1><i class="bi bi-icon"></i> Page Title</h1>
    <nav aria-label="breadcrumb">
        <ol class="breadcrumb">
            <li class="breadcrumb-item"><a href="#">Home</a></li>
            <li class="breadcrumb-item active">Current Page</li>
        </ol>
    </nav>
</div>
```

## ?? Customization

### Changing Sidebar Color

Edit `admin.css`:

```css
.sidebar {
    background: linear-gradient(180deg, #your-color-1 0%, #your-color-2 100%);
}
```

### Changing Sidebar Width

Edit `admin.css`:

```css
.sidebar {
    width: 280px; /* Change from 260px */
}

.main-content {
    margin-left: 280px; /* Match sidebar width */
}
```

### Adding Hover Effects

```css
.dashboard-card {
    transition: transform 0.3s ease, box-shadow 0.3s ease;
}

.dashboard-card:hover {
    transform: translateY(-5px);
    box-shadow: 0 5px 15px rgba(0,0,0,0.15);
}
```

## ?? Mobile Optimization

The layout is fully responsive:
- Sidebar collapses on mobile
- Cards stack vertically
- Tables scroll horizontally
- Touch-friendly buttons

## ?? Best Practices

1. **Consistent Icons**: Use Bootstrap Icons throughout
2. **Color Coding**: Use consistent colors for actions:
   - Blue: Primary actions
   - Green: Success/Approve
   - Red: Danger/Delete
   - Yellow: Warning/Pending
   - Gray: Secondary/Cancel

3. **Spacing**: Use Bootstrap spacing utilities (mt-3, mb-4, etc.)
4. **Cards**: Use cards for grouping related content
5. **Alerts**: Use dismissible alerts for messages

## ?? Troubleshooting

### Sidebar not showing
- Check if `admin.css` is loaded
- Verify Bootstrap CSS is included
- Check browser console for errors

### Menu not highlighting
- Check if JavaScript is enabled
- Verify Bootstrap JS is loaded
- Check URL paths match menu links

### Layout breaks on mobile
- Check viewport meta tag
- Verify responsive classes (col-md-, col-lg-, etc.)
- Test in browser dev tools responsive mode

## ?? Notes

- Bootstrap 5.3+ required
- Bootstrap Icons required
- jQuery included for legacy support
- Modern browsers supported (Chrome, Firefox, Safari, Edge)

## ?? Result

You now have a professional admin dashboard with:
- ? Fixed sidebar navigation
- ? Responsive design
- ? Modern UI components
- ? Easy to customize
- ? Consistent styling
- ? Mobile-friendly

Perfect for managing your BikeMarket admin panel! ??
