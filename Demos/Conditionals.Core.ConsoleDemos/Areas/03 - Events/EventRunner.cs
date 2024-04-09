using Conditionals.Core.Areas.Engine;
using static Conditionals.Core.ConsoleDemos.Common.Utilities.GeneralUtils;

namespace Conditionals.Core.ConsoleDemos.Areas.Events;

public static class EventRunner
{
    public static async Task Start(ConditionEngine conditionEngine)
    {
        WriteLineSeparator();
        WriteLine("3a-Events-Associated-To-Conditions");
        WriteLine();

        await new EventsAssociatedToConditions(conditionEngine).RunConditionEventHandlers();

        WriteLineSeparator();
        WriteLine("3b-Events-Associated-To-Rules");
        WriteLine();

        await new EventsAssociatedToRules(conditionEngine).RunRuleEventHandlers();    
    }
}
