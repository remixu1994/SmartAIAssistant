using Microsoft.Extensions.DependencyInjection;
using Microsoft.SemanticKernel;

namespace SemanticKernel;

public class KernelService
{
    public static Kernel GetKernel(string aiProviderCode)
    {
        var provider = BuildServiceProvider();
        var kernel = provider.GetKeyedService<Kernel>(aiProviderCode);

        Console.WriteLine($"[{aiProviderCode}] AI Kernel  is ready!");

        return kernel;
    }

    private static ServiceProvider BuildServiceProvider()
    {
        // 初始化依赖注入容器
        var services = new ServiceCollection();
        services.AddNLogging();
        services.RegisterKernels();
        return services.BuildServiceProvider();
    }
}