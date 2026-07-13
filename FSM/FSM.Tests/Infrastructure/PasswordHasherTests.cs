using FSM.Infrastructure.Services;

namespace FSM.Tests.Infrastructure;

public class PasswordHasherTests
{
    private readonly PasswordHasher _hasher = new();

    [Fact]
    public void HashPassword_ProducesHash_DifferentFromPlainText()
    {
        var hash = _hasher.HashPassword("Sup3rSecret!");

        Assert.False(string.IsNullOrWhiteSpace(hash));
        Assert.NotEqual("Sup3rSecret!", hash);
    }

    [Fact]
    public void HashPassword_ProducesDifferentHashes_ForSamePassword()
    {
        var first = _hasher.HashPassword("Sup3rSecret!");
        var second = _hasher.HashPassword("Sup3rSecret!");

        // BCrypt salts each hash, so identical inputs yield different hashes.
        Assert.NotEqual(first, second);
    }

    [Fact]
    public void VerifyPassword_ReturnsTrue_ForCorrectPassword()
    {
        var hash = _hasher.HashPassword("Sup3rSecret!");

        Assert.True(_hasher.VerifyPassword("Sup3rSecret!", hash));
    }

    [Fact]
    public void VerifyPassword_ReturnsFalse_ForWrongPassword()
    {
        var hash = _hasher.HashPassword("Sup3rSecret!");

        Assert.False(_hasher.VerifyPassword("wrong-password", hash));
    }
}
