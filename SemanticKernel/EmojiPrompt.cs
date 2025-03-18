using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.Connectors.OpenAI;

namespace SemanticKernel;

public class EmojiPrompt
{
    public async Task<FunctionResult> InvokeAsync(Kernel kernel, string message)
    {
        var prompt = """
                     请使用 Emoji 风格编辑以下段落，该风格以引人入胜的标题、每个段落中包含表情符号和在末尾添加相关标签为特点。
                     输出20-30字以内的抖音文案。
                     请确保保持原文的意思。
                     {{ $article }}
                     """;
        // 参数配置
        PromptExecutionSettings settings = new OpenAIPromptExecutionSettings(){
            Temperature = 1,
            TopP = 1,
            FrequencyPenalty = 0,
            PresencePenalty = 0
        };

        var rewriteFunction = kernel.CreateFunctionFromPrompt(prompt, settings);
        var article = """
                      9月6日16时20分，今年第11号超强台风“摩羯”在海南文昌沿海登陆。超强台风“摩羯”风力有多大？记者只能“抱团”出镜。
                      """;

        var rewriteResult = await kernel.InvokeAsync(rewriteFunction, new KernelArguments { ["article"] = message });
        Console.WriteLine(rewriteResult.ToString());
        return rewriteResult;
    }

    public async Task<FunctionResult> InvokeByPluginsAsync(Kernel kernel, KernelFunction kernelFunction)
    {
        var kernelPlugins = kernel.CreatePluginFromPromptDirectory("Prompts");
        var article = @"9月6日16时20分，今年第11号超强台风“摩羯”在海南文昌沿海登陆。超强台风“摩羯”风力有多大？记者只能“抱团”出镜。";

        var result = await kernel.InvokeAsync(kernelPlugins["RewriteRedBookStyle"], new KernelArguments { ["article"] = article });
        Console.WriteLine(result.ToString());
        return result;
    }

    public KernelFunction GetPlugins(Kernel kernel, string plugin)
    {
        var kernelPlugins = kernel.CreatePluginFromPromptDirectory("Prompts");
        return kernelPlugins[plugin];
    }
    
    public async Task<FunctionResult> InvokeByYamlAsync(Kernel kernel, string yaml)
    {
        var prompt = await File.ReadAllTextAsync("Prompts/RewriteEmojiStyle/rewirtewithemoji.yaml");

        var rewriteFunction = kernel.CreateFunctionFromPromptYaml(prompt);

        var article = @"9月6日16时20分，今年第11号超强台风“摩羯”在海南文昌沿海登陆。超强台风“摩羯”风力有多大？记者只能“抱团”出镜。";

        var result = await kernel.InvokeAsync(rewriteFunction, new KernelArguments { ["article"] = article });
        return result;
    }
}