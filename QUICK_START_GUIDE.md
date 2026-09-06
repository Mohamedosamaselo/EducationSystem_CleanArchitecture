# 🚀 Quick Start Guide - Domain Layer Best Practices

## Using Value Objects

### Email
```csharp
// ✅ Valid usage
var email = new Email("user@example.com");
string emailStr = (string)email;  // Explicit cast

// ❌ Will throw ArgumentException
var invalid = new Email("not-an-email");
var empty = new Email("");
var toolong = new Email("x".PadRight(300) + "@example.com");
```

### Address
```csharp
// ✅ Create with all required fields
var address = new Address(
	street: "123 Main Street",
	city: "Cairo",
	postalCode: "11111",
	country: "Egypt"
);

// Convert to string
string display = address.ToString();
// Output: "123 Main Street, Cairo 11111, Egypt"

// Value equality
var addr2 = new Address("123 Main Street", "Cairo", "11111", "Egypt");
bool isSame = address == addr2;  // true
```

### PhoneNumber
```csharp
// ✅ Valid formats all work
var phone1 = new PhoneNumber("+1-234-567-8900");
var phone2 = new PhoneNumber("+1 (234) 567-8900");
var phone3 = new PhoneNumber("12345678900");

// All normalize to: "+12345678900"
string value = phone1.Value;

// ❌ Invalid formats throw
var invalid = new PhoneNumber("invalid");
var empty = new PhoneNumber("");
```

---

## Using Status Enums

### Examples
```csharp
// User Status
var user = new ApplicationUser 
{ 
	Status = UserStatus.Active  // Default
};

// Role Status
var role = new ApplicationRole
{
	Status = RoleStatus.Active
};

// School Status
var school = new School
{
	Status = SchoolStatus.Active
};

// Permission Status
var permission = new Permission
{
	Status = PermissionStatus.Active
};
```

### Querying
```csharp
// Find active users only
var activeUsers = context.Users
	.Where(u => u.Status == UserStatus.Active)
	.ToList();

// Find inactive schools
var inactive = context.Schools
	.Where(s => s.Status == SchoolStatus.Inactive)
	.ToList();

// Find archived permissions
var archived = context.Permissions
	.Where(p => p.Status == PermissionStatus.Archived)
	.ToList();
```

---

## Using Soft Delete

### Soft Deleting
```csharp
// Soft delete with user tracking
var school = await context.Schools.FindAsync(schoolId);
school.SoftDelete(currentUserId);  // Tracks who deleted
await context.SaveChangesAsync();

// Properties updated:
// - DeletedAt = DateTime.UtcNow
// - DeletedBy = currentUserId
// - IsDeleted = true (computed)
```

### Querying
```csharp
// Get only active (non-deleted) schools
var activeSchools = context.Schools
	.Where(s => !s.IsDeleted)
	.ToList();

// Get all schools including deleted
var allSchools = context.Schools
	.IgnoreQueryFilters()  // If using query filters
	.ToList();

// Check if deleted
if (school.IsDeleted)
{
	Console.WriteLine($"Deleted by: {school.DeletedBy}");
	Console.WriteLine($"Deleted on: {school.DeletedAt}");
}
```

### Restoring
```csharp
// Restore a soft-deleted entity
school.Restore();
await context.SaveChangesAsync();

// Properties updated:
// - DeletedAt = null
// - DeletedBy = null
// - IsDeleted = false (computed)
```

---

## Creating Entities with New Features

### User Example
```csharp
var user = new ApplicationUser
{
	Id = Guid.NewGuid(),
	FirstName = "John",
	LastName = "Doe",
	Address = new Address("123 Main St", "Cairo", "11111", "Egypt"),
	Email = "john@example.com",
	Status = UserStatus.Active,
	DateOfBirth = new DateTime(1990, 1, 1),

	// Audit fields (auto-populated in ApplicationDbContext)
	CreatedAt = DateTime.UtcNow,
	CreatedBy = currentUserId,

	// Foreign keys/Navigation
	SchoolId = schoolId,
	School = null,  // Can be null if not assigned
	GradeId = gradeId,
	Grade = null    // Can be null if not assigned
};
```

### School Example
```csharp
var school = new School
{
	Id = Guid.NewGuid(),
	Name = "Cairo International School",
	Address = new Address("456 Pyramid Ave", "Giza", "12222", "Egypt"),
	Email = new Email("contact@cairo-intl.edu").Value,  // If still using string
	Status = SchoolStatus.Active,
	LogoUrl = "https://example.com/logo.png",

	OrganisationId = organisationId,

	// Audit fields (auto-populated)
	CreatedAt = DateTime.UtcNow,
	CreatedBy = userId
};
```

---

## DTOs & Value Objects

