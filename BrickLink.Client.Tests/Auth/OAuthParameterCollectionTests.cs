using BrickLink.Client.Auth;

namespace BrickLink.Client.Tests.Auth;

/// <summary>
/// Unit tests for the <see cref="OAuthParameterCollection"/> class.
/// </summary>
public class OAuthParameterCollectionTests
{
    [Fact]
    public void Constructor_InitializesEmptyCollection()
    {
        // Act
        var collection = new OAuthParameterCollection();

        // Assert
        Assert.Equal(0, collection.Count);
        Assert.Empty(collection.Keys);
    }

    [Fact]
    public void Add_WithValidKeyValue_AddsParameter()
    {
        // Arrange
        var collection = new OAuthParameterCollection();

        // Act
        collection.Add("test_key", "test_value");

        // Assert
        Assert.Equal(1, collection.Count);
        Assert.Equal("test_value", collection["test_key"]);
    }

    [Fact]
    public void Add_WithNullKey_ThrowsArgumentNullException()
    {
        // Arrange
        var collection = new OAuthParameterCollection();

        // Act & Assert
        var exception = Assert.Throws<ArgumentNullException>(() =>
            collection.Add(null!, "value"));

        Assert.Equal("key", exception.ParamName);
    }

    [Fact]
    public void Add_WithNullValue_AllowsNullValue()
    {
        // Arrange
        var collection = new OAuthParameterCollection();

        // Act
        collection.Add("test_key", null);

        // Assert
        Assert.Equal(1, collection.Count);
        Assert.Null(collection["test_key"]);
    }

    [Fact]
    public void Indexer_Get_WithExistingKey_ReturnsValue()
    {
        // Arrange
        var collection = new OAuthParameterCollection();
        collection.Add("test_key", "test_value");

        // Act
        var value = collection["test_key"];

        // Assert
        Assert.Equal("test_value", value);
    }

    [Fact]
    public void Indexer_Get_WithNonExistingKey_ReturnsNull()
    {
        // Arrange
        var collection = new OAuthParameterCollection();

        // Act
        var value = collection["non_existing_key"];

        // Assert
        Assert.Null(value);
    }

    [Fact]
    public void Indexer_Set_WithValidKey_SetsValue()
    {
        // Arrange
        var collection = new OAuthParameterCollection();

        // Act
        collection["test_key"] = "test_value";

        // Assert
        Assert.Equal("test_value", collection["test_key"]);
        Assert.Equal(1, collection.Count);
    }

    [Fact]
    public void Indexer_Get_WithNullKey_ThrowsArgumentNullException()
    {
        // Arrange
        var collection = new OAuthParameterCollection();

        // Act & Assert
        var exception = Assert.Throws<ArgumentNullException>(() =>
            _ = collection[null!]);

        Assert.Equal("key", exception.ParamName);
    }

    [Fact]
    public void Indexer_Set_WithNullKey_ThrowsArgumentNullException()
    {
        // Arrange
        var collection = new OAuthParameterCollection();

        // Act & Assert
        var exception = Assert.Throws<ArgumentNullException>(() =>
            collection[null!] = "value");

        Assert.Equal("key", exception.ParamName);
    }

    [Fact]
    public void Remove_WithExistingKey_RemovesParameterAndReturnsTrue()
    {
        // Arrange
        var collection = new OAuthParameterCollection();
        collection.Add("test_key", "test_value");

        // Act
        var removed = collection.Remove("test_key");

        // Assert
        Assert.True(removed);
        Assert.Equal(0, collection.Count);
        Assert.Null(collection["test_key"]);
    }

    [Fact]
    public void Remove_WithNonExistingKey_ReturnsFalse()
    {
        // Arrange
        var collection = new OAuthParameterCollection();

        // Act
        var removed = collection.Remove("non_existing_key");

        // Assert
        Assert.False(removed);
    }

    [Fact]
    public void Remove_WithNullKey_ThrowsArgumentNullException()
    {
        // Arrange
        var collection = new OAuthParameterCollection();

        // Act & Assert
        var exception = Assert.Throws<ArgumentNullException>(() =>
            collection.Remove(null!));

        Assert.Equal("key", exception.ParamName);
    }

