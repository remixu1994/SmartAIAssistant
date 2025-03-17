using System.ClientModel;
using System.Diagnostics.CodeAnalysis;
using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.ChatCompletion;
using Microsoft.SemanticKernel.TextGeneration;
using Microsoft.Extensions.DependencyInjection;
using OpenAI;

namespace SemanticKernel;

class Program
{
    [Experimental("SKEXP0010")]
    static async Task Main(string[] args)
    {
        AiOptions aiOptions = AiSettings.LoadAiProvidersFromFile();
        AiProvider openAiProvider = aiOptions.Providers.Find(x => x.Code == "azure-openai")!;
        var builder = Kernel.CreateBuilder();

        Console.WriteLine("Hello, World!");

        // Add OpenAI chat completion
        builder.AddAzureOpenAIChatCompletion(
            deploymentName: "gpt-4o",
            endpoint: "https://my-openapi.openai.azure.com/",
            apiKey: openAiProvider.ApiKey);

        var aiProvider = "zhipu";
        AddChatCompletionService(builder, aiProvider);
        // Build kernel
        var kernel = builder.Build();
        // get text generation service
        var textGenerationService = kernel.Services.GetRequiredService<ITextGenerationService>();

        var text = await textGenerationService.GetTextContentAsync("Introduce yourself");
        Console.WriteLine(text);
    }
    
    public static IKernelBuilder AddChatCompletionService(IKernelBuilder builder, string? aiProviderCode = null)
    {
        var aiOptions = AiSettings.LoadAiProvidersFromFile();
        if (string.IsNullOrWhiteSpace(aiProviderCode))
        {
            aiProviderCode = aiOptions.DefaultProvider;
        }
        var aiProvider = aiOptions.Providers.FirstOrDefault(x => x.Code == aiProviderCode);
        if (aiProvider == null)
        {
            throw new ArgumentException($"AI Service Provider not found with code {aiProviderCode}");
        }
        var modelId = aiProvider.GetChatCompletionApiService()?.ModelId;
        if (string.IsNullOrWhiteSpace(modelId))
        {
            throw new ArgumentException($"未找到名称为 chat-completions 的 API 服务");
        }    

        if (aiProvider.AiType == AiProviderType.OpenAI)
        {
            builder.AddOpenAIChatCompletion(
                modelId: modelId,
                apiKey: aiProvider.ApiKey);
        }
    
        if (aiProvider.AiType == AiProviderType.AzureOpenAI)
        {
            builder.AddAzureOpenAIChatCompletion(
                deploymentName: modelId,
                endpoint: aiProvider.ApiEndpoint,
                apiKey: aiProvider.ApiKey);
        }

        if (aiProvider.AiType == AiProviderType.OpenAI_Compatible)
        {
            OpenAIClientOptions clientOptions = new OpenAIClientOptions
            {
                Endpoint = new Uri(aiProvider.ApiEndpoint)
            };

            OpenAIClient client = new(new ApiKeyCredential(aiProvider.ApiKey), clientOptions);

            builder.AddOpenAIChatCompletion(modelId: modelId, openAIClient: client);
        }
    
        return builder;
    }
}