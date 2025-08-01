using System.Text.Json;
using BrickLink.Client.Auth;
using DotNetEnv;

namespace BrickLink.Client.TestConsole;

/// <summary>
/// Console application for testing BrickLink API authentication integration.
/// This application demonstrates OAuth 1.0a authentication with the BrickLink API
/// by making authenticated requests and validating the authentication headers.
/// </summary>
internal class Program
{
    /// <summary>
    /// Entry point for the authentication test console application.
    /// </summary>
    /// <param name="args">Command line arguments (not used).</param>
    /// <returns>A task representing the asynchronous operation.</returns>
    private static async Task Main(string[] args)
    {
        Console.WriteLine("BrickLink API Authentication Test Console");
        Console.WriteLine("========================================");
        Console.WriteLine();

        // Load .env file if it exists
        LoadEnvironmentFile();

        // Get credentials from .env file, environment variables, or user input
        var credentials = GetCredentialsFromEnvironment() ?? GetCredentialsFromUser();

        if (credentials == null)
        {
            Console.WriteLine("Error: Unable to obtain valid credentials.");
            Environment.Exit(1);
            return;
        }

        try
        {
            // Test authentication integration
            await TestAuthenticationIntegration(credentials);

            Console.WriteLine();
            Console.WriteLine("Authentication test completed successfully!");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error during authentication test: {ex.Message}");
            Console.WriteLine($"Details: {ex}");
            Environment.Exit(1);
        }

        Console.WriteLine();
        Console.WriteLine("Press any key to exit...");
        Console.ReadKey();
    }