    [Fact]
    public void ContainsKey_WithExistingKey_ReturnsTrue()
    {
        // Arrange
        var collection = new OAuthParameterCollection();
        collection.Add("test_key", "test_value");

        // Act
        var contains = collection.ContainsKey("test_key");

        // Assert
        Assert.True(contains);
    }

    [Fact]
    public void ContainsKey_WithNonExistingKey_ReturnsFalse()
    {
        // Arrange
        var collection = new OAuthParameterCollection();

        // Act
        var contains = collection.ContainsKey("non_existing_key");

        // Assert
        Assert.False(contains);
    }

    [Fact]
    public void ContainsKey_WithNullKey_ThrowsArgumentNullException()
    {
        // Arrange
        var collection = new OAuthParameterCollection();

        // Act & Assert
        var exception = Assert.Throws<ArgumentNullException>(() =>
            collection.ContainsKey(null!));

        Assert.Equal("key", exception.ParamName);
    }

    [Fact]
    public void Clear_RemovesAllParameters()
    {
        // Arrange
        var collection = new OAuthParameterCollection();
        collection.Add("key1", "value1");
        collection.Add("key2", "value2");
        collection.Add("key3", "value3");

        // Act
        collection.Clear();

        // Assert
        Assert.Equal(0, collection.Count);
        Assert.Empty(collection.Keys);
    }

    [Fact]
    public void SetConsumerKey_SetsOAuthConsumerKeyParameter()
    {
        // Arrange
        var collection = new OAuthParameterCollection();

        // Act
        collection.SetConsumerKey("consumer_key_value");

        // Assert
        Assert.Equal("consumer_key_value", collection["oauth_consumer_key"]);
    }

    [Fact]
    public void SetConsumerKey_WithNullValue_ThrowsArgumentNullException()
    {
        // Arrange
        var collection = new OAuthParameterCollection();

        // Act & Assert
        var exception = Assert.Throws<ArgumentNullException>(() =>
            collection.SetConsumerKey(null!));

        Assert.Equal("consumerKey", exception.ParamName);
    }

    [Fact]
    public void SetAccessToken_SetsOAuthTokenParameter()
    {
        // Arrange
        var collection = new OAuthParameterCollection();

        // Act
        collection.SetAccessToken("access_token_value");

        // Assert
        Assert.Equal("access_token_value", collection["oauth_token"]);
    }

    [Fact]
    public void SetAccessToken_WithNullValue_ThrowsArgumentNullException()
    {
        // Arrange
        var collection = new OAuthParameterCollection();

        // Act & Assert
        var exception = Assert.Throws<ArgumentNullException>(() =>
            collection.SetAccessToken(null!));

        Assert.Equal("accessToken", exception.ParamName);
    }

    [Fact]
    public void SetSignatureMethod_SetsOAuthSignatureMethodParameter()
    {
        // Arrange
        var collection = new OAuthParameterCollection();

        // Act
        collection.SetSignatureMethod("HMAC-SHA1");

        // Assert
        Assert.Equal("HMAC-SHA1", collection["oauth_signature_method"]);
    }

    [Fact]
    public void SetSignatureMethod_WithNullValue_ThrowsArgumentNullException()
    {
        // Arrange
        var collection = new OAuthParameterCollection();

        // Act & Assert
        var exception = Assert.Throws<ArgumentNullException>(() =>
            collection.SetSignatureMethod(null!));

        Assert.Equal("signatureMethod", exception.ParamName);
    }

    [Fact]
    public void SetTimestamp_SetsOAuthTimestampParameter()
    {
        // Arrange
        var collection = new OAuthParameterCollection();

        // Act
        collection.SetTimestamp("1234567890");

        // Assert
        Assert.Equal("1234567890", collection["oauth_timestamp"]);
    }

    [Fact]
    public void SetTimestamp_WithNullValue_ThrowsArgumentNullException()
    {
        // Arrange
        var collection = new OAuthParameterCollection();

        // Act & Assert
        var exception = Assert.Throws<ArgumentNullException>(() =>
            collection.SetTimestamp(null!));

        Assert.Equal("timestamp", exception.ParamName);
    }

