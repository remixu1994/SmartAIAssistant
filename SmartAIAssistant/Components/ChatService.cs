using Microsoft.SemanticKernel;
using SemanticKernel;

namespace SmartAIAssistant.Components;

public class ChatService([FromKeyedServices("moonshot")] Kernel aiProvider)
{
    public async Task<string> InvokeAsync(string message)
    {
        var emojiPrompt = new EmojiPrompt();
        var result = await emojiPrompt.InvokeAsync(aiProvider, message);
        return result.ToString();
    }
}