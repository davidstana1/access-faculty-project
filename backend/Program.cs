using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using backend.data;
using backend.entity;
using backend.service;

var builder = WebApplication.CreateBuilder(args);

// Adăugare DbContext
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

// Adăugare Identity
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

// Adăugare autentificare JWT
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

// Adăugare servicii personalizate
builder.Services.AddScoped<ITokenService, TokenService>();
// builder.Services.AddScoped<IEmployeeService, EmployeeService>();
// builder.Services.AddScoped<IGateService, GateService>();

// Adăugare politici de autorizare
builder.Services.AddAuthorization(options =>
{
    options.AddPolicy("HROnly", policy => policy.RequireRole("HR"));
    options.AddPolicy("GatePersonnelOnly", policy => policy.RequireRole("GatePersonnel"));
    options.AddPolicy("HROrGatePersonnel", policy => 
        policy.RequireRole("HR", "GatePersonnel"));
});

// Adăugare controllere și configurare CORS
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


// Adăugare SignalR pentru comunicare în timp real
builder.Services.AddSignalR();

// Adăugare Swagger
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Configurare pipeline HTTP
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

// Inițializare roluri
using (var scope = app.Services.CreateScope())
{
    var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole>>();
    var userManager = scope.ServiceProvider.GetRequiredService<UserManager<User>>();
    
    // Creează rolurile dacă nu există
    string[] roleNames = { "HR", "GatePersonnel" };
    foreach (var roleName in roleNames)
    {
        var roleExist = await roleManager.RoleExistsAsync(roleName);
        if (!roleExist)
        {
            await roleManager.CreateAsync(new IdentityRole(roleName));
        }
    }
    
    // Creează un utilizator HR implicit dacă nu există
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
    
    // Creează un utilizator pentru poartă
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
}

app.Run();