    [Fact]
    public void SetNonce_SetsOAuthNonceParameter()
    {
        // Arrange
        var collection = new OAuthParameterCollection();

        // Act
        collection.SetNonce("random_nonce_value");

        // Assert
        Assert.Equal("random_nonce_value", collection["oauth_nonce"]);
    }

    [Fact]
    public void SetNonce_WithNullValue_ThrowsArgumentNullException()
    {
        // Arrange
        var collection = new OAuthParameterCollection();

        // Act & Assert
        var exception = Assert.Throws<ArgumentNullException>(() =>
            collection.SetNonce(null!));

        Assert.Equal("nonce", exception.ParamName);
    }

    [Fact]
    public void SetVersion_SetsOAuthVersionParameter()
    {
        // Arrange
        var collection = new OAuthParameterCollection();

        // Act
        collection.SetVersion("1.0");

        // Assert
        Assert.Equal("1.0", collection["oauth_version"]);
    }

    [Fact]
    public void SetVersion_WithNullValue_ThrowsArgumentNullException()
    {
        // Arrange
        var collection = new OAuthParameterCollection();

        // Act & Assert
        var exception = Assert.Throws<ArgumentNullException>(() =>
            collection.SetVersion(null!));

        Assert.Equal("version", exception.ParamName);
    }

    [Fact]
    public void SetSignature_SetsOAuthSignatureParameter()
    {
        // Arrange
        var collection = new OAuthParameterCollection();

        // Act
        collection.SetSignature("signature_value");

        // Assert
        Assert.Equal("signature_value", collection["oauth_signature"]);
    }

    [Fact]
    public void SetSignature_WithNullValue_ThrowsArgumentNullException()
    {
        // Arrange
        var collection = new OAuthParameterCollection();

        // Act & Assert
        var exception = Assert.Throws<ArgumentNullException>(() =>
            collection.SetSignature(null!));

        Assert.Equal("signature", exception.ParamName);
    }

    [Fact]
    public void ToQueryString_WithMultipleParameters_ReturnsSortedEncodedString()
    {
        // Arrange
        var collection = new OAuthParameterCollection();
        collection.Add("oauth_consumer_key", "consumer key");
        collection.Add("oauth_nonce", "nonce value");
        collection.Add("oauth_timestamp", "1234567890");

        // Act
        var queryString = collection.ToQueryString();

        // Assert
        // Parameters should be sorted alphabetically and URL-encoded
        var expected = "oauth_consumer_key=consumer%20key&oauth_nonce=nonce%20value&oauth_timestamp=1234567890";
        Assert.Equal(expected, queryString);
    }

    [Fact]
    public void ToQueryString_WithSpecialCharacters_ProperlyEncodesValues()
    {
        // Arrange
        var collection = new OAuthParameterCollection();
        collection.Add("param1", "value with spaces");
        collection.Add("param2", "value&with&ampersands");
        collection.Add("param3", "value=with=equals");

        // Act
        var queryString = collection.ToQueryString();

        // Assert
        Assert.Contains("param1=value%20with%20spaces", queryString);
        Assert.Contains("param2=value%26with%26ampersands", queryString);
        Assert.Contains("param3=value%3Dwith%3Dequals", queryString);
    }

    [Fact]
    public void ToQueryString_WithEmptyCollection_ReturnsEmptyString()
    {
        // Arrange
        var collection = new OAuthParameterCollection();

        // Act
        var queryString = collection.ToQueryString();

        // Assert
        Assert.Equal("", queryString);
    }

    [Fact]
    public void ToAuthorizationHeaderValue_WithMultipleParameters_ReturnsFormattedHeader()
    {
        // Arrange
        var collection = new OAuthParameterCollection();
        collection.Add("oauth_consumer_key", "consumer_key");
        collection.Add("oauth_nonce", "nonce_value");
        collection.Add("oauth_timestamp", "1234567890");

        // Act
        var headerValue = collection.ToAuthorizationHeaderValue();

        // Assert
        // Should start with "OAuth " and have comma-separated quoted values
        Assert.StartsWith("OAuth ", headerValue);
        Assert.Contains("oauth_consumer_key=\"consumer_key\"", headerValue);
        Assert.Contains("oauth_nonce=\"nonce_value\"", headerValue);
        Assert.Contains("oauth_timestamp=\"1234567890\"", headerValue);
    }

