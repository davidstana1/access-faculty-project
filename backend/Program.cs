using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using backend.data;
using backend.entity;
using backend.enums;
using backend.service;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

builder.Services.AddIdentity<User, IdentityRole>(options =>
{
    options.Password.RequireDigit = true;
    options.Password.RequireLowercase = true;
    options.Password.RequireNonAlphanumeric = false;
    options.Password.RequireUppercase = true;
    options.Password.RequiredLength = 6;
})
.AddEntityFrameworkStores<ApplicationDbContext>()
.AddDefaultTokenProviders();

builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(options =>
{
    options.SaveToken = true;
    options.RequireHttpsMetadata = false;
    options.TokenValidationParameters = new TokenValidationParameters()
    {
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidateLifetime = true,
        ValidateIssuerSigningKey = true,
        ClockSkew = TimeSpan.Zero,
        ValidAudience = builder.Configuration["JWT:ValidAudience"],
        ValidIssuer = builder.Configuration["JWT:ValidIssuer"],
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["JWT:Secret"]))
    };
});

// add services
builder.Services.AddScoped<ITokenService, TokenService>();
// builder.Services.AddScoped<IEmployeeService, EmployeeService>();
// builder.Services.AddScoped<IGateService, GateService>();

// auth policies
builder.Services.AddAuthorization(options =>
{
    options.AddPolicy("HROnly", policy => policy.RequireRole("HR"));
    options.AddPolicy("GatePersonnelOnly", policy => policy.RequireRole("GatePersonnel"));
    options.AddPolicy("HROrGatePersonnel", policy => 
        policy.RequireRole("HR", "GatePersonnel"));
});

// add controllers and cors
builder.Services.AddControllers();
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAngularApp", corsBuilder =>
    {
        corsBuilder.WithOrigins("http://localhost:4200")
            .AllowAnyHeader()
            .AllowAnyMethod()
            .AllowCredentials();
    });
});


// add signalR for real time 
builder.Services.AddSignalR();

// add swagger
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// http pipeline
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
    app.UseDeveloperExceptionPage();
}
else
{
    app.UseExceptionHandler("/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseCors("AllowAngularApp");

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();
// app.MapHub<GateHub>("/gatehub");

// init roles
using (var scope = app.Services.CreateScope())
{
    var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole>>();
    var userManager = scope.ServiceProvider.GetRequiredService<UserManager<User>>();
    var context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

    // create roles if non-existing
    string[] roleNames = { "HR", "GatePersonnel", "Manager" };
    foreach (var roleName in roleNames)
    {
        var roleExist = await roleManager.RoleExistsAsync(roleName);
        if (!roleExist)
        {
            await roleManager.CreateAsync(new IdentityRole(roleName));
        }
    }

    // create hr user
    var adminUser = await userManager.FindByNameAsync("hr@company.com");
    if (adminUser == null)
    {
        var user = new User
        {
            UserName = "hr@company.com",
            Email = "hr@company.com",
            FirstName = "Admin",
            LastName = "HR",
            EmailConfirmed = true
        };

        var result = await userManager.CreateAsync(user, "Hr123456!");
        if (result.Succeeded)
        {
            await userManager.AddToRoleAsync(user, "HR");
        }
    }

    // create gate user
    var gateUser = await userManager.FindByNameAsync("gate@company.com");
    if (gateUser == null)
    {
        var user = new User
        {
            UserName = "gate@company.com",
            Email = "gate@company.com",
            FirstName = "Gate",
            LastName = "Operator",
            EmailConfirmed = true
        };

        var result = await userManager.CreateAsync(user, "Gate123456!");
        if (result.Succeeded)
        {
            await userManager.AddToRoleAsync(user, "GatePersonnel");
        }
    }

    // var division = new Division
    // {
    //     Name = "IT Department"
    // };
    // context.Divisions.Add(division);
    // await context.SaveChangesAsync(); 

    var manager = await userManager.FindByEmailAsync("manager@company.com");
    if (manager == null)
    {
        var user = new User
        {
            UserName = "manager@company.com",
            Email = "manager@company.com",
            FirstName = "Manager",
            LastName = "Operator",
            EmailConfirmed = true
        };

        var result = await userManager.CreateAsync(user, "Manager123456!");
        if (result.Succeeded)
        {
            await userManager.AddToRoleAsync(user, "Manager");
        }
    }
    
    var manager2 = await userManager.FindByEmailAsync("manager2@company.com");
    if (manager2 == null)
    {
        var user2 = new User
        {
            UserName = "manager2@company.com",
            Email = "manager2@company.com",
            FirstName = "Manager2",
            LastName = "Operator",
            EmailConfirmed = true
        };

        var result = await userManager.CreateAsync(user2, "Manager123!");
        if (result.Succeeded)
        {
            await userManager.AddToRoleAsync(user2, "Manager");
        }
    }
    var access_log = new AccessLog
    {
        EmployeeId = 11,
        Timestamp = DateTime.Now,
        Direction = AccessDirection.Entry,
        Method = AccessMethod.Vehicle,
        VehicleNumber = "ABQ141",
        IsWithinSchedule = true,
        WasOverridden = true,
        OverrideUserId = "2"
    };
    var access_log2 = new AccessLog
    {
        EmployeeId = 11,
        Timestamp = new DateTime(2025,5,17,19,30,0),
        Direction = AccessDirection.Exit,
        Method = AccessMethod.Vehicle,
        VehicleNumber = "ABQ141",
        IsWithinSchedule = true,
        WasOverridden = true,
        OverrideUserId = "2"
    };
        
    context.AccessLogs.Add(access_log);
    await context.SaveChangesAsync();

    context.AccessLogs.Add(access_log2);
    await context.SaveChangesAsync();
//     var employee = new Employee
//     {
//         FirstName = "John",
//         LastName = "Doe",
//         CNP = "1234567890123",
//         BadgeNumber = "12345",
//         PhotoUrl = "http://example.com/photo.jpg",
//         DivisionId = 1,
//         BluetoothSecurityCode = "BluetoothCode",
//         VehicleNumber = "ABC123",
//         IsAccessEnabled = true,
//         ApprovedById = "bc840f50-408e-4384-87b9-e4289cbf1999",
//         ApprovalDate = DateTime.Now,
//         AccessSchedules = new List<AccessSchedule>(),
//         AccessLogs = new List<AccessLog>() 
//     };
//
// // Salvează entitatea
//     context.Employees.Add(employee);
//     await context.SaveChangesAsync();
}

app.Run();