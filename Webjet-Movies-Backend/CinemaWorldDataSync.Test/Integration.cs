using Castle.Core.Logging;
using CinemaWorldDataSync.DTO;
using CinemaWorldDataSync.Services;
using CinemaWorldDataSync.Services.Interfaces;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Moq;
using RedisLibrary;
using StackExchange.Redis;
using System.Net;
using System.Net.Http;
using System.Text.Json;

namespace CinemaWorldDataSync.Test
{
    public class Integration : BaseTest
    {
        // TODO more tests to be added

        [Fact]
        public async Task DataSync_FullProcess_ApisAreUp()
        {
            try
            {
                Setup();

                // arrange
                // TODO better to move the long strings to txt files to keep it clear and more readable.
                var moviesApiCallFakeResponse = "{\r\n    \"Movies\": [\r\n        {\r\n            \"Title\": \"Star Wars: Episode IV - A New Hope\",\r\n            \"Year\": \"1977\",\r\n            \"ID\": \"cw0076759\",\r\n            \"Type\": \"movie\",\r\n            \"Poster\": \"https://m.media-amazon.com/images/M/MV5BOTIyMDY2NGQtOGJjNi00OTk4LWFhMDgtYmE3M2NiYzM0YTVmXkEyXkFqcGdeQXVyNTU1NTcwOTk@._V1_SX300.jpg\"\r\n        },\r\n        {\r\n            \"Title\": \"Star Wars: Episode V - The Empire Strikes Back\",\r\n            \"Year\": \"1980\",\r\n            \"ID\": \"cw0080684\",\r\n            \"Type\": \"movie\",\r\n            \"Poster\": \"https://m.media-amazon.com/images/M/MV5BMjE2MzQwMTgxN15BMl5BanBnXkFtZTcwMDQzNjk2OQ@@._V1_SX300.jpg\"\r\n        }\r\n    ]\r\n}";
                var firstMovieApiCallFakeResponse = "{\r\n    \"Title\": \"Star Wars: Episode IV - A New Hope\",\r\n    \"Year\": \"1977\",\r\n    \"Rated\": \"PG\",\r\n    \"Released\": \"25 May 1977\",\r\n    \"Runtime\": \"121 min\",\r\n    \"Genre\": \"Action, Adventure, Fantasy\",\r\n    \"Director\": \"George Lucas\",\r\n    \"Writer\": \"George Lucas\",\r\n    \"Actors\": \"Mark Hamill, Harrison Ford, Carrie Fisher, Peter Cushing\",\r\n    \"Plot\": \"Luke Skywalker joins forces with a Jedi Knight, a cocky pilot, a wookiee and two droids to save the galaxy from the Empire's world-destroying battle-station, while also attempting to rescue Princess Leia from the evil Darth Vader.\",\r\n    \"Language\": \"English\",\r\n    \"Country\": \"USA\",\r\n    \"Awards\": \"Won 6 Oscars. Another 48 wins & 28 nominations.\",\r\n    \"Poster\": \"https://m.media-amazon.com/images/M/MV5BOTIyMDY2NGQtOGJjNi00OTk4LWFhMDgtYmE3M2NiYzM0YTVmXkEyXkFqcGdeQXVyNTU1NTcwOTk@._V1_SX300.jpg\",\r\n    \"Metascore\": \"92\",\r\n    \"Rating\": \"8.7\",\r\n    \"Votes\": \"915,459\",\r\n    \"ID\": \"cw0076759\",\r\n    \"Type\": \"movie\",\r\n    \"Price\": \"123.5\"\r\n}";
                var secondMovieApiCallFakeResponse = "{\r\n    \"Title\": \"Star Wars: Episode V - The Empire Strikes Back\",\r\n    \"Year\": \"1980\",\r\n    \"Rated\": \"PG\",\r\n    \"Released\": \"20 Jun 1980\",\r\n    \"Runtime\": \"124 min\",\r\n    \"Genre\": \"Action, Adventure, Fantasy\",\r\n    \"Director\": \"Irvin Kershner\",\r\n    \"Writer\": \"Leigh Brackett (screenplay), Lawrence Kasdan (screenplay), George Lucas (story by)\",\r\n    \"Actors\": \"Mark Hamill, Harrison Ford, Carrie Fisher, Billy Dee Williams\",\r\n    \"Plot\": \"After the Rebel base on the icy planet Hoth is taken over by the Empire, Han, Leia, Chewbacca, and C-3PO flee across the galaxy from the Empire. Luke travels to the forgotten planet of Dagobah to receive training from the Jedi master Yoda, while Vader endlessly pursues him.\",\r\n    \"Language\": \"English\",\r\n    \"Country\": \"USA\",\r\n    \"Awards\": \"Won 1 Oscar. Another 19 wins & 18 nominations.\",\r\n    \"Poster\": \"https://m.media-amazon.com/images/M/MV5BMjE2MzQwMTgxN15BMl5BanBnXkFtZTcwMDQzNjk2OQ@@._V1_SX300.jpg\",\r\n    \"Metascore\": \"80\",\r\n    \"Rating\": \"8.8\",\r\n    \"Votes\": \"842,451\",\r\n    \"ID\": \"cw0080684\",\r\n    \"Type\": \"movie\",\r\n    \"Price\": \"13.5\"\r\n}";

                var mockHttpClientFactory = new Mock<IHttpClientFactory>();
                var httpClientMock = GetMockHttpClient(HttpStatusCode.OK, moviesApiCallFakeResponse, firstMovieApiCallFakeResponse, secondMovieApiCallFakeResponse);

                mockHttpClientFactory.Setup(f => f.CreateClient(It.IsAny<string>()))
                    .Returns(httpClientMock);

                IRedisService inMemoryRedisService = new InMemoryRedis();

                ServiceCollection.ReplaceWithMock<IHttpClientFactory>(mockHttpClientFactory);
                ServiceCollection.ReplaceWith<IRedisService>(inMemoryRedisService);

                var syncDataService = ServiceCollection.BuildServiceProvider().GetRequiredService<ISyncDataService>();

                using var cts = new CancellationTokenSource();

                await syncDataService.RunAsync(cts.Token, 0);

                var moviesInRedis = await inMemoryRedisService.GetAsync<List<CinemaWorldMovieDetailsDTO>>("movies:cinemaWorldMovies");

                Assert.NotNull(moviesInRedis);
                Assert.Equal(2, moviesInRedis.Count);

                var test = JsonSerializer.Serialize(moviesInRedis);
                var a = 0;
            }
            catch(Exception ex)
            {
                Console.WriteLine(ex.ToString());
                throw;
            }
        }