    [Fact]
    public void ToAuthorizationHeaderValue_WithSpecialCharacters_ProperlyEncodesValues()
    {
        // Arrange
        var collection = new OAuthParameterCollection();
        collection.Add("oauth_consumer_key", "key with spaces");
        collection.Add("oauth_signature", "sig/with/slashes");

        // Act
        var headerValue = collection.ToAuthorizationHeaderValue();

        // Assert
        Assert.Contains("oauth_consumer_key=\"key%20with%20spaces\"", headerValue);
        Assert.Contains("oauth_signature=\"sig%2Fwith%2Fslashes\"", headerValue);
    }

    [Fact]
    public void GetEnumerator_ReturnsAllKeyValuePairs()
    {
        // Arrange
        var collection = new OAuthParameterCollection();
        collection.Add("key1", "value1");
        collection.Add("key2", "value2");
        collection.Add("key3", "value3");

        // Act
        var pairs = collection.ToList();

        // Assert
        Assert.Equal(3, pairs.Count);
        Assert.Contains(pairs, p => p.Key == "key1" && p.Value == "value1");
        Assert.Contains(pairs, p => p.Key == "key2" && p.Value == "value2");
        Assert.Contains(pairs, p => p.Key == "key3" && p.Value == "value3");
    }

    [Fact]
    public void ToString_ReturnsQueryStringFormat()
    {
        // Arrange
        var collection = new OAuthParameterCollection();
        collection.Add("param1", "value1");
        collection.Add("param2", "value2");

        // Act
        var result = collection.ToString();

        // Assert
        Assert.Equal(collection.ToQueryString(), result);
    }

    [Fact]
    public void CompleteOAuthFlow_AllParametersSet_ProducesValidOutput()
    {
        // Arrange
        var collection = new OAuthParameterCollection();

        // Act
        collection.SetConsumerKey("consumer_key");
        collection.SetAccessToken("access_token");
        collection.SetSignatureMethod("HMAC-SHA1");
        collection.SetTimestamp("1234567890");
        collection.SetNonce("random_nonce");
        collection.SetVersion("1.0");
        collection.SetSignature("signature_value");

        // Assert
        Assert.Equal(7, collection.Count);

        var queryString = collection.ToQueryString();
        Assert.Contains("oauth_consumer_key=consumer_key", queryString);
        Assert.Contains("oauth_token=access_token", queryString);
        Assert.Contains("oauth_signature_method=HMAC-SHA1", queryString);
        Assert.Contains("oauth_timestamp=1234567890", queryString);
        Assert.Contains("oauth_nonce=random_nonce", queryString);
        Assert.Contains("oauth_version=1.0", queryString);
        Assert.Contains("oauth_signature=signature_value", queryString);

        var headerValue = collection.ToAuthorizationHeaderValue();
        Assert.StartsWith("OAuth ", headerValue);
        Assert.Contains("oauth_consumer_key=\"consumer_key\"", headerValue);
        Assert.Contains("oauth_token=\"access_token\"", headerValue);
    }

    [Fact]
    public void Keys_ReturnsAllParameterKeys()
    {
        // Arrange
        var collection = new OAuthParameterCollection();
        collection.Add("key1", "value1");
        collection.Add("key2", "value2");
        collection.Add("key3", "value3");

        // Act
        var keys = collection.Keys.ToList();

        // Assert
        Assert.Equal(3, keys.Count);
        Assert.Contains("key1", keys);
        Assert.Contains("key2", keys);
        Assert.Contains("key3", keys);
    }

    [Fact]
    public void ParameterOverwrite_ReplacesExistingValue()
    {
        // Arrange
        var collection = new OAuthParameterCollection();
        collection.Add("test_key", "original_value");

        // Act
        collection.Add("test_key", "new_value");

        // Assert
        Assert.Equal("new_value", collection["test_key"]);
        Assert.Equal(1, collection.Count); // Should still be 1, not 2
    }
}
