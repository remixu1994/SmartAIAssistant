using Microsoft.Extensions.Configuration;

namespace SemanticKernel;

public class AiSettings
{
    public static AiOptions LoadAiProvidersFromFile()
    {
        var configuration = new ConfigurationBuilder()
            .SetBasePath(Directory.GetCurrentDirectory())
            .AddJsonFile("AiAppSetting.json", optional: true, reloadOnChange: true)
            .Build();

        var aiOptions = configuration.GetSection("AI").Get<AiOptions>();
        return aiOptions!;
    }
}