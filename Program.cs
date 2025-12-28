var builder = WebApplication.CreateBuilder(args);
builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents();

var app = builder.Build();

app.UseStaticFiles();
app.UseAntiforgery();

app.MapRazorComponents<Sudoku16x16Solver.Components.App>()
    .AddInteractiveServerRenderMode();

app.Run();
