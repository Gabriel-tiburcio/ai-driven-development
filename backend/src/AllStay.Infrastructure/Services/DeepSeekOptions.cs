namespace AllStay.Infrastructure.Services;

public class DeepSeekOptions
{
    public const string SectionName = "DeepSeek";

    public required string ApiKey { get; set; }
    public string Model { get; set; } = "deepseek-chat";
    public int MaxTokens { get; set; } = 400;
}
