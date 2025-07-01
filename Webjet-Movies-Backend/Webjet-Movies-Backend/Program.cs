using Asp.Versioning;
using RedisLibrary;
using StackExchange.Redis;
using Webjet_Movies_Backend;
using Webjet_Movies_Backend.ConfigOptions;
using Webjet_Movies_Backend.Mappers;
using Webjet_Movies_Backend.Middlewares;
using Webjet_Movies_Backend.Models.DTO;
using Webjet_Movies_Backend.Services;
using Webjet_Movies_Backend.Services.Interfaces;

try
{
    var builder = WebApplication.CreateBuilder(args);

    builder.Services.AddControllers();

    builder.Services.AddEndpointsApiExplorer();
    builder.Services.AddSwaggerGen();

    // I have added versioning for the api
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

    builder.Services.AddTransient<IMovieDataProvider<List<CinemaWorldMovieDetailsDTO>>, MovieDataProvider<List<CinemaWorldMovieDetailsDTO>>>();
    builder.Services.AddTransient<IMovieDataProvider<List<FilmWorldMovieDetailsDTO>>, MovieDataProvider<List<FilmWorldMovieDetailsDTO>>>();
    builder.Services.AddTransient<IRedisService, RedisService>();
    builder.Services.AddAutoMapper(typeof(MapperProfile));

    var allowOrigin = "AllowAngularApp";

    builder.Services.AddCors(options =>
    {
        // TODO add proper origin policy in prod
        options.AddPolicy(name: allowOrigin,
                          policy =>
                          {
                              policy.WithOrigins("http://localhost:4200")
                                    .AllowAnyHeader()
                                    .AllowAnyMethod();
                          });
    });

    var app = builder.Build();

    // Configure the HTTP request pipeline.
    if (app.Environment.IsDevelopment())
    {
        app.UseSwagger();
        app.UseSwaggerUI();
    }


    // TODO I commented this line since the frontend is not ready to send the key in the header yet.
    //app.UseMiddleware<ApiKeyValidationMiddleware>();

    app.UseMiddleware<ExceptionHandlingMiddleware>();

    app.UseAuthorization();

    app.UseCors(allowOrigin);

    app.MapControllers();

    app.Run();
}
catch (Exception ex)
{
    Console.WriteLine(ex.Message);
    throw;
}

