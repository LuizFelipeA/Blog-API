using Blog.Data;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddControllers();
// Making DataContext available to all the controller
builder.Services.AddDbContext<BlogDataContext>();

var app = builder.Build();
app.MapControllers();
app.Run();