        [Fact]
        public async Task DataSync_FullProcess_ApisAreDown_ReadFromRedis()
        {
            try
            {
                Setup();

                // arrange
                var moviesApiCallFakeResponse = "{\r\n    \"Movies\": [\r\n        {\r\n            \"Title\": \"Star Wars: Episode IV - A New Hope\",\r\n            \"Year\": \"1977\",\r\n            \"ID\": \"cw0076759\",\r\n            \"Type\": \"movie\",\r\n            \"Poster\": \"https://m.media-amazon.com/images/M/MV5BOTIyMDY2NGQtOGJjNi00OTk4LWFhMDgtYmE3M2NiYzM0YTVmXkEyXkFqcGdeQXVyNTU1NTcwOTk@._V1_SX300.jpg\"\r\n        },\r\n        {\r\n            \"Title\": \"Star Wars: Episode V - The Empire Strikes Back\",\r\n            \"Year\": \"1980\",\r\n            \"ID\": \"cw0080684\",\r\n            \"Type\": \"movie\",\r\n            \"Poster\": \"https://m.media-amazon.com/images/M/MV5BMjE2MzQwMTgxN15BMl5BanBnXkFtZTcwMDQzNjk2OQ@@._V1_SX300.jpg\"\r\n        }\r\n    ]\r\n}";
                var firstMovieApiCallFakeResponse = "{\r\n    \"Title\": \"Star Wars: Episode IV - A New Hope\",\r\n    \"Year\": \"1977\",\r\n    \"Rated\": \"PG\",\r\n    \"Released\": \"25 May 1977\",\r\n    \"Runtime\": \"121 min\",\r\n    \"Genre\": \"Action, Adventure, Fantasy\",\r\n    \"Director\": \"George Lucas\",\r\n    \"Writer\": \"George Lucas\",\r\n    \"Actors\": \"Mark Hamill, Harrison Ford, Carrie Fisher, Peter Cushing\",\r\n    \"Plot\": \"Luke Skywalker joins forces with a Jedi Knight, a cocky pilot, a wookiee and two droids to save the galaxy from the Empire's world-destroying battle-station, while also attempting to rescue Princess Leia from the evil Darth Vader.\",\r\n    \"Language\": \"English\",\r\n    \"Country\": \"USA\",\r\n    \"Awards\": \"Won 6 Oscars. Another 48 wins & 28 nominations.\",\r\n    \"Poster\": \"https://m.media-amazon.com/images/M/MV5BOTIyMDY2NGQtOGJjNi00OTk4LWFhMDgtYmE3M2NiYzM0YTVmXkEyXkFqcGdeQXVyNTU1NTcwOTk@._V1_SX300.jpg\",\r\n    \"Metascore\": \"92\",\r\n    \"Rating\": \"8.7\",\r\n    \"Votes\": \"915,459\",\r\n    \"ID\": \"cw0076759\",\r\n    \"Type\": \"movie\",\r\n    \"Price\": \"123.5\"\r\n}";
                var secondMovieApiCallFakeResponse = "{\r\n    \"Title\": \"Star Wars: Episode V - The Empire Strikes Back\",\r\n    \"Year\": \"1980\",\r\n    \"Rated\": \"PG\",\r\n    \"Released\": \"20 Jun 1980\",\r\n    \"Runtime\": \"124 min\",\r\n    \"Genre\": \"Action, Adventure, Fantasy\",\r\n    \"Director\": \"Irvin Kershner\",\r\n    \"Writer\": \"Leigh Brackett (screenplay), Lawrence Kasdan (screenplay), George Lucas (story by)\",\r\n    \"Actors\": \"Mark Hamill, Harrison Ford, Carrie Fisher, Billy Dee Williams\",\r\n    \"Plot\": \"After the Rebel base on the icy planet Hoth is taken over by the Empire, Han, Leia, Chewbacca, and C-3PO flee across the galaxy from the Empire. Luke travels to the forgotten planet of Dagobah to receive training from the Jedi master Yoda, while Vader endlessly pursues him.\",\r\n    \"Language\": \"English\",\r\n    \"Country\": \"USA\",\r\n    \"Awards\": \"Won 1 Oscar. Another 19 wins & 18 nominations.\",\r\n    \"Poster\": \"https://m.media-amazon.com/images/M/MV5BMjE2MzQwMTgxN15BMl5BanBnXkFtZTcwMDQzNjk2OQ@@._V1_SX300.jpg\",\r\n    \"Metascore\": \"80\",\r\n    \"Rating\": \"8.8\",\r\n    \"Votes\": \"842,451\",\r\n    \"ID\": \"cw0080684\",\r\n    \"Type\": \"movie\",\r\n    \"Price\": \"13.5\"\r\n}";

                var mockHttpClientFactory = new Mock<IHttpClientFactory>();
                var httpClientMock = GetMockHttpClient(HttpStatusCode.Forbidden, "", "", "");

                mockHttpClientFactory.Setup(f => f.CreateClient(It.IsAny<string>()))
                    .Returns(httpClientMock);

                IRedisService inMemoryRedisService = new InMemoryRedis();

                // TODO better to move the long strings to txt files to keep it clear and more readable.
                var movieDetails = JsonSerializer.Deserialize<List<CinemaWorldMovieDetailsDTO>>("[{\"ID\":\"cw0076759\",\"Title\":\"Star Wars: Episode IV - A New Hope\",\"Year\":\"1977\",\"Rated\":\"PG\",\"Released\":\"25 May 1977\",\"Runtime\":\"121 min\",\"Genre\":\"Action, Adventure, Fantasy\",\"Director\":\"George Lucas\",\"Writer\":\"George Lucas\",\"Actores\":null,\"Plot\":\"Luke Skywalker joins forces with a Jedi Knight, a cocky pilot, a wookiee and two droids to save the galaxy from the Empire\\u0027s world-destroying battle-station, while also attempting to rescue Princess Leia from the evil Darth Vader.\",\"Language\":\"English\",\"Country\":\"USA\",\"Awards\":\"Won 6 Oscars. Another 48 wins \\u0026 28 nominations.\",\"Poster\":\"https://m.media-amazon.com/images/M/MV5BOTIyMDY2NGQtOGJjNi00OTk4LWFhMDgtYmE3M2NiYzM0YTVmXkEyXkFqcGdeQXVyNTU1NTcwOTk@._V1_SX300.jpg\",\"Metascore\":\"92\",\"Rating\":\"8.7\",\"Votes\":\"915,459\",\"Type\":\"movie\",\"Price\":\"123.5\",\"Provider\":\"CinemaWorld\"},{\"ID\":\"cw0080684\",\"Title\":\"Star Wars: Episode V - The Empire Strikes Back\",\"Year\":\"1980\",\"Rated\":\"PG\",\"Released\":\"20 Jun 1980\",\"Runtime\":\"124 min\",\"Genre\":\"Action, Adventure, Fantasy\",\"Director\":\"Irvin Kershner\",\"Writer\":\"Leigh Brackett (screenplay), Lawrence Kasdan (screenplay), George Lucas (story by)\",\"Actores\":null,\"Plot\":\"After the Rebel base on the icy planet Hoth is taken over by the Empire, Han, Leia, Chewbacca, and C-3PO flee across the galaxy from the Empire. Luke travels to the forgotten planet of Dagobah to receive training from the Jedi master Yoda, while Vader endlessly pursues him.\",\"Language\":\"English\",\"Country\":\"USA\",\"Awards\":\"Won 1 Oscar. Another 19 wins \\u0026 18 nominations.\",\"Poster\":\"https://m.media-amazon.com/images/M/MV5BMjE2MzQwMTgxN15BMl5BanBnXkFtZTcwMDQzNjk2OQ@@._V1_SX300.jpg\",\"Metascore\":\"80\",\"Rating\":\"8.8\",\"Votes\":\"842,451\",\"Type\":\"movie\",\"Price\":\"13.5\",\"Provider\":\"CinemaWorld\"}]");

                await inMemoryRedisService.SetAsync<List<CinemaWorldMovieDetailsDTO>>("movies:cinemaWorldMovies", movieDetails);

                ServiceCollection.ReplaceWithMock<IHttpClientFactory>(mockHttpClientFactory);
                ServiceCollection.ReplaceWith<IRedisService>(inMemoryRedisService);

                var syncDataService = ServiceCollection.BuildServiceProvider().GetRequiredService<ISyncDataService>();

                using var cts = new CancellationTokenSource();

                await syncDataService.RunAsync(cts.Token, 0);

                var moviesInRedis = await inMemoryRedisService.GetAsync<List<CinemaWorldMovieDetailsDTO>>("movies:cinemaWorldMovies");

                Assert.NotNull(moviesInRedis);
                Assert.Equal(2, moviesInRedis.Count);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
                throw;
            }
        }
    }
}