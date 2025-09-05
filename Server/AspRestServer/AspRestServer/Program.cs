//var builder = WebApplication.CreateBuilder(args);

//// Add services to the container.

//builder.Services.AddControllers();
//// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
//builder.Services.AddEndpointsApiExplorer();
//builder.Services.AddSwaggerGen();

//var app = builder.Build();

//// Configure the HTTP request pipeline.
//if (app.Environment.IsDevelopment())
//{
//    app.UseSwagger();
//    app.UseSwaggerUI();
//}

//app.UseHttpsRedirection();        

//app.UseAuthorization();

//app.MapControllers();

//app.Run();
// Program.cs (ASP.NET Core Web API, .NET 8)
using AspRestServer.Models;
using AspRestServer.Services;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// DB

builder.Services.AddDbContext<MyAppDbContext>(opts => {
    opts.UseSqlServer(builder.Configuration["ConnectionStrings:ProductConnection"]);
    opts.EnableSensitiveDataLogging(true);
});

// Services
builder.Services.AddScoped<DbManager>();
builder.Services.AddHttpClient<OpenAiService>();

builder.Services.AddControllers();

// CORS (개발 편의)
builder.Services.AddCors(o => o.AddPolicy("AllowAll",
    p => p.AllowAnyOrigin().AllowAnyHeader().AllowAnyMethod()));

// Swagger (선택)
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

app.UseCors("AllowAll");
app.UseSwagger();
app.UseSwaggerUI();

app.MapControllers();

app.Run();
