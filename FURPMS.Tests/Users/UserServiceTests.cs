using FURPMS.Application.Constants;
using FURPMS.Application.DTOs.Users;
using FURPMS.Application.Interfaces.Services;
using FURPMS.Domain.Entities.Users;
using FURPMS.Infrastructure.Data;
using FURPMS.Infrastructure.Repositories;
using FURPMS.Infrastructure.Services;
using FURPMS.Tests.Helpers;
using Microsoft.EntityFrameworkCore;

namespace FURPMS.Tests.Users;

public class UserServiceTests
{
    private static async Task<(FURPMSDbContext db, Role staff, OrganizationalUnit unit)> SeedAsync()
    {
        var db = TestDbContextFactory.Create($"test-{Guid.NewGuid()}");
        var staff = new Role { Id = 1, Name = "Staff" };
        var admin = new Role { Id = 2, Name = "Admin" };
        var unit = new OrganizationalUnit { Id = 1, Code = "CNTT", Name = "Khoa CNTT", UnitType = "FACULTY" };
        db.Roles.AddRange(staff, admin);
        db.OrganizationalUnits.Add(unit);
        await db.SaveChangesAsync();
        return (db, staff, unit);
    }

    private static UserService Service(FURPMSDbContext db) => new(
        new UserRepository(db), TestNotifier.Create(db), new ProposalRepository(db), new ReviewRepository(db));

    [Fact]
    public async Task Create_ValidUser_TrimsEmailHashesPasswordAssignsRoleAndNotifies()
    {
        var (db, staff, _) = await SeedAsync();

        var result = await Service(db).CreateUserAsync(new CreateUserRequest
        {
            Email = "  new.user@example.com  ", FullName = "Người mới", TemporaryPassword = "TempPass123",
            Roles = new() { staff.Id }
        }, Guid.NewGuid());

        Assert.Equal("new.user@example.com", result.Email);
        Assert.Contains("Staff", result.Roles);
        var saved = await db.Users.SingleAsync(u => u.Id == result.Id);
        Assert.NotEqual("TempPass123", saved.PasswordHash);
        Assert.True(BCrypt.Net.BCrypt.Verify("TempPass123", saved.PasswordHash));
        Assert.True(await db.Notifications.AnyAsync(n => n.UserId == result.Id && n.NotificationType == "ACCOUNT_CREATED"));
    }

    [Theory]
    [InlineData("")]
    [InlineData("not-an-email")]
    [InlineData("person@localhost")]
    [InlineData("person@example")]
    public async Task Create_InvalidEmail_ThrowsArgumentException(string email)
    {
        var (db, staff, _) = await SeedAsync();

        await Assert.ThrowsAsync<ArgumentException>(() => Service(db).CreateUserAsync(new CreateUserRequest
        {
            Email = email, FullName = "Người dùng", TemporaryPassword = "TempPass123", Roles = new() { staff.Id }
        }, Guid.NewGuid()));
    }

    [Fact]
    public async Task Create_DuplicateActiveEmail_ThrowsConflict()
    {
        var (db, staff, _) = await SeedAsync();
        db.Users.Add(new User { Email = "same@example.com", FullName = "Cũ", Status = UserStatus.Active });
        await db.SaveChangesAsync();

        await Assert.ThrowsAsync<InvalidOperationException>(() => Service(db).CreateUserAsync(new CreateUserRequest
        {
            Email = "same@example.com", FullName = "Mới", TemporaryPassword = "TempPass123", Roles = new() { staff.Id }
        }, Guid.NewGuid()));
    }

    [Fact]
    public async Task Create_UnknownRole_ThrowsNotFoundBeforePersistingUser()
    {
        var (db, _, _) = await SeedAsync();

        await Assert.ThrowsAsync<KeyNotFoundException>(() => Service(db).CreateUserAsync(new CreateUserRequest
        {
            Email = "new@example.com", FullName = "Người dùng", TemporaryPassword = "TempPass123", Roles = new() { 999 }
        }, Guid.NewGuid()));

        Assert.Empty(await db.Users.ToListAsync());
    }

