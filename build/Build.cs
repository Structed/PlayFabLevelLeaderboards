using System;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Reflection.PortableExecutable;
using Newtonsoft.Json;
using Nuke.Common;
using Nuke.Common.CI;
using Nuke.Common.CI.GitHubActions;
using Nuke.Common.Execution;
using Nuke.Common.IO;
using Nuke.Common.ProjectModel;
using Nuke.Common.Tooling;
using Nuke.Common.Tools.DotNet;
using Nuke.Common.Utilities.Collections;
using static Nuke.Common.EnvironmentInfo;
using static Nuke.Common.IO.FileSystemTasks;
using static Nuke.Common.IO.PathConstruction;
using static Nuke.Common.Tools.DotNet.DotNetTasks;
using static Nuke.Common.IO.CompressionTasks;
using static Nuke.Common.Tools.Npm.NpmTasks;

[GitHubActions(
    "continuous",
    GitHubActionsImage.UbuntuLatest,
    On = new[] { GitHubActionsTrigger.Push },
    InvokedTargets = new[] { nameof(Deploy) })]
class Build : NukeBuild
{
    /// Support plugins are available for:
    ///   - JetBrains ReSharper        https://nuke.build/resharper
    ///   - JetBrains Rider            https://nuke.build/rider
    ///   - Microsoft VisualStudio     https://nuke.build/visualstudio
    ///   - Microsoft VSCode           https://nuke.build/vscode

    public static int Main () => Execute<Build>(x => x.Compile);

    [Solution]
    readonly Solution Solution;

    AbsolutePath OutputDirectory => RootDirectory / "output";

    string AppName => "pfleaderboards-function";
    AbsolutePath ZipPath => OutputDirectory / $"{AppName}.zip";

    [PathExecutable]
    readonly Tool Az;

    [PathExecutable]
    readonly Tool Func;

    [Parameter("The Deployment Slot for the Azure Function")]
    readonly string DeploymentSlot = String.Empty;

    [Parameter("Configuration to build - Default is 'Debug' (local) or 'Release' (server)")]
    readonly Configuration Configuration = IsLocalBuild ? Configuration.Debug : Configuration.Release;

    [Parameter("The name of the Azure Resource Group to be used")]
    readonly string ResourceGroupName = "pfleaderboards-rg";   //Environment.GetEnvironmentVariable("LEADERBOARDS_RESOURCE_GROUP_NAME");

    [Parameter("The Azure Region to deploy resources to")]
    readonly string AzureLocation  = Environment.GetEnvironmentVariable("LEADERBOARDS_AZURE_LOCATION");

    [Parameter("The Azure Subscription ID to deploy into")]
    readonly string AzureSubscriptionId  = Environment.GetEnvironmentVariable("LEADERBOARDS_AZURE_SUBSCRIPTION_ID");

    public Build()
    {
        Npm("i -g azure-functions-core-tools@4 --unsafe-perm true");
    }

    Target Clean => _ => _
        .Before(Restore)
        .Executes(() =>
        {
            EnsureCleanDirectory(OutputDirectory);
        });

    Target Restore => _ => _
        .DependsOn(Clean)
        .Executes(() =>
        {
            DotNetRestore(_ => _.SetProjectFile(Solution));
        });

    Target Compile => _ => _
        .DependsOn(Restore)
        .Executes(() =>
        {
            DotNetBuild(_ => _
                .EnableNoRestore()
                .SetConfiguration(Configuration)
                .SetProjectFile(Solution));
        });

    Target Deploy => _ => _
        .DependsOn(Compile)
        .Requires(() => ResourceGroupName)
        .Executes(() =>
        {
            Func($"azure functionapp publish {AppName}", Solution.GetProject("LevelLeaderboards.Web")?.Directory);
        });

    Target InitializeAzure => _ => _
        .DependsOn(AzLogin)
        .DependsOn(CreateResourceGroup)
        .DependsOn(CreateServicePrincipal)
        .Executes(() =>
        {
        });


    Target AzLogin => _ => _
        .Executes(() =>
        {
            Az($"login");
        });


    Target CreateResourceGroup => _ => _
        .Requires(() => ResourceGroupName)
        .Requires(() => AzureLocation)
        .Executes(() =>
        {
            Az($"group create --name {ResourceGroupName} --location {AzureLocation}");
        });

    Target CreateServicePrincipal => _ => _
        .Requires(() => ResourceGroupName)
        .Requires(() => AzureSubscriptionId)
        .Executes(() =>
    {
        var spName = "playfablevelleaderboards";
        var role = "contributor";
        var scopes = $"/subscriptions/{AzureSubscriptionId}/resourceGroups/{ResourceGroupName}";
        var arguments = $"ad sp create-for-rbac --name {spName} --role {role} --scopes {scopes}";
        Az(arguments);
    });

}
