namespace FilmWorldDataSync.Services.Interfaces
{
    public interface ISyncDataService
    {
        public Task RunAsync(CancellationToken cancellationToken, int interval);
    }
}