    /// <summary>
    /// Xoá mềm một người rồi tạo lại đúng email đó. Trước 29/09 kiểm tra trùng bỏ qua tài khoản đã
    /// xoá, INSERT dòng mới đâm vào UNIQUE(email) trên Postgres ⇒ 500 trên bản deploy. InMemory
    /// không có UNIQUE nên test không bắt được 500 — thay vào đó chứng minh <b>không có dòng thứ
    /// hai</b> nào được thêm, tức là không còn đường đâm vào UNIQUE.
    /// </summary>
    [Fact]
    public async Task Create_EmailOfDeletedUser_RestoresSameRowWithNewDetails()
    {
        var (db, staff, _) = await SeedAsync();
        var admin = await db.Roles.SingleAsync(r => r.Name == "Admin");
        var old = new User
        {
            Email = "back@example.com", FullName = "Tên cũ", Status = "INACTIVE", PasswordHash = "old-hash",
            IsDeleted = true, DeletedAt = DateTime.UtcNow, DeletedBy = Guid.NewGuid()
        };
        db.Users.Add(old);
        db.UserRoles.Add(new UserRole { UserId = old.Id, RoleId = admin.Id });
        await db.SaveChangesAsync();

        var result = await Service(db).CreateUserAsync(new CreateUserRequest
        {
            Email = "back@example.com", FullName = "Tên mới", TemporaryPassword = "TempPass123",
            Roles = new() { staff.Id }
        }, Guid.NewGuid());

        Assert.Equal(old.Id, result.Id);
        var rows = await db.Users.IgnoreQueryFilters().Where(u => u.Email == "back@example.com").ToListAsync();
        var user = Assert.Single(rows);
        Assert.False(user.IsDeleted);
        Assert.Null(user.DeletedAt);
        Assert.Equal(UserStatus.Active, user.Status);
        Assert.Equal("Tên mới", user.FullName);
        Assert.True(BCrypt.Net.BCrypt.Verify("TempPass123", user.PasswordHash));
        // Vai cũ (Admin) KHÔNG được sống lại theo — chỉ vai trên form mới.
        Assert.Equal(new[] { "Staff" }, result.Roles);
    }

    [Fact]
    public async Task Create_SavesUnitAndDegreeFromForm()
    {
        var (db, staff, unit) = await SeedAsync();

        var result = await Service(db).CreateUserAsync(new CreateUserRequest
        {
            Email = "unit@example.com", FullName = "Có đơn vị", TemporaryPassword = "TempPass123",
            Department = unit.Name, AcademicDegree = 1, Roles = new() { staff.Id }
        }, Guid.NewGuid());

        var user = await db.Users.SingleAsync(u => u.Id == result.Id);
        Assert.Equal(unit.Id, user.UnitId);
        var profile = await db.AcademicProfiles.SingleAsync(p => p.UserId == result.Id);
        Assert.Equal("Thạc sĩ", profile.DegreeLevel);
    }

    [Fact]
    public async Task Update_ValidUser_ChangesProfileUnitDegreeAndRoles()
    {
        var (db, staff, unit) = await SeedAsync();
        var user = new User { Email = "old@example.com", FullName = "Tên cũ", Status = UserStatus.Active };
        db.Users.Add(user);
        await db.SaveChangesAsync();

        var result = await Service(db).UpdateUserAsync(user.Id, new UpdateUserRequest
        {
            FullName = "Tên mới", PhoneNumber = "0900000000", Department = unit.Code,
            AcademicDegree = 2, Roles = new() { staff.Id }
        });

        Assert.Equal("Tên mới", result.FullName);
        Assert.Equal("0900000000", result.PhoneNumber);
        Assert.Equal(unit.Name, result.Department);
        Assert.Equal("Tiến sĩ", result.AcademicDegree);
        Assert.Contains("Staff", result.Roles);
        Assert.Equal(unit.Id, (await db.Users.FindAsync(user.Id))!.UnitId);
        Assert.Equal("Tiến sĩ", (await db.AcademicProfiles.SingleAsync(a => a.UserId == user.Id)).DegreeLevel);
    }

    [Fact]
    public async Task Update_MissingNameOrUnknownRole_IsRejected()
    {
        var (db, staff, _) = await SeedAsync();
        var user = new User { Email = "user@example.com", FullName = "Tên", Status = UserStatus.Active };
        db.Users.Add(user);
        await db.SaveChangesAsync();

        await Assert.ThrowsAsync<ArgumentException>(() => Service(db).UpdateUserAsync(user.Id,
            new UpdateUserRequest { FullName = "  ", Roles = new() { staff.Id } }));
        await Assert.ThrowsAsync<KeyNotFoundException>(() => Service(db).UpdateUserAsync(user.Id,
            new UpdateUserRequest { FullName = "Tên mới", Roles = new() { 999 } }));
    }

    [Fact]
    public async Task Update_UnknownUser_ThrowsNotFound()
    {
        var (db, staff, _) = await SeedAsync();

        await Assert.ThrowsAsync<KeyNotFoundException>(() => Service(db).UpdateUserAsync(Guid.NewGuid(),
            new UpdateUserRequest { FullName = "Tên mới", Roles = new() { staff.Id } }));
    }
}
