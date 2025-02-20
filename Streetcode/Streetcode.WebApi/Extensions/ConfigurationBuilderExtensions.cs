namespace Streetcode.WebApi.Extensions
{
    public static class ConfigurationBuilderExtensions
    {
        public static IConfigurationBuilder ConfigureCustom(this IConfigurationBuilder builder, string environment)
        {
            builder.SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
                .AddJsonFile($"appsettings.{environment}.json", optional: true, reloadOnChange: true)
                .AddUserSecrets<Program>()
                .AddEnvironmentVariables("STREETCODE_");

            return builder;
        }
    }
}