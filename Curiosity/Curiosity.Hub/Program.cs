using Curiosity.Hub;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddRazorPages();
builder.Services.AddSignalR();
builder.Services.AddControllers();
builder.Services.Configure<IISServerOptions>(options =>
       {
           options.MaxRequestBodySize = 20 * 1024 * 1024; // 20MB
       });
builder.Services.AddCors(options =>
{
    options.AddPolicy(name: "MyOriginPolicy",
                      policy =>
                      {
                          policy.WithOrigins("https://curiosity.taiseiue.jp",
                                              "https://rover-hub.taiseiue.jp");
                      });
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();

app.UseRouting();
app.MapHub<CommandHub>("/command");

app.UseDefaultFiles();
app.UseStaticFiles();
app.UseAuthorization();

app.MapStaticAssets();
app.MapControllers();
app.MapRazorPages()
   .WithStaticAssets();

app.Run();
