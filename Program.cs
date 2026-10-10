using EventServiceApi.Application;
using EventServiceApi.Infrastructure;
using EventServiceApi.Presentation;
using EventServiceApi.Presentation.Middleware;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddApplication();
builder.Services.AddInfrastructure();
builder.Services.AddPresentation();

if (builder.Environment.IsDevelopment())
{
    builder.Host.UseDefaultServiceProvider(options =>
    {
        options.ValidateScopes = true;
        options.ValidateOnBuild = true;
    });
}

var app = builder.Build();
/*
 * 
 Если коммент излишен, то потом при review уберу, я просто для себя написал,
 как это все работает. Вроде все просто


 Запрос клиента
     │
     ▼
┌─────────────────────────────────────┐
│  UseExceptionHandling()  ← ПЕРВЫМ   │  ← ловит ВСЁ, что ниже
├─────────────────────────────────────┤
│  UseSwagger / UseSwaggerUI          │
├─────────────────────────────────────┤
│  UseHttpsRedirection()              │
├─────────────────────────────────────┤
│  MapControllers()                   │  ← здесь возникают исключения
└─────────────────────────────────────┘
     │
     ▼
Ответ клиента
 */
app.UseMiddleware<ExceptionHandlingMiddleware>();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();

    await app.SeedDatabaseAsync();
}

app.UseHttpsRedirection();

app.MapControllers();

app.Run();
