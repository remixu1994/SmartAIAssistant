using Microsoft.Extensions.DependencyInjection;

namespace SemanticKernel;

public static class KernelInjectionExtensions
{
    public static IServiceCollection RegisterKernels(this IServiceCollection services)
    {
        // 从配置文件中加载AI配置
        var aiOptions = AiSettings.LoadAiProvidersFromFile();
        // 注册其他AI服务提供商
        foreach (var aiProvider in aiOptions.Providers)
        {
            var providerRegister = Create(aiProvider!.AiType);
    
            providerRegister.Register(services, aiProvider);
        }
        return services;
    }
    
    public static AiProviderRegister Create(AiProviderType aiProviderType)
    {
        return aiProviderType switch
        {
            AiProviderType.OpenAI => new OpenAiRegister(),
            AiProviderType.OpenAI_Compatible => new OpenAiCompatibleAiRegister(),
            AiProviderType.AzureOpenAI => new AzureOpenAiRegister(),
            _ => throw new NotImplementedException($"No AI register for nameof(aiProviderType)")
        };
    }
}