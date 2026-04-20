using Lanchonete.Infra.EF;
using Lanchonete.Infra.EF.DbCOntext;
using Microsoft.EntityFrameworkCore;
using Scalar.AspNetCore;
using SistemaControle.Infra.DI;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddOpenApi();
builder.Services.AddMemoryCache();
builder.Services.AddServiceCollection(builder.Configuration);

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.MapScalarApiReference();
}

app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();

using (var scope = app.Services.CreateScope())
{
    var context = scope.ServiceProvider.GetRequiredService<LanchoneteContext>();

    await context.Database.MigrateAsync();
    await DbSeeder.SeedAsync(context);
}

app.Run();