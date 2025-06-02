using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using backend.data;
using backend.entity;
using backend.enums;
using backend.mock;
using backend.service;
using backend.service.access.access_request;
using backend.service.access.gate;
using backend.service.access.gate.command_sender;
// using backend.signalR;
using Microsoft.OpenApi.Models;

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

// Adăugare HttpClient și servicii mock
builder.Services.AddHttpClient();
//builder.Services.AddHostedService<MockMobileApp>();
builder.Services.AddMockEspGateDevice();

// Adăugare servicii pentru accesarea porții
builder.Services.AddScoped<IAccessRequestService, AccessRequestService>();
builder.Services.AddScoped<IGateService, GateService>();

// Configurare condiționată a IGateCommandSender bazată pe mediu
if (builder.Environment.IsDevelopment())
{
    // În mediul de dezvoltare, utilizăm configurarea mock
    builder.Services.AddScoped<IGateCommandSender, EspGateCommandSender>(sp =>
    {
        var httpClient = sp.GetRequiredService<HttpClient>();
        var config = sp.GetRequiredService<IConfiguration>();
        var logger = sp.GetRequiredService<ILogger<EspGateCommandSender>>();
        
        // Asigurați-vă că appsettings.Development.json are GateController:Url configurat
        return new EspGateCommandSender(httpClient, config, logger);
    });
}
else
{
    // În mediul de producție, utilizăm implementarea standard
    builder.Services.AddScoped<IGateCommandSender, EspGateCommandSender>();
}

// add signalR for real time 
builder.Services.AddSignalR();
builder.WebHost.ConfigureKestrel(serverOptions =>
{
    serverOptions.ListenAnyIP(5000);
});


// add swagger
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Name = "Authorization",
        Type = SecuritySchemeType.Http,
        Scheme = "bearer",
        BearerFormat = "JWT",
        In = ParameterLocation.Header,
        Description = "Introdu un token JWT pentru autorizare"
    });

    c.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference { Type = ReferenceType.SecurityScheme, Id = "Bearer" }
            },
            new string[] { }
        }
    });
});

var app = builder.Build();

// Configurare middleware pentru mock ESP în mediul de dezvoltare
if (app.Environment.IsDevelopment())
{
    // Setup mock ESP32 endpoints on a different route prefix
    var espMockApp = app.Map("/mock-esp", builder => builder.UseMockEspGateDevice());
}

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
        VehicleNumber = "ABQ141"
    };
    var access_log2 = new AccessLog
    {
        EmployeeId = 11,
        Timestamp = new DateTime(2025,5,17,19,30,0),
        Direction = AccessDirection.Exit,
        Method = AccessMethod.Vehicle,
        VehicleNumber = "ABQ141"
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