### Converting Value Objects to DTOs
```csharp
// In Service
public SchoolResponse MapToResponse(School school)
{
	return new SchoolResponse
	{
		Name = school.Name,
		Address = school.Address.ToString(),  // Convert to string
		OrganisationId = school.OrganisationId
	};
}

// When Creating
var school = new School
{
	Address = new Address(
		dto.Address,  // Assume DTO has full address, parse as needed
		"Cairo", 
		"11111", 
		"Egypt"
	)
};
```

---

## Auditing

### Tracking Changes
```csharp
// All entities inheriting from BaseAuditableEntity track:
public DateTime CreatedAt { get; set; }      // When created
public Guid? CreatedBy { get; set; }         // Who created
public DateTime? ModifiedAt { get; set; }    // When last modified
public Guid? LastModifiedBy { get; set; }    // Who last modified

// For soft-deletable entities also:
public DateTime? DeletedAt { get; set; }     // When deleted
public Guid? DeletedBy { get; set; }         // Who deleted
public bool IsDeleted { get; }               // Computed property
```

### Using Audit Info
```csharp
var school = await context.Schools.FindAsync(schoolId);

Console.WriteLine($"Created: {school.CreatedAt} by {school.CreatedBy}");
Console.WriteLine($"Last modified: {school.ModifiedAt} by {school.LastModifiedBy}");

if (school.IsDeleted)
{
	Console.WriteLine($"Deleted: {school.DeletedAt} by {school.DeletedBy}");
}
```

---

## Common Errors & Fixes

### ❌ Error: "Cannot implicitly convert string to Address"
```csharp
// Wrong
school.Address = "Cairo";

// Right
school.Address = new Address("Street", "Cairo", "11111", "Egypt");
```

### ❌ Error: "Address does not have IsActive"
```csharp
// Wrong
school.IsActive = true;

// Right
school.Status = SchoolStatus.Active;
```

### ❌ Error: "School cannot be null"
```csharp
// Wrong
public class User
{
	public School School { get; set; }  // ← Nullable FK but non-nullable nav
	public Guid? SchoolId { get; set; }
}

// Right
public class User
{
	public School? School { get; set; }  // ← Matches nullable FK
	public Guid? SchoolId { get; set; }
}
```

### ❌ Error: "Invalid email format"
```csharp
// Wrong
var email = new Email("not-an-email");

// Right - validate before creating
try 
{
	var email = new Email(userInput);
}
catch (ArgumentException ex)
{
	ModelState.AddModelError("Email", ex.Message);
}
```

---

## Migration to Database

### When to Migrate
After implementing value objects and status enums, create a new migration:

```powershell
# In Package Manager Console
Add-Migration DomainLayerEnhancements -Project EducationSystem.Infrastructure

# Or via CLI
dotnet ef migrations add DomainLayerEnhancements -p EducationSystem.Infrastructure
```

### Key Schema Changes
1. **Status columns** - Convert from bit (IsActive) to int (Status enum)
   ```sql
   ALTER TABLE AspNetUsers ADD Status INT DEFAULT 1;
   ALTER TABLE AspNetRoles ADD Status INT DEFAULT 1;
   ALTER TABLE Schools ADD Status INT DEFAULT 1;
   ```

2. **Address columns** - Keep as nvarchar (value object converts to string)
   ```sql
   -- No schema change needed, value object handles serialization
   ```

3. **Soft delete columns** - Add for soft-deletable entities
   ```sql
   ALTER TABLE Schools ADD DeletedAt DATETIME2 NULL;
   ALTER TABLE Schools ADD DeletedBy UNIQUEIDENTIFIER NULL;
   ```

---

## Best Practices Checklist

- [ ] Always validate before creating value objects
- [ ] Use nullable navigations when FK is nullable
- [ ] Check `Status` instead of `IsActive`
- [ ] Query `!IsDeleted` to exclude deleted items
- [ ] Provide deletion reason (track in DeletedBy)
- [ ] Test value object equality and conversions
- [ ] Keep DTOs as strings, convert in services
- [ ] Document custom value object validation
- [ ] Use query filters to auto-exclude deleted entities
- [ ] Audit critical entity changes

---

## Resources

📖 **See Full Documentation:** `EducationSystem.Domain/DOMAIN_BEST_PRACTICES.md`

🔧 **Key Files:**
- `EducationSystem.Domain/ValueObjects/Email.cs`
- `EducationSystem.Domain/ValueObjects/Address.cs`
- `EducationSystem.Domain/ValueObjects/PhoneNumber.cs`
- `EducationSystem.Domain/Common/BaseSoftDeleteEntity.cs`
- `EducationSystem.Domain/Enums/UserStatus.cs`
- `EducationSystem.Domain/Enums/RoleStatus.cs`
- `EducationSystem.Domain/Enums/SchoolStatus.cs`
- `EducationSystem.Domain/Enums/PermissionStatus.cs`

---

**Quick Reference v1.0**  
**Last Updated:** 2024
