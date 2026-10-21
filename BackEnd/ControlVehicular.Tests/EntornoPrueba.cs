using Microsoft.Extensions.FileProviders;

namespace ControlVehicular.Tests;

internal sealed class EntornoPrueba : IHostEnvironment
{
    public string ApplicationName { get; set; } = "ControlVehicular.Tests";
    public IFileProvider ContentRootFileProvider { get; set; } = new NullFileProvider();
    public string ContentRootPath { get; set; } = AppContext.BaseDirectory;
    public string EnvironmentName { get; set; } = Environments.Development;
}
