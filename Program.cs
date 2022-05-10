using System.Text;
using Blog;
using Blog.Data;
using Blog.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;

var builder = WebApplication.CreateBuilder(args);

var key = Encoding.ASCII.GetBytes(Configuration.JwtKey);
builder.Services.AddAuthentication(
    // Authentication
    x => 
    {
        x.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
        x.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
    }).AddJwtBearer(
        // Authotization
        x => 
        {
            x.TokenValidationParameters = new TokenValidationParameters
            {
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = new SymmetricSecurityKey(key),
                ValidateIssuer = false,
                ValidateAudience = false
            };
        });

builder
    .Services
    .AddControllers()
    .ConfigureApiBehaviorOptions(options =>
    {
        options.SuppressModelStateInvalidFilter = true;
    });
    
// Making DataContext available to all the controller
builder.Services.AddDbContext<BlogDataContext>();

// Dependencie Injection
builder.Services.AddTransient<TokenService>(); // -- Always create a new instance --
// builder.Services.AddScoped(); // -- Create a new instance to each request --
// builder.Services.AddSingleton(); // -- 1 per app -> Always the same instance for each app --

var app = builder.Build();

app.UseAuthentication(); // Who you are
app.UseAuthorization(); // What you can do

app.MapControllers();
app.Run();
