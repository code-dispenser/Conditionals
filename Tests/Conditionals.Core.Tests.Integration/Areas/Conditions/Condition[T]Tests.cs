using Conditionals.Core.Areas.Conditions;
using Conditionals.Core.Areas.Engine;
using Conditionals.Core.Areas.Events;
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

namespace Conditionals.Core.Tests.Integration.Areas.Conditions;

public class ConditionTests
{
    [Fact]
    public async Task Should_raise_the_condition_event_if_set_to_on_success_and_the_condition_succeeds()
    {
        var condition = new Condition<Customer>("ConditionOne", "c => c.CustomerName == \"CustomerOne\"", ConditionType.LambdaPredicate, "Customer name should be CustomerOne",
                                                GlobalStrings.Predicate_Condition_Evaluator,[],EventDetails.Create<ConditionEventCustomer>());
        var engine    = new ConditionEngine();

        var senderName = String.Empty;

        var data = ConditionDataBuilder.AddForAny(StaticData.CustomerOne()).Create();

        _ = engine.SubscribeToEvent<ConditionEventCustomer>(HandleEvent);

        var conditionResult = await condition.Evaluate(engine.EvaluatorResolver, data, engine.EventPublisher);

        while (senderName == String.Empty) await Task.Delay(5);//events are fire and forget tasks so need delay before test just exits

        using (new AssertionScope())
        {
            conditionResult.IsSuccess.Should().BeTrue();
            senderName.Should().Be("ConditionOne");
        }

        async Task HandleEvent(ConditionEventCustomer conditionEvent, CancellationToken cancellationToken)
        {
            senderName = conditionEvent.SenderName;
            await Task.CompletedTask;
        }

    }

    [Fact]
    public async Task Any_unhandled_errors_raising_an_event_should_get_caught_and_added_to_the_conditions_result_exceptions_list()
    {
        var conditionEngine = new ConditionEngine();    
        var condition = new Condition<Customer>("ConditionOne", "c => c.CustomerName == \"CustomerOne\"", ConditionType.LambdaPredicate, "Customer name should be CustomerOne",
                                                GlobalStrings.Predicate_Condition_Evaluator, [], EventDetails.Create<BadConditionEventCustomer>());

        ConditionData data  = new(new DataContext[] { new DataContext(StaticData.CustomerOne(),"ConditionOne")});
        var conditionResult = await condition.Evaluate(conditionEngine.EvaluatorResolver,data, conditionEngine.EventPublisher);
        /*
            * Errors in events do not get passed back to the caller, however, this test causes an error in the RaiseEvent method
            * when trying to create the event to publish, which throws and exception in its constructor.
        */

        conditionResult.Should().Match<ConditionResult>(c => c.IsSuccess == true && c.Exceptions[0].GetType() == typeof(RaiseEventException));

    }

    [Fact]
    public async Task Any_evaluation_exception_should_get_added_to_the_events_exception_list()
    {
        var conditionEngine = new ConditionEngine();
        var condition = new Condition<Customer>("ConditionOne", "c => c.CustomerName == \"WrongValue\"", ConditionType.LambdaPredicate, "Customer name should not be CustomerOne",
                                                "ClosedGenericCustomerEvaluator", [], EventDetails.Create<ConditionEventCustomer>(EventWhenType.OnSuccess));
        // The ClosedGenericCustomerEvaluator returns an evaluation result of true but with an added SystemException
        
        Exception? executionException = null;

        ConditionData data = new(new DataContext[] { new DataContext(StaticData.CustomerOne(), "ConditionOne") });

        conditionEngine.RegisterCustomEvaluator("ClosedGenericCustomerEvaluator", typeof(ClosedGenericCustomerEvaluator));

        _= conditionEngine.SubscribeToEvent<ConditionEventCustomer>(HandleEvent);
        
        var conditionResult = await condition.Evaluate(conditionEngine.EvaluatorResolver, data, conditionEngine.EventPublisher);

        using(new AssertionScope())
        {
            await Task.Delay(10);//Just in case events are fire and forget

            conditionResult.Should().Match<ConditionResult>(r => r.IsSuccess == true && r.Exceptions[0].GetType() == typeof(SystemException));
            executionException.Should().NotBeNull().And.BeOfType<SystemException>();
        }

        async Task HandleEvent(ConditionEventCustomer theEvent, CancellationToken cancellation) 
        {
            executionException = theEvent.ExecutionExceptions[0];
            await Task.CompletedTask;
        }

    }
    [Fact]
    public async Task Any_exception_tyring_to_serialize_the_context_data_should_get_added_to_the_events_conversion_exception_property()
    {
        var conditionEngine = new ConditionEngine();
        var condition = new Condition<BadObject>("ConditionOne", "a => a.Name == \"WrongValue\"", ConditionType.LambdaPredicate, "The accountants name should be Accountant",
                                                GlobalStrings.Predicate_Condition_Evaluator, [], EventDetails.Create<ConditionEventBadObjectSerialization>(EventWhenType.OnFailure));
        // The ClosedGenericCustomerEvaluator returns an evaluation result of true but with an added SystemException

        Exception? conversionException = null;

        ConditionData data = new(new DataContext[] { new DataContext(new BadObject("Causes a problem")  , "ConditionOne") });

        _= conditionEngine.SubscribeToEvent<ConditionEventBadObjectSerialization>(HandleEvent);

        var conditionResult = await condition.Evaluate(conditionEngine.EvaluatorResolver, data, conditionEngine.EventPublisher);

        await Task.Delay(10);//Just in case events are fire and forget
        /*
            * Bad object causes a not implemented exception during serialization 
        */ 
        conversionException.Should().NotBeNull().And.BeOfType<NotImplementedException>();

        async Task HandleEvent(ConditionEventBadObjectSerialization theEvent, CancellationToken cancellation)
        {
            conversionException = theEvent.ConversionException;
            await Task.CompletedTask;
        }

    }
}
