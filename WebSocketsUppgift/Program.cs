var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddRazorPages();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
}

app.UseStaticFiles();

app.UseRouting();

//Telling our project to use WebSockets
app.UseWebSockets();

app.UseAuthorization();

app.MapControllers();

app.MapRazorPages();

app.Run();
