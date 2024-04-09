using Conditionals.Core.Areas.Conditions;
using Conditionals.Core.Areas.Engine;
using Conditionals.Core.Areas.Events;
using Conditionals.Core.Areas.Rules;
using Conditionals.Core.Common.Builders;
using Conditionals.Core.Common.Models;
using Conditionals.Core.Common.Seeds;
using Conditionals.Core.ConsoleDemos.Common.Models;

using static Conditionals.Core.ConsoleDemos.Common.Utilities.GeneralUtils;

namespace Conditionals.Core.ConsoleDemos.Areas.Events;

public class EventsAssociatedToRules(ConditionEngine conditionEngine)
{
    public readonly ConditionEngine _conditionEngine = conditionEngine;

    public async Task RunRuleEventHandlers()
    {
        /*
             * The process using and handling rule events is exactly the same as that of conditions. It is only the event data that is different.
             * You can have both rule events and condition events.
        */ 
        var order           = new Order(99, 99, DateOnly.FromDateTime(DateTime.Now), 1_250.00M);
        var conditionData   = ConditionDataBuilder.AddForAny(order).Create();
        var eventCondition  = new PredicateCondition<Order>("CheckTotalCondition", o => o.OrderTotal >= 5000, "No checks required for totals under 5000");
        var eventRule       = new Rule<None>("EventRule", None.Value, new ConditionSet<None>("EventConditionSet", None.Value, eventCondition), EventDetails.Create<CheckTotalRuleEvent>(EventWhenType.OnFailure));

        /*
            * For local event handlers we need to subscribe to the event that we are interested, we will receive and event subscription object. This subscription object
            * uses weak references to avoid any memory leaks, but the event can be unsubscribed via call the dispose method on the event subscription.  
        */
        var eventSubscription = _conditionEngine.SubscribeToEvent<CheckTotalRuleEvent>(CheckTotalRuleEventHandler);

        var ruleResult = await eventRule.Evaluate(_conditionEngine.EvaluatorResolver, conditionData, _conditionEngine.EventPublisher);

        WriteLine($"Finished evaluating the rule: {ruleResult.RuleName}, with the event when type of: {eventRule.RuleEventDetails!.EventWhenType}");
        WriteLine($"There is both a local handler and a dynamic handler that will process the event.");
        WriteLine();
        await Task.Delay(50);//make sure we don't dispose before the async handler is called (only needed for demo purposes)
        eventSubscription.Dispose();
    }
    public static async Task CheckTotalRuleEventHandler(CheckTotalRuleEvent theEvent, CancellationToken cancellationToken)
    {
        /*
            * Added a delay so that the info from both handlers do not get intertwined
        */
        await Task.Delay(25);//hopefully this will ensure that the dynamically invoked handler gets printed before this one
        await Console.Out.WriteLineAsync($"Received the event from {theEvent.SenderName} in the local handler, the rule evaluated to: {theEvent.IsSuccessEvent}.");
        await Console.Out.WriteLineAsync($"There were {theEvent.ExecutionExceptions.Count} execution exceptions.");

    }
}
