using Microsoft.EntityFrameworkCore;
using MockCrmApi;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddDbContext<CrmDbContext>(options =>
    options.UseInMemoryDatabase("MockCrmDatabase"));

var app = builder.Build();

app.Use(async (context, next) =>
{
    if (!context.Request.Path.StartsWithSegments("/api"))
    {
        await next();
        return;
    }

    var validApiKey = app.Configuration["ApiSettings:ApiKey"];

    if (!context.Request.Headers.TryGetValue("X-Api-Key", out var extractedApiKey)
        || extractedApiKey != validApiKey)
    {
        context.Response.StatusCode = 401;
        await context.Response.WriteAsync("Unauthorized: Invalid CRM API Key");
        return;
    }

    await next();
});

app.MapGet("/api/clients/{code}", async (string code, CrmDbContext db) =>
{
    var existingClient = await db.Clients.FindAsync(code);

    if (existingClient != null)
    {
        return Results.Ok(existingClient);
    }

    var random = new Random();
    var isLegalEntity = code.Length == 8;

    var newClient = new CrmClient
    {
        Code = code,
        Status = random.NextDouble() > 0.9 ? 2 : 0
    };

    if (isLegalEntity)
    {
        string[] forms = { "Товариство з обмеженою відповідальністю", "Публічне акціонерне товариство", "Приватне підприємство" };
        string[] names = { "'Альфа'", "'МегаБуд'", "'АгроСвіт'", "'Логістика Плюс'" };

        var form = forms[random.Next(forms.Length)];
        var name = names[random.Next(names.Length)];

        newClient.FullName = $"{form} {name}";
        newClient.Email = $"info@{name.Replace("'", "").ToLower()}.ua";
        newClient.Phone = $"+38044{random.Next(1000000, 9999999)}";
    }
    else
    {
        string[] firstNames = { "Олександр", "Марія", "Іван", "Олена", "Тарас" };
        string[] lastNames = { "Шевченко", "Коваленко", "Бойко", "Мельник", "Кравченко" };
        string[] patronymics = { "Іванович", "Олегівна", "Тарасович", "Василівна", "Петрович" }; 

        newClient.FullName = $"{lastNames[random.Next(lastNames.Length)]} {firstNames[random.Next(firstNames.Length)]} {patronymics[random.Next(patronymics.Length)]}";
        newClient.Email = $"user{code.Substring(0, 4)}@gmail.com";
        newClient.Phone = $"+380{random.Next(50, 99)}{random.Next(1000000, 9999999)}";
    }

    db.Clients.Add(newClient);
    await db.SaveChangesAsync();
    return Results.Ok(newClient);
});

app.Run();