using Autofac;
using Autofac.Extensions.DependencyInjection;
using Conditionals.Core.Areas.Engine;
using Conditionals.Core.Areas.Events;
using Conditionals.Core.ConsoleDemos.Areas.Chaining;
using Conditionals.Core.ConsoleDemos.Areas.Conditions;
using Conditionals.Core.ConsoleDemos.Areas.Contexts;
using Conditionals.Core.ConsoleDemos.Areas.Events;
using Conditionals.Core.ConsoleDemos.Areas.JsonRules;
using Conditionals.Core.ConsoleDemos.Areas.Results;
using Conditionals.Core.ConsoleDemos.Areas.TenantsAndCulture;
using Conditionals.Core.ConsoleDemos.Common.Models;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;


namespace Conditionals.Core.ConsoleDemos;

internal class Program
{
    static async Task Main()
    {
        /*
            * Choose an IOC for the demos, Autofac or Microsoft container. I chose to use the concrete ConditionEngine as I only added an interface to the project to reduce the 
            * xml comments in the condition engine class, but you can configure apps to use either.
        */
        //var conditionEngine = ConfigureMicrosoftContainer().GetRequiredService<ConditionEngine>();
        var conditionEngine   = ConfigureAutofac().Resolve<ConditionEngine>();

        /*
            * Please review the code and comments within the code classes contained within the respective Area folders. 
        */
        //Areas - 01 - Contexts
        await ContextRunner.Start(conditionEngine);

        //Areas - 02 - Conditions
        await ConditionRunner.Start(conditionEngine);

        //Areas - 03 - Events
        await EventRunner.Start(conditionEngine);

        //Areas - 04 - Json Rules
        await JsonRunner.Start(conditionEngine);

        //Areas - 05 - Chaining
        await ChainingRunner.Start(conditionEngine);

        //Areas - 06 - Results
        await ResultsRunner.Start(conditionEngine);

        //Areas - 07 - Tenants and Culture
        await TenantsAndCultureRunner.Start(conditionEngine);
        Console.ReadLine();
    }


        
    
    private static ServiceProvider ConfigureMicrosoftContainer()
    {
        var appSettings = GetFakeAppSettings();

        return Host.CreateApplicationBuilder()
                .Services.AddHttpClient()
                         .AddSingleton(appSettings)
                            .AddTransient<IEventHandler<CheckTotalConditionEvent>, CheckTotalConditionEventDynamicHandler>()
                            .AddTransient<IEventHandler<CheckTotalRuleEvent>, CheckRuleEventDynamicHandler>()
                         //.AddTransient<IEventHandler<DeviceConditionEvent>, DeviceBatteryEventHandler>()
                         //.AddTransient<MyCustomGenericDIAwareEvaluator<Address>>()
                         .AddTransient<ProbeConditionEvaluator>()
                         .AddSingleton<ConditionEngine>(provider => new ConditionEngine(type => provider.GetRequiredService(type)))
                .BuildServiceProvider();


    }

    private static IContainer ConfigureAutofac()
    {
        var appSettings = GetFakeAppSettings();

        var serviceCollection = new ServiceCollection().AddHttpClient();

        var builder = new ContainerBuilder();

        builder.Populate(serviceCollection);
        builder.RegisterInstance(appSettings).SingleInstance();
        builder.RegisterType<CheckTotalConditionEventDynamicHandler>().As<IEventHandler<CheckTotalConditionEvent>>().InstancePerDependency();
        builder.RegisterType<CheckRuleEventDynamicHandler>().As<IEventHandler<CheckTotalRuleEvent>>().InstancePerDependency();
        //builder.RegisterType<DeviceBatteryEventHandler>().As<IEventHandler<DeviceConditionEvent>>().InstancePerDependency();
        //builder.RegisterType<MyCustomGenericDIAwareEvaluator<Address>>().InstancePerDependency();
        builder.RegisterType<ProbeConditionEvaluator>().InstancePerDependency();
        builder.Register(c => //Set up as shown!
        {
            var context = c.Resolve<IComponentContext>();
            return new ConditionEngine(type => context.Resolve(type));
        }).As<ConditionEngine>().SingleInstance();


        return builder.Build();

    }

    private static AppSettings GetFakeAppSettings()

        => new ("admin@demos.conditions.com", "Service API Endpoint", "Server=.;Initial Catalog=FAKE;Integrated Security= SSPI; Encrypt=false;MultipleActiveResultSets=True;");
}
