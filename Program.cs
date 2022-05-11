using System.Text;
using Blog;
using Blog.Data;
using Blog.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;

var builder = WebApplication.CreateBuilder(args);

var key = Encoding.ASCII.GetBytes(Configuration.JwtKey);

ConfigureAuthentication(builder);

ConfigureMvc(builder);

ConfigureServices(builder);


var app = builder.Build();

LoadConfiguration(app);

app.UseAuthentication(); // Who you are
app.UseAuthorization(); // What you can do
app.MapControllers();
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
    builder
    .Services
    .AddControllers()
    .ConfigureApiBehaviorOptions(options =>
    {
        options.SuppressModelStateInvalidFilter = true;
    });
}

void ConfigureServices(WebApplicationBuilder builder)
{
    // Making DataContext available to all the controller
    builder.Services.AddDbContext<BlogDataContext>();

    // Dependencie Injection
    builder.Services.AddTransient<TokenService>(); // -- Always create a new instance --
    builder.Services.AddTransient<EmailService>(); // -- Always create a new instance --
    // builder.Services.AddScoped(); // -- Create a new instance to each request --
    // builder.Services.AddSingleton(); // -- 1 per app -> Always the same instance for each app --
}