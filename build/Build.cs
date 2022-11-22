using System;
using System.Linq;
using System.Reflection.PortableExecutable;
using Newtonsoft.Json;
using Nuke.Common;
using Nuke.Common.CI;
using Nuke.Common.Execution;
using Nuke.Common.IO;
using Nuke.Common.ProjectModel;
using Nuke.Common.Tooling;
using Nuke.Common.Utilities.Collections;
using static Nuke.Common.EnvironmentInfo;
using static Nuke.Common.IO.FileSystemTasks;
using static Nuke.Common.IO.PathConstruction;

class Build : NukeBuild
{
    /// Support plugins are available for:
    ///   - JetBrains ReSharper        https://nuke.build/resharper
    ///   - JetBrains Rider            https://nuke.build/rider
    ///   - Microsoft VisualStudio     https://nuke.build/visualstudio
    ///   - Microsoft VSCode           https://nuke.build/vscode

    public static int Main () => Execute<Build>(x => x.Compile);


    [PathExecutable]
    readonly Tool Az;

    [Parameter("Configuration to build - Default is 'Debug' (local) or 'Release' (server)")]
    readonly Configuration Configuration = IsLocalBuild ? Configuration.Debug : Configuration.Release;

    [Parameter("The name of the Azure Resource Group to be used")]
    readonly string ResourceGroupName = "pfleaderboards-rg";   //Environment.GetEnvironmentVariable("LEADERBOARDS_RESOURCE_GROUP_NAME");

    [Parameter("The Azure Region to deploy resources to")]
    readonly string AzureLocation  = Environment.GetEnvironmentVariable("LEADERBOARDS_AZURE_LOCATION");

    [Parameter("The Azure Subscription ID to deploy into")]
    readonly string AzureSubscriptionId  = Environment.GetEnvironmentVariable("LEADERBOARDS_AZURE_SUBSCRIPTION_ID");

    Target Clean => _ => _
        .Before(Restore)
        .Executes(() =>
        {
        });

    Target Restore => _ => _
        .Executes(() =>
        {
        });

    Target Compile => _ => _
        .DependsOn(Restore)
        .Executes(() =>
        {
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
