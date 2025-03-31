var builder = WebApplication.CreateBuilder(args);

// Afegir serveis a l'aplicació
builder.Services.AddControllersWithViews();

var app = builder.Build();

// Configuració del pipeline de peticions HTTP
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();
app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Barcode}/{action=Index}/{id?}");

app.Run();
