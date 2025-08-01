# BrickLink API Authentication Test Console

This console application demonstrates and tests the BrickLink API authentication integration. It validates that the OAuth 1.0a authentication system works correctly by making authenticated requests to the BrickLink API.

## Purpose

The test console serves several key purposes:

1. **Authentication Verification** - Validates that OAuth signature generation and header construction work correctly
2. **Integration Testing** - Tests the complete authentication flow from credentials to API response
3. **Real-world Validation** - Makes actual requests to the BrickLink API to ensure compatibility
4. **Developer Tool** - Provides a simple way to test credentials and troubleshoot authentication issues

## Features

- **Credential Input Options**:
  - `.env` file (recommended for local development)
  - Environment variables (recommended for automation)
  - Interactive console input with masked password entry
- **Comprehensive Testing**:
  - HttpClient creation with authentication handler
  - OAuth signature generation validation
  - Real API request to `/api/v1/colors` endpoint
  - Response validation and pretty-printing
- **Security Features**:
  - Masked password input for console entry
  - Secure credential handling
  - No credential logging or persistence

## Usage

### Option 1: .env File (Recommended for Local Development)

Create a `.env` file in the `BrickLink.Client.TestConsole` directory with your BrickLink API credentials:

```bash
# Copy the example file
cp .env.example .env
```

Then edit the `.env` file with your actual credentials:

```env
# BrickLink API Credentials
BRICKLINK_CONSUMER_KEY=your_consumer_key_here
BRICKLINK_CONSUMER_SECRET=your_consumer_secret_here
BRICKLINK_TOKEN_VALUE=your_token_value_here
BRICKLINK_TOKEN_SECRET=your_token_secret_here
```

Run the console application:

```bash
dotnet run --project BrickLink.Client.TestConsole
```

The application will automatically load the credentials from the `.env` file.

**Security Note**: The `.env` file is automatically ignored by Git to prevent accidental credential exposure.

### Option 2: Environment Variables (Recommended for CI/CD)

Set the following environment variables with your BrickLink API credentials:

```bash
# Windows Command Prompt
set BRICKLINK_CONSUMER_KEY=your_consumer_key
set BRICKLINK_CONSUMER_SECRET=your_consumer_secret
set BRICKLINK_TOKEN_VALUE=your_token_value
set BRICKLINK_TOKEN_SECRET=your_token_secret

# Windows PowerShell
$env:BRICKLINK_CONSUMER_KEY="your_consumer_key"
$env:BRICKLINK_CONSUMER_SECRET="your_consumer_secret"
$env:BRICKLINK_TOKEN_VALUE="your_token_value"
$env:BRICKLINK_TOKEN_SECRET="your_token_secret"

# Linux/macOS
export BRICKLINK_CONSUMER_KEY=your_consumer_key
export BRICKLINK_CONSUMER_SECRET=your_consumer_secret
export BRICKLINK_TOKEN_VALUE=your_token_value
export BRICKLINK_TOKEN_SECRET=your_token_secret
```

Then run the console application:

```bash
dotnet run --project BrickLink.Client.TestConsole
```

### Option 3: Interactive Input

Simply run the application without creating a `.env` file or setting environment variables, and it will prompt you to enter your credentials:

```bash
dotnet run --project BrickLink.Client.TestConsole
```

The application will prompt for:
- Consumer Key (visible input)
- Consumer Secret (masked input)
- Token Value (visible input)  
- Token Secret (masked input)

## Expected Output

### Successful Authentication

When authentication works correctly, you should see output similar to:

```
BrickLink API Authentication Test Console
========================================

Loaded environment variables from .env file...
Loading credentials from environment variables...
Creating authenticated HttpClient...
HttpClient created successfully with authentication handler.

Testing OAuth signature generation...
Target URL: https://api.bricklink.com/api/v1/colors
Making authenticated request to BrickLink API...
Response Status: OK (200)
Response Headers:
  Date: Thu, 01 Aug 2025 10:00:00 GMT
  Content-Type: application/json; charset=utf-8
  ...

✅ Authentication successful!
Response content preview:
{
  "meta": {
    "description": "OK",
    "message": "OK",
    "code": 200
  },
  "data": [
    {
      "color_id": 1,
      "color_name": "White",
      "color_code": "FFFFFF",
      "color_type": "Solid"
    },
    ...
  ]
}

Authentication test completed successfully!

Press any key to exit...
```

### Authentication Failure

If authentication fails, you'll see error details:

```
❌ Authentication failed or API error occurred.
Response content:
{"meta":{"description":"PERMISSION_DENIED","message":"Invalid consumer key","code":401}}

Error during authentication test: API request failed with status Unauthorized: {"meta":...}
```

## Getting BrickLink API Credentials

To use this test console, you need BrickLink API credentials:

1. **Register as a BrickLink Developer**:
   - Go to https://www.bricklink.com/v3/api/register_consumer.page
   - Fill out the developer registration form
   - You'll receive a Consumer Key and Consumer Secret

2. **Register Your IP Address**:
   - Go to https://www.bricklink.com/v3/api/register_token.page
   - Register your server/development machine's IP address
   - You'll receive a Token Value and Token Secret

3. **Use Credentials**: All four values (Consumer Key, Consumer Secret, Token Value, Token Secret) are required for authentication.

## Troubleshooting

### Common Issues

1. **"Invalid consumer key"** - Check that your Consumer Key is correct and properly registered
2. **"Invalid token"** - Verify your Token Value and Token Secret are correct
3. **"IP not registered"** - Ensure your current IP address is registered with BrickLink
4. **SSL/TLS errors** - Check your network connection and firewall settings

### Debugging Tips

- The console application shows detailed request/response information for troubleshooting
- Check that all four credential values are non-empty
- Verify your IP address matches what's registered with BrickLink
- Test with a simple API endpoint like `/api/v1/colors` first

## Security Notes

- **Never commit credentials to source control**
- **Use environment variables for automated testing**
- **The application doesn't log or persist credentials**
- **Secrets are masked during console input**
- **Consider using a secrets management system in production**

## Integration with CI/CD

This console application can be used in automated testing pipelines using environment variables:

```yaml
# Example GitHub Actions step
- name: Test BrickLink Authentication
  run: dotnet run --project BrickLink.Client.TestConsole
  env:
    BRICKLINK_CONSUMER_KEY: ${{ secrets.BRICKLINK_CONSUMER_KEY }}
    BRICKLINK_CONSUMER_SECRET: ${{ secrets.BRICKLINK_CONSUMER_SECRET }}
    BRICKLINK_TOKEN_VALUE: ${{ secrets.BRICKLINK_TOKEN_VALUE }}
    BRICKLINK_TOKEN_SECRET: ${{ secrets.BRICKLINK_TOKEN_SECRET }}
```

For local development, prefer using the `.env` file approach as it's more convenient and secure.

## Development Notes

This console application demonstrates several key patterns:

- **Factory Pattern**: Uses `AuthenticatedHttpClientFactory` to create configured HttpClients
- **Dependency Injection Ready**: Shows how the authentication system integrates with DI
- **Error Handling**: Proper exception handling and user-friendly error messages
- **Security Best Practices**: Secure credential handling and masked input
- **JSON Processing**: Response parsing and pretty-printing for debugging

The implementation serves as both a testing tool and a reference for integrating the BrickLink authentication system into other applications.