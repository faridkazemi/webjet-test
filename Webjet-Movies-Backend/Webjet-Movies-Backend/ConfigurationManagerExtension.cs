namespace Webjet_Movies_Backend
{
    public static class ConfigurationManagerExtension
    {
        public static IConfigurationBuilder AddSecrets(this IConfigurationBuilder config, string secretDir)
        {
            if (Directory.Exists(secretDir))
            {
                var files = Directory.GetFiles(secretDir);

                var secrets = new Dictionary<string, string>();

                foreach (var file in files)
                {
                    var fileName = Path.GetFileName(file);

                    var secretKey = fileName.Replace("__", ":");

                    var secretValue = File.ReadAllText(file);

                    secrets[secretKey] = secretValue;

                }
                config.AddInMemoryCollection(secrets);

            }
            return config;
        }
    }
}