    /// <summary>
    /// Loads environment variables from a .env file if it exists.
    /// Searches for the .env file in the application directory first, then the current working directory.
    /// This method silently handles cases where the .env file doesn't exist or can't be read.
    /// </summary>
    private static void LoadEnvironmentFile()
    {
        try
        {
            // First, try to find .env file in the application's directory (where the exe is located)
            var appDirectory = Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location);
            var envFilePath = Path.Combine(appDirectory ?? "", ".env");
            
            // If not found in app directory, try the current working directory
            if (!File.Exists(envFilePath))
            {
                envFilePath = Path.Combine(Directory.GetCurrentDirectory(), ".env");
            }
            
            // If still not found, try looking in the project source directory
            if (!File.Exists(envFilePath))
            {
                // Get the directory containing the source code (where Program.cs is located)
                var sourceFile = GetSourceFilePath();
                if (!string.IsNullOrEmpty(sourceFile))
                {
                    var sourceDirectory = Path.GetDirectoryName(sourceFile);
                    if (!string.IsNullOrEmpty(sourceDirectory))
                    {
                        envFilePath = Path.Combine(sourceDirectory, ".env");
                    }
                }
            }
            
            if (File.Exists(envFilePath))
            {
                Env.Load(envFilePath);
                Console.WriteLine($"Loaded environment variables from .env file: {envFilePath}");
            }
            else
            {
                Console.WriteLine("No .env file found. Using system environment variables or interactive input.");
            }
        }
        catch (Exception ex)
        {
            // Silently handle .env file loading errors to not disrupt the normal flow
            Console.WriteLine($"Note: Could not load .env file: {ex.Message}");
        }
    }

    /// <summary>
    /// Gets the source file path of the current method using caller attributes.
    /// This helps locate the .env file relative to the source code location.
    /// </summary>
    /// <param name="sourceFilePath">Automatically populated with the source file path.</param>
    /// <returns>The source file path or null if not available.</returns>
    private static string? GetSourceFilePath([System.Runtime.CompilerServices.CallerFilePath] string? sourceFilePath = null)
    {
        return sourceFilePath;
    }

    /// <summary>
    /// Attempts to load BrickLink credentials from environment variables.
    /// This includes variables loaded from a .env file or system environment variables.
    /// </summary>
    /// <returns>BrickLink credentials if all required environment variables are present, otherwise null.</returns>
    private static BrickLinkCredentials? GetCredentialsFromEnvironment()
    {
        var consumerKey = Environment.GetEnvironmentVariable("BRICKLINK_CONSUMER_KEY");
        var consumerSecret = Environment.GetEnvironmentVariable("BRICKLINK_CONSUMER_SECRET");
        var tokenValue = Environment.GetEnvironmentVariable("BRICKLINK_TOKEN_VALUE");
        var tokenSecret = Environment.GetEnvironmentVariable("BRICKLINK_TOKEN_SECRET");

        if (string.IsNullOrWhiteSpace(consumerKey) ||
            string.IsNullOrWhiteSpace(consumerSecret) ||
            string.IsNullOrWhiteSpace(tokenValue) ||
            string.IsNullOrWhiteSpace(tokenSecret))
        {
            return null;
        }

        Console.WriteLine("Loading credentials from environment variables...");
        Console.WriteLine($"Consumer Key: {consumerKey?.Substring(0, Math.Min(8, consumerKey.Length))}...");
        Console.WriteLine($"Token Value: {tokenValue?.Substring(0, Math.Min(8, tokenValue.Length))}...");
        return new BrickLinkCredentials(consumerKey, consumerSecret, tokenValue, tokenSecret);
    }

    /// <summary>
    /// Prompts the user to enter BrickLink credentials interactively.
    /// </summary>
    /// <returns>BrickLink credentials entered by the user.</returns>
    private static BrickLinkCredentials GetCredentialsFromUser()
    {
        Console.WriteLine("Please enter your BrickLink API credentials:");
        Console.WriteLine("(You can also create a .env file or set environment variables: BRICKLINK_CONSUMER_KEY, BRICKLINK_CONSUMER_SECRET, BRICKLINK_TOKEN_VALUE, BRICKLINK_TOKEN_SECRET)");
        Console.WriteLine();

        Console.Write("Consumer Key: ");
        var consumerKey = Console.ReadLine() ?? "";

        Console.Write("Consumer Secret: ");
        var consumerSecret = ReadPasswordFromConsole();

        Console.Write("Token Value: ");
        var tokenValue = Console.ReadLine() ?? "";

        Console.Write("Token Secret: ");
        var tokenSecret = ReadPasswordFromConsole();

        Console.WriteLine();
        return new BrickLinkCredentials(consumerKey, consumerSecret, tokenValue, tokenSecret);
    }

    /// <summary>
    /// Reads a password from the console without displaying the characters on screen.
    /// </summary>
    /// <returns>The password entered by the user.</returns>
    private static string ReadPasswordFromConsole()
    {
        var password = new System.Text.StringBuilder();
        ConsoleKeyInfo key;

        do
        {
            key = Console.ReadKey(true);

            if (key.Key == ConsoleKey.Backspace && password.Length > 0)
            {
                password.Remove(password.Length - 1, 1);
                Console.Write("\b \b");
            }
            else if (key.Key != ConsoleKey.Enter && key.Key != ConsoleKey.Backspace)
            {
                password.Append(key.KeyChar);
                Console.Write("*");
            }
        } while (key.Key != ConsoleKey.Enter);

        Console.WriteLine();
        return password.ToString();
    }

    /// <summary>
    /// Tests the authentication integration by creating an authenticated HttpClient
    /// and making a request to the BrickLink API.
    /// </summary>
    /// <param name="credentials">The BrickLink API credentials to use for authentication.</param>
    /// <returns>A task representing the asynchronous operation.</returns>
    private static async Task TestAuthenticationIntegration(BrickLinkCredentials credentials)
    {
        Console.WriteLine("Creating authenticated HttpClient...");

        // Create authenticated HttpClient using the authentication handler
        using var httpClient = AuthenticatedHttpClientFactory.CreateAuthenticatedHttpClient(credentials);

        Console.WriteLine("HttpClient created successfully with authentication handler.");
        Console.WriteLine();

        // Test OAuth signature generation and header construction
        Console.WriteLine("Testing OAuth signature generation...");
        var testUrl = "colors";
        Console.WriteLine($"Target URL: {httpClient.BaseAddress}{testUrl}");

        // Make authenticated request to BrickLink API
        Console.WriteLine("Making authenticated request to BrickLink API...");

        var response = await httpClient.GetAsync(testUrl);

        Console.WriteLine($"Response Status: {response.StatusCode} ({(int)response.StatusCode})");
        Console.WriteLine($"Response Headers:");

        foreach (var header in response.Headers)
        {
            Console.WriteLine($"  {header.Key}: {string.Join(", ", header.Value)}");
        }

        foreach (var header in response.Content.Headers)
        {
            Console.WriteLine($"  {header.Key}: {string.Join(", ", header.Value)}");
        }

        Console.WriteLine();

        // Read and display response content
        var content = await response.Content.ReadAsStringAsync();

        if (response.IsSuccessStatusCode)
        {
            Console.WriteLine("✅ Authentication successful!");
            Console.WriteLine("Response content preview:");

            // Try to pretty-print JSON if possible
            try
            {
                var jsonDocument = JsonDocument.Parse(content);
                var prettyJson = JsonSerializer.Serialize(jsonDocument, new JsonSerializerOptions
                {
                    WriteIndented = true
                });

                // Show first 500 characters of pretty JSON
                var preview = prettyJson.Length > 500 ? prettyJson.Substring(0, 500) + "..." : prettyJson;
                Console.WriteLine(preview);
            }
            catch
            {
                // If JSON parsing fails, show raw content preview
                var preview = content.Length > 500 ? content.Substring(0, 500) + "..." : content;
                Console.WriteLine(preview);
            }
        }
        else
        {
            Console.WriteLine("❌ Authentication failed or API error occurred.");
            Console.WriteLine("Response content:");
            Console.WriteLine(content);

            throw new InvalidOperationException($"API request failed with status {response.StatusCode}: {content}");
        }
    }
}
