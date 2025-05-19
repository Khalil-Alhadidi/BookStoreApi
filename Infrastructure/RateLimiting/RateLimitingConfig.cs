namespace BookStoreApi.Infrastructure.RateLimiting;

public class RateLimitingConfig
{
    public RateLimitPolicy Global { get; set; } = new();
    public RateLimitPolicy Auth { get; set; } = new();
    public RateLimitPolicy Books { get; set; } = new();
}

public class RateLimitPolicy
{
    public int PermitLimit { get; set; } = 100;
    public int Window { get; set; } = 10;
    public int QueueLimit { get; set; } = 2;
} 