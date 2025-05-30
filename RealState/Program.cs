﻿using DataLayer;
using Hangfire;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.FileProviders;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using RaelState.Assistant;
using RealState.Assistant;
using Serilog;
using System.Security.Claims;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;

Log.Logger = new LoggerConfiguration()
	.WriteTo.Console()
	.WriteTo.File("Logs/requests.txt", rollingInterval: RollingInterval.Day)
	.CreateLogger();

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

// JWT Config
var jwtSection = builder.Configuration.GetSection("JWT");
var jwtConfig = jwtSection.Get<SecurityTokenConfig>();
builder.Services.AddSingleton(jwtConfig);

builder.Services.AddControllers(options =>
	{
		options.RespectBrowserAcceptHeader = true; // به Accept header احترام بگذار
		//options.ReturnHttpNotAcceptable = true;    // اگر Accept پشتیبانی نشد، 406 برگردون
	})
	.AddJsonOptions(options =>
	{
		options.JsonSerializerOptions.ReferenceHandler = System.Text.Json.Serialization.ReferenceHandler.IgnoreCycles;
		options.JsonSerializerOptions.DefaultIgnoreCondition = System.Text.Json.Serialization.JsonIgnoreCondition.WhenWritingNull;
		options.JsonSerializerOptions.PropertyNamingPolicy = JsonNamingPolicy.CamelCase;
		options.JsonSerializerOptions.WriteIndented = true; // فقط برای خوانایی بهتر
	});

//builder.Services.AddControllers()
//	.AddJsonOptions(options =>
//	{
//		options.JsonSerializerOptions.ReferenceHandler = System.Text.Json.Serialization.ReferenceHandler.Preserve;
//		options.JsonSerializerOptions.WriteIndented = true; // اختیاری
//	});
//builder.Services.AddControllers()
//	.AddJsonOptions(options =>
//	{
//		options.JsonSerializerOptions.WriteIndented = true; // اختیاری
//		options.JsonSerializerOptions.ReferenceHandler = null; // حذف $id و $ref
//		options.JsonSerializerOptions.DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull; // اختیاری
//	});

// Controllers
builder.Services.AddControllers();
// Database connection
string connectionString = builder.Environment.IsDevelopment()
	? builder.Configuration.GetConnectionString("ServerDbConnection")
	: builder.Configuration.GetConnectionString("ProductionDbConnection");

//string connectionString = builder.Configuration.GetConnectionString("ProductionDbConnection");

builder.Services.AddDbContext<ApplicationDbContext>(options =>
	options.UseSqlServer(connectionString).EnableDetailedErrors());

// Swagger
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
	c.SwaggerDoc("v1", new Microsoft.OpenApi.Models.OpenApiInfo
	{
		Title = "RealState API",
		Version = "v1"
	});

	c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
	{
		Name = "Authorization",
		Type = SecuritySchemeType.ApiKey,
		Scheme = "Bearer",
		BearerFormat = "JWT",
		In = ParameterLocation.Header,
		Description = "Enter 'Bearer' [space] and then your token in the text input below.\r\n\r\nExample: \"Bearer eyJhbGciOi...\""
	});

	c.AddSecurityRequirement(new OpenApiSecurityRequirement
	{
		{
			new OpenApiSecurityScheme
			{
				Reference = new Microsoft.OpenApi.Models.OpenApiReference
				{
					Type = ReferenceType.SecurityScheme,
					Id = "Bearer"
				}
			},
			Array.Empty<string>()
		}
	});
});

//builder.Services.AddCors(options =>
//{
//	options.AddPolicy("AllowSpecific",
//		policy =>
//		{
//			policy.WithOrigins("http://localhost:5173", "http://localhost:5184", "http://localhost:3000", "http://89.40.240.13:3000",
//				"http://89.40.240.13:5173", "http://91.107.129.210:3000")
//				  .AllowAnyHeader()
//				  .AllowAnyMethod()
//				  .AllowCredentials();
//		});
//});
//builder.Services.AddCors(options =>
//{
//	options.AddPolicy("AllowSpecific",
//		policy =>
//		{
//			policy.WithOrigins("http://localhost:5173")
//				  .AllowAnyHeader()
//				  .AllowAnyMethod()
//				  .AllowCredentials();
//		});
//});
builder.Services.AddCors(options =>
{
	options.AddDefaultPolicy(policy =>
	{
		policy
			.SetIsOriginAllowed(_ => true) // فقط برای تست!
			.AllowAnyHeader()
			.AllowAnyMethod()
			.AllowCredentials();
	});
});
// In middleware:

// Repositories and services
builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();
builder.Services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();
builder.Services.AddScoped<JwtTokenService>();
builder.Services.AddScoped<TokenCleanupTask>();
builder.Services.AddScoped<JobScheduler>();
builder.Services.AddDistributedMemoryCache();

builder.Services.AddSession(options =>
{
	options.IdleTimeout = TimeSpan.FromMinutes(30);
	options.Cookie.HttpOnly = true;
	options.Cookie.IsEssential = true;
});

// JWT Authentication
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
	.AddJwtBearer(options =>
	{
		options.TokenValidationParameters = new TokenValidationParameters
		{
			ValidateIssuer = true,
			ValidateAudience = true,
			ValidateLifetime = true,
			ValidateIssuerSigningKey = true,
			ValidIssuer = jwtConfig.Issuer,
			ValidAudience = jwtConfig.Audience,
			IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtConfig.Key)),
			NameClaimType = ClaimTypes.NameIdentifier // این خط را اضافه کنید تا `NameIdentifier` به درستی ست شود
		};

		options.Events = new JwtBearerEvents
		{
			OnMessageReceived = context =>
			{
				var token = context.Request.Headers["Authorization"].FirstOrDefault()?.Split(" ").Last();
				if (string.IsNullOrEmpty(token))
				{
					token = context.Request.Cookies["jwt"]; // اگر توکن در هدر نبود از کوکی بخون
				}

				if (!string.IsNullOrEmpty(token))
				{
					context.Token = token;
				}

				return Task.CompletedTask;
			}
		};
	});


// Hangfire
builder.Services.AddHangfire(config => config.UseSqlServerStorage(connectionString));
builder.Services.AddHangfireServer();
builder.Services.AddSignalR();

var app = builder.Build();


// Middlewares
//app.UseCors("AllowSpecific");
app.UseCors();
//app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseStaticFiles(new StaticFileOptions
{
	FileProvider = new PhysicalFileProvider(Path.Combine(builder.Environment.ContentRootPath, "images")),
	RequestPath = "/images"
});
app.UseStaticFiles(new StaticFileOptions
{
	FileProvider = new PhysicalFileProvider(Path.Combine(Directory.GetCurrentDirectory(), "Logs")),
	RequestPath = "/logs"
});
app.UseMiddleware<LoggingMiddleware>();
app.UseRouting();
app.UseSession();
app.UseAuthentication();
app.UseAuthorization();

//if (app.Environment.IsDevelopment())
//{ 
app.UseDeveloperExceptionPage();
app.UseSwagger();
//app.UseSwaggerUI();
app.UseSwaggerUI(options =>
{
	options.SwaggerEndpoint("/swagger/v1/swagger.json", "My API V1");
	//options.InjectJavascript("/swagger/swagger-authtoken.js");
});
//}

app.MapControllers();
app.Run();
