using Server.SchedulerConfig;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllersWithViews();
builder.Configuration.AddJsonFile("secret.json");

_ = builder.Configuration.GetConnectionString("ToDoConnection") 
?? throw new Exception("Connection String is null");

SchedulerConfig.Start(builder.Configuration).GetAwaiter().GetResult();

var app = builder.Build();

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseCors(c => c.AllowAnyHeader().AllowAnyMethod().AllowAnyOrigin());

app.UseAuthorization();

app.MapControllerRoute
(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}"
);

app.Run();

// dotnet ef dbcontext scaffold "Data Source=DESKTOP-3VDMESE;Initial Catalog=Ecom;Integrated Security=True;Connect Timeout=30;Encrypt=False;Trust Server Certificate=False;Application Intent=ReadWrite;Multi Subnet Failover=False" Microsoft.EntityFrameworkCore.SqlServer -o Model --force 
// Data Source=DESKTOP-3VDMESE;Initial Catalog=ToDoDB;Integrated Security=True