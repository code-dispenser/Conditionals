using Conditionals.Core.Areas.Events;
using Conditionals.Core.ConsoleDemos.Common.Models;

namespace Conditionals.Core.ConsoleDemos.Areas.Events;

public class CheckTotalConditionEventDynamicHandler : IEventHandler<CheckTotalConditionEvent>
{
    /*
        * Your class just needs to implement the interface IEventHandler<TEvent> closing the generic with the type of event you wish to handle.
        * The class then needs registering with your DI container, please see the program file di container setup(s).
    */
    public async Task Handle(CheckTotalConditionEvent theEvent, CancellationToken cancellationToken = default)
    {

        await Console.Out.WriteLineAsync($"Received the event from {theEvent.SenderName} in the dynamic handler, the condition evaluated to: {theEvent.IsSuccessEvent}.");
        await Console.Out.WriteLineAsync($"Serializing the order data resulted in: {(theEvent.ConversionException is null ? "no conversion error" : theEvent.ConversionException.Message)}");
        /*
            * The ConversionException gets set if there is an error trying to serialize the data using the default Json ... 
            * It also gets set if there is an error trying to deserialize the json string back to the type of data serialized.
        */
        string deserializedData = theEvent.TryGetData(out var order) ? order!.ToString() : theEvent.ConversionException!.Message;

        await Console.Out.WriteLineAsync(deserializedData);
        await Console.Out.WriteLineAsync("");
    }
}
