using Asp.Versioning;
using RedisLibrary;
using StackExchange.Redis;
using Webjet_Movies_Backend;
using Webjet_Movies_Backend.ConfigOptions;
using Webjet_Movies_Backend.Models.DTO;
using Webjet_Movies_Backend.Services;
using Webjet_Movies_Backend.Services.Interfaces;

try
{
    var builder = WebApplication.CreateBuilder(args);

    builder.Services.AddControllers();

    builder.Services.AddEndpointsApiExplorer();
    builder.Services.AddSwaggerGen();

    builder.Services.AddApiVersioning(options =>
    {
        options.DefaultApiVersion = new Asp.Versioning.ApiVersion(1);
        options.ReportApiVersions = true;
        options.ApiVersionReader = ApiVersionReader.Combine(
            new UrlSegmentApiVersionReader(),
            new HeaderApiVersionReader("X-Api-Version"));
    }).AddMvc();

    RedisConfigOption redisOptions = new();

    builder.Configuration.GetSection(nameof(RedisConfigOption))
        .Bind(redisOptions);

    builder.Services.AddSingleton<IConnectionMultiplexer>(x =>
    {
        // For this test project I haven't added any password for connecting to the redis.
        // But in the real world project connection needs to be secure
        return ConnectionMultiplexer.Connect(redisOptions.ConnectionString);
    });

    builder.Services.AddTransient<IMovieDataProvider<CinemaWorldMoviesDTO>, MovieDataProvider<CinemaWorldMoviesDTO>>();
    builder.Services.AddTransient<IRedisService, RedisService>();


    var allowOrigin = "AllowAngularApp";

    builder.Services.AddCors(options =>
    {
        options.AddPolicy(name: allowOrigin,
                          policy =>
                          {
                              policy.AllowAnyOrigin()//WithOrigins("http://localhost:4200")
                                    .AllowAnyHeader()
                                    .AllowAnyMethod();
                              //.AllowCredentials(); 
                          });
    });

    var app = builder.Build();

    // Configure the HTTP request pipeline.
    if (app.Environment.IsDevelopment())
    {
        app.UseSwagger();
        app.UseSwaggerUI();
    }

    //app.UseHttpsRedirection();

    app.UseAuthorization();


    //app.UseMiddleware<RequestLoggingMiddleware>();
    app.UseCors(allowOrigin);

    app.MapControllers();

    app.Run();
}
catch (Exception ex)
{
    Console.WriteLine(ex.Message);
    throw;
}






//// Program.cs - TEMPORARY DIAGNOSTIC VERSION
//// MAKE SURE TO BACK UP YOUR ORIGINAL Program.cs FIRST!

//using System;

//var builder = WebApplication.CreateBuilder(args);

//// TEMPORARY: Explicitly configure Kestrel to bind to port 80 using UseUrls
//// This provides another layer of certainty for Kestrel's binding configuration
//builder.WebHost.UseUrls("http://+:80");

//var app = builder.Build();

//// TEMPORARY: Remove ALL your application's middleware and routing for this test
//// For example, comment out or remove these lines if they exist:
//// app.UseHttpsRedirection();
//// app.UseRouting();
//// app.UseMiddleware<RequestLoggingMiddleware>();
//// app.UseCors(MyAllowSpecificOrigins);
//// app.MapControllers(); // Or app.UseEndpoints, app.UseSwagger, app.UseSwaggerUI, etc.

//// TEMPORARY: Add a very simple endpoint to confirm Kestrel is working
//app.MapGet("/", () => "Hello from Docker Kestrel!");
//app.MapGet("/api/v1/movies", () => "Movies from Docker Kestrel!"); // Use the path Nginx expects

//// Add a Console.WriteLine to see if this part of the code is reached
//Console.WriteLine("Application is about to start running Kestrel and listen on port 80...");

//try
//{
//    app.Run(); // This is the blocking call where Kestrel starts listening
//    // This line below should ideally not be reached if Kestrel is running
//    Console.WriteLine("Application has finished running Kestrel (this should not happen normally).");
//}
//catch (Exception ex)
//{
//    // This catch block is to ensure any very early startup exceptions are visible
//    Console.Error.WriteLine($"FATAL EXCEPTION CAUGHT IN PROGRAM.CS: {ex.Message}");
//    Console.Error.WriteLine(ex.ToString()); // Print full stack trace
//}

//Console.WriteLine("Application process ending."); // Final check if process terminates

