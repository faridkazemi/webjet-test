namespace Webjet_Movies_Backend.Services.Interfaces
{
    public interface IMovieDataProvider<T>
    {
        public Task<T> GetMovies(string key);
    }
}
