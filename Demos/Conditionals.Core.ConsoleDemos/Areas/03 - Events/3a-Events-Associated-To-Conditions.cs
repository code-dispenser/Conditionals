using Conditionals.Core.Areas.Conditions;
using Conditionals.Core.Areas.Engine;
using Conditionals.Core.Areas.Events;
using Conditionals.Core.Common.Builders;
using Conditionals.Core.ConsoleDemos.Common.Models;

using static Conditionals.Core.ConsoleDemos.Common.Utilities.GeneralUtils;

namespace Conditionals.Core.ConsoleDemos.Areas.Events;

public class EventsAssociatedToConditions(ConditionEngine conditionEngine)
{
    public readonly ConditionEngine _conditionEngine = conditionEngine;

    public async Task RunConditionEventHandlers()
    {
        /*
            * Events can be associated to individual conditions or to a rule.
            * All events are fire and forget tasks, any exceptions within the event handlers do not get propagated back to the caller and as such
            * have not impact on the processing of the rest of the conditions within the rule.
            * 
            * Events can be handled with local event handlers or what I term dynamic event handlers that get instantiated each time an event is raised/published.
         */

        /*
            * You associate events to conditions by using the static Create method of the EventDetails class.
            * Events can be raised/published either when the condition evaluates to true 'OnSuccess', evaluates to false 'OnFailure' or either the default
            * You need to create a class that implements and closes the generic ConditionEventBase<TContext> abstract class. The class merely serves as marker
            * for the type of event you want to receive. The class gets dynamically created and its properties populated within the conditions evaluate method once the 
            * the result it known.
        */

        /*
            * Although not seen yet, you can evaluate individual conditions without creating rules and condition sets. 
            * We will take this approach to reduce the code needed to show the use of events.
            
        */

        var order           = new Order(99,99,DateOnly.FromDateTime(DateTime.Now),5_250.00M);
        var conditionData   = ConditionDataBuilder.AddForAny(order).Create();
        var eventCondition  = new PredicateCondition<Order>("CheckTotalCondition", o => o.OrderTotal >= 5000, "No checks required for totals under 5000", EventDetails.Create<CheckTotalConditionEvent>());

        /*
            * For local event handlers we need to subscribe to the event that we are interested, we will receive and event subscription object. This subscription object
            * uses weak references to avoid any memory leaks, but the event can be unsubscribed via call the dispose method on the event subscription.  
        */
        var eventSubscription = _conditionEngine.SubscribeToEvent<CheckTotalConditionEvent>(CheckTotalConditionEventHandler);

        var conditionResult = await eventCondition.Evaluate(_conditionEngine.EvaluatorResolver, conditionData, _conditionEngine.EventPublisher);

        /*
            * Events are task based fire and forget so added a Task.Delay so we could get all the output to the console screen before
            * the next example 3b starts running and the output gets intermingled due to asynchrony
            * add a delay if you not seeing the handlers output await Task.Delay(20);
            * The class CheckTotalConditionEventDynamicHandler has been added to the DI container in the program class 
        */

        WriteLine($"Finished evaluating the condition: {eventCondition.ConditionName}, with the event when type of: {eventCondition.EventDetails!.EventWhenType}");
        WriteLine($"There is both a local handler and a dynamic handler that will process the event.");
        WriteLine();
        await Task.Delay(50);
        eventSubscription.Dispose();
    }
    
    public static async Task CheckTotalConditionEventHandler(CheckTotalConditionEvent theEvent, CancellationToken cancellationToken)
    {
        /*
            * Added a delay so that the info from both handlers do not get intertwined
        */ 
        await Task.Delay(25);//hopefully this will ensure that the dynamically invoked handler gets printed before this one

        await Console.Out.WriteLineAsync($"Received the event from {theEvent.SenderName} in the local handler, the condition evaluated to: {theEvent.IsSuccessEvent}.");
        await Console.Out.WriteLineAsync($"Serializing the order data resulted in: {(theEvent.ConversionException is null ? "no conversion error" : theEvent.ConversionException.Message)}");
        
        /*
            * The ConversionException gets set if there is an error trying to serialize the data using the default System.Text.Json serialization defaults ... 
            * It also gets set if there is an error trying to deserialize the json string back to the type of data serialized.
            * Given the serialization to the json each event handler gets its own copy of the data that was passed to the condition.
            * Non serializable objects will not stop the event process it just means the TryGetData will return false with an default out param
        */ 

        string deserializedData =  theEvent.TryGetData(out var order) ? order!.ToString() : theEvent.ConversionException!.Message;

        await Console.Out.WriteLineAsync(deserializedData);
       
    }

}
