using Conditionals.Core.Areas.Conditions;
using Conditionals.Core.Areas.Engine;
using Conditionals.Core.Areas.Events;
using Conditionals.Core.Areas.Rules;
using Conditionals.Core.Common.Builders;
using Conditionals.Core.Common.Exceptions;
using Conditionals.Core.Common.Models;
using Conditionals.Core.Common.Seeds;
using Conditionals.Core.Tests.SharedDataAndFixtures.Data;
using Conditionals.Core.Tests.SharedDataAndFixtures.Evaluators;
using Conditionals.Core.Tests.SharedDataAndFixtures.Events;
using Conditionals.Core.Tests.SharedDataAndFixtures.Models;
using FluentAssertions;
using FluentAssertions.Execution;
using Xunit;

namespace Conditionals.Core.Tests.Integration.Areas.Rule;

public class RuleTests
{
    [Fact]
    public async Task Any_unhandled_errors_raising_an_event_should_not_change_the_evaluation_results_with_the_exception_being_added_to_the_exceptions_list()
    {
        var conditionEngine = new ConditionEngine();
        var conditionOne    = new PredicateCondition<Customer>("CustomerName", c => c.CustomerName == "WrongName", "CustomerName should be WrongName");
        var conditionSetOne = new ConditionSet<int>("SetOne", 20, conditionOne);

        var conditionTwo    = new PredicateCondition<Person>("PersonAge", p => p.Age < 18, "Person should be under 18");
        var conditionSetTwe = new ConditionSet<int>("SetTwo", 30, conditionTwo);

        var rule = new Rule<int>("RuleOne", 10, conditionSetOne, EventDetails.Create<BadEventInt>(EventWhenType.OnSuccessOrFailure))
                                    .OrConditionSet(conditionSetTwe);

        ConditionData data = new([new(StaticData.CustomerOne()), new(StaticData.PersonUnder18())]);

        var ruleResult = await rule.Evaluate(conditionEngine.EvaluatorResolver, data, conditionEngine.EventPublisher);
        /*
            * Errors in events do not get passed back to the caller, however, this test causes an error in the RaiseEvent method
            * when trying to create the event to publish, which throws and exception in its constructor.
        */
        ruleResult.Should().Match<RuleResult<int>>(r => r.IsSuccess == true && r.EvaluationCount == 2 && r.Exceptions[0].GetType() == typeof(RaiseEventException));

    }
    [Fact]
    public async Task Should_raise_the_rule_event_if_set_to_on_success_and_the_rule_succeeds()
    {
        var condition = new PredicateCondition<Customer>("ConditionOne", c => c.CustomerName == "CustomerOne", "Customer name should be CustomerOne");
        var rule      = new Rule<int>("RuleOne", 0, new ConditionSet<int>("SetOne", 42, condition), EventDetails.Create<RuleEventInt>(EventWhenType.OnSuccess));
        var engine    = new ConditionEngine();

        var senderName = String.Empty;

        var data = ConditionDataBuilder.AddForAny(StaticData.CustomerOne()).Create();

        _ = engine.SubscribeToEvent<RuleEventInt>(HandleRuleEvent);

        var ruleResult = await rule.Evaluate(engine.EvaluatorResolver, data, engine.EventPublisher);

        while (senderName == String.Empty) await Task.Delay(5);//events are fire and forget tasks so need delay before test just exits

        using (new AssertionScope())
        {
            ruleResult.IsSuccess.Should().BeTrue();
            senderName.Should().Be("RuleOne");
        }

        async Task HandleRuleEvent(RuleEventInt ruleEvent, CancellationToken cancellationToken)
        {
            senderName = ruleEvent.SenderName;
            await Task.CompletedTask;
        }

    }

