using Domain.Interface;
using Entity.Models;
using Microsoft.EntityFrameworkCore;
using MyDemo.DAL;
using MyDemo.Extensions;
using Repository.Repository;
using Repository.Services;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.ConfigureDependencyInjection(builder.Configuration);

var app = builder.Build();

// Configure middleware
app.ConfigureMiddleware();

app.Run();
