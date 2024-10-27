using Exatek.RegistrationApi.Services.Interfase;
using Microsoft.Extensions.Caching.Memory;

namespace Exatek.RegistrationApi.Services;

public class OtpService : IOtpService
{
    private readonly IMemoryCache _cache;

    public OtpService(IMemoryCache cache)
    {
        _cache = cache;
    }

    // Method to generate and save the OTP
    public async Task SaveOtp(string key, string otp)
    {
        var cacheKey = $"OTP_{key}";
        var cacheEntryOptions = new MemoryCacheEntryOptions()
            .SetAbsoluteExpiration(TimeSpan.FromMinutes(5)); // Set OTP expiry time

        // Save OTP in cache with expiration options
        _cache.Set(cacheKey, otp, cacheEntryOptions);
    }

    // Method to retrieve the OTP for verification
    public bool TryGetOtp(string key, out string otp)
    {
        return _cache.TryGetValue($"OTP_{key}", out otp);
    }

    // Method to remove OTP after verification
    public void RemoveOtp(string key)
    {
        _cache.Remove($"OTP_{key}");
    }
}
