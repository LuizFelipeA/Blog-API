using System.IO.Compression;
using System.Text;
using System.Text.Json.Serialization;
using Blog;
using Blog.Data;
using Blog.Repositories;
using Blog.Repositories.Interfaces;
using Blog.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.ResponseCompression;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;

var builder = WebApplication.CreateBuilder(args);

var key = Encoding.ASCII.GetBytes(Configuration.JwtKey);

ConfigureAuthentication(builder);

ConfigureMvc(builder);

ConfigureServices(builder);

ConfigureDocumentation(builder);

var app = builder.Build();

LoadConfiguration(app);

app.UseHttpsRedirection();
app.UseAuthentication(); // Who you are
app.UseAuthorization(); // What you can do

app.UseResponseCompression(); // Enable use of response compression

app.MapControllers();
app.UseStaticFiles(); // Allows app to store files in wwwroot

if(app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.Run();

void LoadConfiguration(WebApplication app)
{
    Configuration.JwtKey = app.Configuration.GetValue<string>(key: "JwtKey");
    
    var smtp = new Configuration.SmtpConfiguration();
    app.Configuration.GetSection(key: "Smtp").Bind(smtp);
    Configuration.Smtp = smtp;
}

void ConfigureAuthentication(WebApplicationBuilder builder)
{
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
}

void ConfigureMvc(WebApplicationBuilder builder)
{
    builder.Services.AddMemoryCache(); // Support to memory cache

    // Adding response compression to our app with GZIP provider
    builder.Services.AddResponseCompression(options =>
    {
        options.Providers.Add<GzipCompressionProvider>();
    });
    builder.Services.Configure<GzipCompressionProviderOptions>(options => 
    {
        options.Level = CompressionLevel.Optimal;
    });

    builder.Services
    .AddControllers()
    .ConfigureApiBehaviorOptions(options =>
    {
        options.SuppressModelStateInvalidFilter = true;
    })
    .AddJsonOptions(jsonOption =>
    {
        jsonOption.JsonSerializerOptions.ReferenceHandler = ReferenceHandler.IgnoreCycles;
        // Property will be ignore if it is null
        jsonOption.JsonSerializerOptions.DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingDefault;
    });
}

void ConfigureServices(WebApplicationBuilder builder)
{
    var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");

    // Making DataContext available to all the controller
    builder.Services.AddDbContext<BlogDataContext>(
        options => options.UseSqlServer(connectionString));

    // Dependencie Injection
    builder.Services.AddTransient<TokenService>(); // -- Always create a new instance --
    builder.Services.AddTransient<EmailService>(); // -- Always create a new instance --
    builder.Services.AddTransient<ICategoryRepository, CategoryRepository>(); // -- Always create a new instance --
    builder.Services.AddTransient<ICategoryRepository, CategoryRepository>(); // -- Always create a new instance --
    // builder.Services.AddScoped(); // -- Create a new instance to each request --
    // builder.Services.AddSingleton(); // -- 1 per app -> Always the same instance for each app --
}

void ConfigureDocumentation(WebApplicationBuilder builder)
{
    builder.Services.AddEndpointsApiExplorer(); // Add Swagger

    builder.Services.AddSwaggerGen(); // Generate Swagger
}