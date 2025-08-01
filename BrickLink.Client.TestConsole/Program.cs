using System.Text.Json;
using BrickLink.Client.Auth;

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

        // Get credentials from environment variables or user input
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
    /// Attempts to load BrickLink credentials from environment variables.
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
        return new BrickLinkCredentials(consumerKey, consumerSecret, tokenValue, tokenSecret);
    }

    /// <summary>
    /// Prompts the user to enter BrickLink credentials interactively.
    /// </summary>
    /// <returns>BrickLink credentials entered by the user.</returns>
    private static BrickLinkCredentials GetCredentialsFromUser()
    {
        Console.WriteLine("Please enter your BrickLink API credentials:");
        Console.WriteLine("(You can also set environment variables: BRICKLINK_CONSUMER_KEY, BRICKLINK_CONSUMER_SECRET, BRICKLINK_TOKEN_VALUE, BRICKLINK_TOKEN_SECRET)");
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
        var testUrl = "https://api.bricklink.com/api/v1/colors";
        Console.WriteLine($"Target URL: {testUrl}");

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
