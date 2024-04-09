using Conditionals.Core.Areas.Events;

namespace Conditionals.Core.ConsoleDemos.Areas.Events;

public class CheckRuleEventDynamicHandler : IEventHandler<CheckTotalRuleEvent>
{
    /*
        * Your class just needs to implement the interface IEventHandler<TEvent> closing the generic with the type of event you wish to handle.
        * The class then needs registering with your DI container, please see the program file di container setup(s).
    */
    public async Task Handle(CheckTotalRuleEvent theEvent, CancellationToken cancellationToken = default)
    {
        /*
            * The rule events do not contain the condition data as they could contain many conditions.
            * These events mainly inform you about the success or failure of a rule. If there were any exceptions during the
            * the processing the of the rule they will be contained in the List<Exception> ExecutionExceptions property
        */ 
        
        await Console.Out.WriteLineAsync($"Received the event from {theEvent.SenderName} in the dynamic handler, the rule evaluated to: {theEvent.IsSuccessEvent}.");
        await Console.Out.WriteLineAsync("");
    }
}