    [Fact]
    public async Task Should_not_raise_the_rule_event_if_set_to_on_success_and_the_rule_fails()
    {
        var condition = new PredicateCondition<Customer>("ConditionOne", c => c.CustomerName == "NotCustomerOne", "Customer name should be NotCustomerOne");
        var rule = new Rule<int>("RuleOne", 0, new ConditionSet<int>("SetOne", 42, condition), EventDetails.Create<RuleEventInt>(EventWhenType.OnSuccess));
        var engine = new ConditionEngine();

        var senderName = String.Empty;

        var data = ConditionDataBuilder.AddForAny(StaticData.CustomerOne()).Create();

        _ = engine.SubscribeToEvent<RuleEventInt>(HandleRuleEvent);

        var ruleResult = await rule.Evaluate(engine.EvaluatorResolver, data, engine.EventPublisher);

        await Task.Delay(5);//events are fire and forget tasks so need delay before test just exits

        using (new AssertionScope())
        {
            ruleResult.IsSuccess.Should().BeFalse();
            senderName.Should().BeEmpty();
        }

        async Task HandleRuleEvent(RuleEventInt ruleEvent, CancellationToken cancellationToken)
        {
            senderName = ruleEvent.SenderName;
            await Task.CompletedTask;
        }

    }
    [Fact]
    public async Task Should_not_raise_the_rule_event_if_set_to_on_failure_and_the_rule_succeeds()
    {
        var condition = new PredicateCondition<Customer>("ConditionOne", c => c.CustomerName == "CustomerOne", "Customer name should be CustomerOne");
        var rule      = new Rule<int>("RuleOne", 0, new ConditionSet<int>("SetOne", 42, condition), EventDetails.Create<RuleEventInt>(EventWhenType.OnFailure));
        var engine    = new ConditionEngine();

        var senderName = String.Empty;

        var data = ConditionDataBuilder.AddForAny(StaticData.CustomerOne()).Create();

        _ = engine.SubscribeToEvent<RuleEventInt>(HandleRuleEvent);

        var ruleResult = await rule.Evaluate(engine.EvaluatorResolver, data, engine.EventPublisher);

        await Task.Delay(5);//events are fire and forget tasks so need delay before test just exits

        using (new AssertionScope())
        {
            ruleResult.IsSuccess.Should().BeTrue();
            senderName.Should().BeEmpty();
        }

        async Task HandleRuleEvent(RuleEventInt ruleEvent, CancellationToken cancellationToken)
        {
            senderName = ruleEvent.SenderName;
            await Task.CompletedTask;
        }

    }
    [Fact]
    public async Task Should_not_raise_the_rule_event_if_the_event_when_type_is_never()
    {
        var condition   = new PredicateCondition<Customer>("ConditionOne", c => c.CustomerName == "CustomerOne", "Customer name should be CustomerOne");
        var rule        = new Rule<int>("RuleOne", 0, new ConditionSet<int>("SetOne", 42, condition), EventDetails.Create<RuleEventInt>(EventWhenType.Never));//gets set to never if there was a problem converting from json
        var engine      = new ConditionEngine();

        var senderName = String.Empty;

        var data = ConditionDataBuilder.AddForAny(StaticData.CustomerOne()).Create();

        _ = engine.SubscribeToEvent<RuleEventInt>(HandleRuleEvent);

        _ = await rule.Evaluate(engine.EvaluatorResolver, data, engine.EventPublisher);

        await Task.Delay(20);//events are fire and forget tasks so need delay before test just exits

        senderName.Should().BeEmpty();

        async Task HandleRuleEvent(RuleEventInt ruleEvent, CancellationToken cancellationToken)
        {
            senderName = ruleEvent.SenderName;
            await Task.CompletedTask;
        }

    }

    [Fact]
    public async Task Any_execution_exceptions_should_be_added_to_the_rule_events_exception_list()
    {
        var condition   = new CustomPredicateCondition<Customer>("ConditionOne", c => c.CustomerName == "CustomerOne", "Customer name should be CustomerOne", "ClosedGenericCustomerEvaluator");
        var rule        = new Rule<int>("RuleOne", 0, new ConditionSet<int>("SetOne", 42, condition), EventDetails.Create<RuleEventInt>());
        var engine      = new ConditionEngine();

        Exception? executionException = null;

        var data = ConditionDataBuilder.AddForAny(StaticData.CustomerOne()).Create();

        engine.RegisterCustomEvaluator("ClosedGenericCustomerEvaluator", typeof(ClosedGenericCustomerEvaluator));
        //returns a true evaluation result but with a system exception

        _ = engine.SubscribeToEvent<RuleEventInt>(HandleRuleEvent);

        _ = await rule.Evaluate(engine.EvaluatorResolver, data, engine.EventPublisher);

        await Task.Delay(20);//events are fire and forget tasks so need delay before test just exits

        executionException.Should().BeOfType<SystemException>();

        async Task HandleRuleEvent(RuleEventInt ruleEvent, CancellationToken cancellationToken)
        {
            executionException = ruleEvent.ExecutionExceptions[0];
            await Task.CompletedTask;
        }

    }

}
