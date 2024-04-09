using Autofac;
using Conditionals.Core.Areas.Engine;
using Conditionals.Core.Common.Exceptions;
using Conditionals.Core.Common.Seeds;
using Conditionals.Core.Tests.SharedDataAndFixtures.Evaluators;
using Conditionals.Core.Tests.SharedDataAndFixtures.Events;
using Conditionals.Core.Tests.SharedDataAndFixtures.Models;
using FluentAssertions;
using Xunit;

namespace Conditionals.Core.Tests.Integration.Areas.Engine;

public class ConditionEngineTests 
{
    private readonly ConditionEngine    _conditionEngine;

    public ConditionEngineTests()
        
        =>  _conditionEngine = ConfigureAutofac().Resolve<ConditionEngine>();

    private static IContainer ConfigureAutofac()
    {
        var strategy = new InjectedStrategy("Injected Value");
        var builder  = new ContainerBuilder();

        builder.RegisterInstance(strategy).SingleInstance();
        builder.RegisterType<OpenGenericDIRegisteredEvaluator<Customer>>().InstancePerDependency();
        builder.RegisterType<ClosedGenericDIRegisteredEvaluator>().InstancePerDependency();
        builder.Register<ConditionEngine>(c =>
        {
            var context = c.Resolve<IComponentContext>();
            return new ConditionEngine(type => context.Resolve(type));
        }).SingleInstance();


        return builder.Build();
    }

    [Fact]
    public void Should_be_able_to_register_and_use_open_generic_evaluators_requiring_dependency_injection()
    {
        _conditionEngine.RegisterCustomEvaluatorForDependencyInjection("OpenGenericDIRegisteredEvaluator", typeof(OpenGenericDIRegisteredEvaluator<>));

        var evaluator = (OpenGenericDIRegisteredEvaluator<Customer>) _conditionEngine.EvaluatorResolver("OpenGenericDIRegisteredEvaluator", typeof(Customer));

        evaluator.InjectedStrategy.StrategyValue.Should().Be("Injected Value");
    }
    [Fact]
    public void Should_be_able_to_register_and_use_closed_generic_evaluators_requiring_dependency_injection()
    {
        _conditionEngine.RegisterCustomEvaluatorForDependencyInjection("ClosedGenericDIRegisteredEvaluator", typeof(ClosedGenericDIRegisteredEvaluator));

        var evaluator = (ClosedGenericDIRegisteredEvaluator)_conditionEngine.EvaluatorResolver("ClosedGenericDIRegisteredEvaluator", typeof(Customer));

        evaluator.InjectedStrategy.StrategyValue.Should().Be("Injected Value");
    }

    [Fact]
    public void Tyring_to_get_an_evaluator_not_in_the_di_container_or_cache_throw_a_missing_condition_evaluator_exception()
    
       => FluentActions.Invoking(() => _conditionEngine.EvaluatorResolver("nonexitentevaluator", typeof(Person)))
                        .Should().ThrowExactly<MissingConditionEvaluatorException>();
    

    [Fact]
    public async Task Should_be_able_to_publish_an_event()
    {
        var eventSubscription = _conditionEngine.SubscribeToEvent<RuleEventInt>(HandleRuleEvent);
        var ruleEvent         = new RuleEventInt("Test", false, 42, 10, GlobalStrings.Default_TenantID, []);
        var failureValue      = 0;

        _conditionEngine.EventPublisher(ruleEvent,CancellationToken.None);

        while (failureValue == 0) await Task.Delay(5);//events are fire and forget tasks

        failureValue.Should().Be(10);

        eventSubscription.Dispose();

        async Task HandleRuleEvent(RuleEventInt theEvent, CancellationToken cancellationToken)
        {
            failureValue = theEvent.FailureValue;
        
            await Task.CompletedTask;
        }

       
    }
}
