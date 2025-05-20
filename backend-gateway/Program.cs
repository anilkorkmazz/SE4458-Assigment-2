using Microsoft.AspNetCore.Mvc;
using Ocelot.DependencyInjection;
using Ocelot.Middleware;
using backend_gateway.Services;

var builder = WebApplication.CreateBuilder(args);


builder.Configuration.AddJsonFile("ocelot.json", optional: false, reloadOnChange: true);


builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();


builder.Services.AddOcelot();


builder.Services.AddHttpClient<OpenAiService>(client =>
{
    client.Timeout = TimeSpan.FromSeconds(60); // Gerekirse uzatılabilir
});
builder.Services.AddScoped<OpenAiService>();

builder.Services.AddHttpClient<AirlineApiService>();
builder.Services.AddScoped<AirlineApiService>();


builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll", builder =>
    {
        builder.AllowAnyOrigin()
               .AllowAnyMethod()
               .AllowAnyHeader();
    });
});

var app = builder.Build();


app.UseSwagger();
app.UseSwaggerUI();


app.UseRouting();
app.UseCors("AllowAll");
app.UseAuthorization();


app.UseEndpoints(endpoints =>
{
    endpoints.MapControllers();
});


await app.UseOcelot();

app.Run();
