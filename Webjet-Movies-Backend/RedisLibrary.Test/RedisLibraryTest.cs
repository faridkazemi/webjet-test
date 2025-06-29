using Microsoft.Extensions.Logging;
using CinemaWorldDataSync.DTO;
using Moq;
using StackExchange.Redis;
using System.Text.Json;

namespace RedisLibrary.Test
{
    public class RedisLibraryTest
    {

        [Fact]
        public async Task Redis_ReturnsNull_NoValueInCache()
        {
            // arrange
            var key = "movies:cinemaWorldMovies";
            var mockDb = new Mock<IDatabase>();
            mockDb.Setup(db => db.StringGetAsync(key, It.IsAny<CommandFlags>()))
                  .ReturnsAsync("");

            var mockConnection = new Mock<IConnectionMultiplexer>();
            mockConnection.Setup(c => c.GetDatabase(It.IsAny<int>(), It.IsAny<object>()))
                          .Returns(mockDb.Object);

            var mockLogger = new Mock<ILogger<RedisService>>();

            IRedisService redisService = new RedisService(mockConnection.Object, mockLogger.Object);

            // act
            var result = await redisService.GetAsync<CinemaWorldMovieDTO>(key);

            // assert
            Assert.Null(result);
        }

        [Fact]
        public async Task Redis_ReturnsValuesFromCache()
        {
            // arrange
            var key = "cinemaWorldMovies";

            var mockDb = new Mock<IDatabase>();

            mockDb.Setup(db => db.StringGetAsync(key, It.IsAny<CommandFlags>()))
                  .ReturnsAsync("{\r\n    \"Movies\": [\r\n        {\r\n            \"Title\": \"Star Wars: Episode IV - A New Hope\",\r\n            \"Year\": \"1977\",\r\n            \"ID\": \"cw0076759\",\r\n            \"Type\": \"movie\",\r\n            \"Poster\": \"https://m.media-amazon.com/images/M/MV5BOTIyMDY2NGQtOGJjNi00OTk4LWFhMDgtYmE3M2NiYzM0YTVmXkEyXkFqcGdeQXVyNTU1NTcwOTk@._V1_SX300.jpg\"\r\n        },\r\n        {\r\n            \"Title\": \"Star Wars: Episode V - The Empire Strikes Back\",\r\n            \"Year\": \"1980\",\r\n            \"ID\": \"cw0080684\",\r\n            \"Type\": \"movie\",\r\n            \"Poster\": \"https://m.media-amazon.com/images/M/MV5BMjE2MzQwMTgxN15BMl5BanBnXkFtZTcwMDQzNjk2OQ@@._V1_SX300.jpg\"\r\n        }]\r\n}");

            var mockConnection = new Mock<IConnectionMultiplexer>();
            mockConnection.Setup(c => c.GetDatabase(It.IsAny<int>(), It.IsAny<object>()))
                          .Returns(mockDb.Object);
            var mockLogger = new Mock<ILogger<RedisService>>();
            IRedisService redisService = new RedisService(mockConnection.Object, mockLogger.Object);

            // act
            var result = await redisService.GetAsync<CinemaWorldMoviesDTO>(key);

            // assert
            Assert.NotNull(result);
            Assert.True(result.Movies.Count() == 2);
            Assert.Equal("Star Wars: Episode IV - A New Hope", result.Movies.First().Title);
        }

        [Fact]
        public async Task Redis_Add_Value()
        {
            // arrange
            var key = "cinemaWorldMovies";

            var mockDb = new Mock<IDatabase>();

            mockDb.Setup(db => db.StringSetAsync(
                It.IsAny<RedisKey>(),
                It.IsAny<RedisValue>(),
                null,
                It.IsAny<When>(),
                It.IsAny<CommandFlags>()))
            .ReturnsAsync(true);
            
            var mockConnection = new Mock<IConnectionMultiplexer>();
            mockConnection.Setup(c => c.GetDatabase(It.IsAny<int>(), It.IsAny<object>()))
                          .Returns(mockDb.Object);

            var mockLogger = new Mock<ILogger<RedisService>>();

            IRedisService redisService = new RedisService(mockConnection.Object, mockLogger.Object);
            List<CinemaWorldMovieDTO> movies = new();

            movies.Add(new CinemaWorldMovieDTO { ID = "id1", Poster = "http://abcd.com/abc.jpg", Title = "Title 1", Type = "Movie", Year = "2002" });

            var moviesDto = new CinemaWorldMoviesDTO { Movies = movies };

            var value = JsonSerializer.Serialize(moviesDto);

            // act
            await redisService.SetAsync<CinemaWorldMoviesDTO>(key, moviesDto);

            // assert

            mockDb.Verify(db =>
                db.StringSetAsync(
                    key,
                    (RedisValue)value,
                    It.IsAny<TimeSpan?>(),
                    It.IsAny<When>(),
                    It.IsAny<CommandFlags>()),
                Times.Once);
        }
    }
}