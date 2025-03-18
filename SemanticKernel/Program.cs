using System.Diagnostics.CodeAnalysis;
using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.ChatCompletion;
using Microsoft.SemanticKernel.TextGeneration;
using Microsoft.Extensions.DependencyInjection;

namespace SemanticKernel;

class Program
{
    [Experimental("SKEXP0010")]
    static async Task Main(string[] args)
    {
        var request = "请快速发送今日简报给总经理";
        var prompt = @$"<message role=""system"">Instructions: What is the intent of this request?
 If you don't know the intent, don't guess; instead respond with ""Unknown"".
 Choices: SendEmail, SendMessage, CompleteTask, CreateDocument, Unknown.
 </message>
 <message role=""user"">Can you send a very quick approval to the marketing team?</message>
 <message role=""assistant"">SendMessage</message>
 <message role=""user"">Can you send the full update to the marketing team? </message>
 <message role=""assistant"">SendEmail</message>
 <message role=""user"">{request}</message>
 <message role=""assistant""></message>";
        Kernel kernel = KernelService.GetKernel("azure-openai");
        var chatCompletionService = kernel.GetRequiredService<IChatCompletionService>();
        await StreamOutput(chatCompletionService, "Help me study semantic kernel.", kernel);
    }

    private static async Task StreamOutput(IChatCompletionService chatCompletionService, string prompt, Kernel kernel)
    {
        var chatHistory = new ChatHistory();
        chatHistory.AddUserMessage(prompt);
        var chatResult =  chatCompletionService.GetStreamingChatMessageContentsAsync(chatHistory, new PromptExecutionSettings(), kernel);
        // Get response from chat completion service     
        // var chatResult = chatCompletionService.GetStreamingChatMessageContentsAsync(message);
        string response = "";
        await foreach (var chunk in chatResult)
        {
            if (chunk.Role.HasValue) Console.Write(chunk.Role + ": ");
            response += chunk;
            await Task.Delay(100);
            Console.Write(chunk);
        }
        chatHistory.AddAssistantMessage(response);
    }
}