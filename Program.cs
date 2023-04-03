using Pulumi;
using Pulumi.AzureNative.Resources;
using System.Collections.Generic;
using Pulumi.AzureNative.App;
using Pulumi.AzureNative.Web;
using Pulumi.AzureNative.Web.Inputs;
using Pulumi.AzureNative.Insights;

return await Pulumi.Deployment.RunAsync(() =>
{
    // Create an Azure Resource Group
    var resourceGroup = new ResourceGroup("sunit-pip-rg-bicep", new()
    {
        Location = "eastus",
        ResourceGroupName = "sunit-pip-rg-bicep",
    });
    var appServicePlan = new AppServicePlan("asp-pulumi-demo-sunit", new AppServicePlanArgs
    {
        ResourceGroupName = resourceGroup.Name,
        Kind = "App",
        Sku = new SkuDescriptionArgs
        {
            Tier = "Basic",
            Name = "B1",
        },
    });

    //Create an Azure App Insights
    var appInsights = new Component("appInsights-pulimi-demo-sunit", new ComponentArgs
    {
        ApplicationType = "web",
        Kind = "web",
        ResourceGroupName = resourceGroup.Name,
    });

   //Create Azure Web App to host application
    var app = new WebApp("webapp-pulumi-demo-sunit", new WebAppArgs
    {
        ResourceGroupName = resourceGroup.Name,
        ServerFarmId = appServicePlan.Id,
        SiteConfig = new SiteConfigArgs
        {
            AppSettings = {

                    new NameValuePairArgs{
                        Name = "APPINSIGHTS_INSTRUMENTATIONKEY",
                        Value = appInsights.InstrumentationKey
                    },
                    new NameValuePairArgs{
                        Name = "APPLICATIONINSIGHTS_CONNECTION_STRING",
                        Value = appInsights.InstrumentationKey.Apply(key => $"InstrumentationKey={key}"),
                    },
                    new NameValuePairArgs{
                        Name = "ApplicationInsightsAgent_EXTENSION_VERSION",
                        Value = "~2",
                    },
                },
        }
    }); 
});
