// <copyright file="Program.cs" company="PlaceholderCompany">
// Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>

namespace DbUpdate;

using DbUp;
using Microsoft.Extensions.Configuration;

/// <summary>
/// The main entry point for the application.
/// </summary>
public class Program
{
    /// <summary>
    /// The main method which runs the database update process.
    /// </summary>
    /// <param name="args">The command line arguments.</param>
    /// <returns>Returns 0 if successful, otherwise -1.</returns>
    private static int Main(string[] args)
    {
        string migrationPath = Path.Combine(
            Directory.GetCurrentDirectory(),
            "Streetcode.DAL",
            "Persistence",
            "ScriptsMigration");
        string seedPath = Path.Combine(
            Directory.GetCurrentDirectory(),
            "Streetcode.WebApi",
            "Extensions");

        var environment =
            Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") ?? "Local";

        var configuration = new ConfigurationBuilder()
            .SetBasePath(Path.Combine(Directory.GetCurrentDirectory(), "Streetcode.WebApi"))
            .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
            .AddJsonFile($"appsettings.{environment}.json", optional: true, reloadOnChange: true)
            .AddEnvironmentVariables("STREETCODE_")
            .Build();

        var connectionString = configuration.GetConnectionString("DefaultConnection");

        string? pathToScript = string.Empty;

        Console.WriteLine("Enter '-m' to MIGRATE or '-s' to SEED db:");

        pathToScript = Console.ReadLine() switch
        {
            "-s" => seedPath,
            "-m" => migrationPath,
            _ => null
        };

        if (pathToScript == null)
        {
            Console.WriteLine("Invalid command");
            return -1;
        }

        var upgrader =
            DeployChanges.To
                .SqlDatabase(connectionString)
                .WithScriptsFromFileSystem(pathToScript)
                .LogToConsole()
                .Build();

        var result = upgrader.PerformUpgrade();

        if (!result.Successful)
        {
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine(result.Error);
            Console.ResetColor();
#if DEBUG
            Console.ReadLine();
#endif
            return -1;
        }

        Console.ForegroundColor = ConsoleColor.Green;
        Console.WriteLine("Success!");
        Console.ResetColor();
        return 0;
    }
}
