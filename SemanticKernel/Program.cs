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

        var zhipuEndpoint = new Uri("https://open.bigmodel.cn/api/paas/v4/");
        AiProvider zhiPuAiProvider = aiOptions.Providers.Find(x => x.Code == "zhipu")!;
        OpenAIClientOptions clientOptions = new OpenAIClientOptions();
        clientOptions.Endpoint = zhipuEndpoint;
        OpenAIClient client = new(new ApiKeyCredential(zhiPuAiProvider.ApiKey), clientOptions);
        // Add OpenAI Chat completion
        builder.AddOpenAIChatCompletion(
            modelId: "glm-4-flash", // 可选模型编码：glm-4-plus、glm-4-0520、glm-4 、glm-4-air、glm-4-airx、glm-4-long、 glm-4-flash(免费)
            openAIClient: client);

        // Build kernel
        var kernel = builder.Build();
        // get text generation service
        var textGenerationService = kernel.Services.GetRequiredService<ITextGenerationService>();

        var text = await textGenerationService.GetTextContentAsync("Introduce yourself");
        Console.WriteLine(text);
    }
}