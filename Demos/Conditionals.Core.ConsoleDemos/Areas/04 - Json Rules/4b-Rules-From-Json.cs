using Conditionals.Core.Areas.Engine;
using Conditionals.Core.Common.Builders;
using Conditionals.Core.Common.Extensions;
using Conditionals.Core.ConsoleDemos.Common.Seeds;
using Conditionals.Core.ConsoleDemos.Common.StaticData;
using Conditionals.Core.ConsoleDemos.Common.Utilities;

using static Conditionals.Core.ConsoleDemos.Common.Utilities.GeneralUtils;

namespace Conditionals.Core.ConsoleDemos.Areas.JsonRules;

public class RulesFromJson(ConditionEngine conditionEngine)
{
    private readonly ConditionEngine _conditionEngine = conditionEngine;
    
    public async Task RunRuleWithoutAddingToEngine()
    {
        /*
            * You can use the static method RuleFromJson on the condition engine to create a rule from Json without it being added to the condition engine.  
            * We will read the json files from the JsonRules folder. These files get output to the ..\ Conditionals.Core.ConsoleDemos\bin\Debug\net8.0\JsonRules
            * each time the demo is run (in debug mode).
            * 
            * You need to know the rules value type for this method. 
         */

        var jsonString = await GeneralUtils.ReadJsonRuleFile(Path.Combine(DemoGlobalStrings.Json_Rules_Folder_Path, "DiscountRule.json"));
        
        WriteLine(jsonString);
   
        var customerData    = DemoData.GetCustomer(2);
        var conditionData   = ConditionDataBuilder.AddForAny(customerData).Create();

        var discountRuleFromJson = ConditionEngine.RuleFromJson<decimal>(jsonString);
        /*
            * The rule and one condition does have an event, the rule event is on failure, the condition is on either.
            * Lets use inline local handlers for a change. No dynamic handlers added for this demo.
            
        */
        
        var conditionSubscription = _conditionEngine.SubscribeToEvent<CustomerConditionEvent>(ConditionEventHandler);
        var ruleSubscription      = _conditionEngine.SubscribeToEvent<DecimalRuleEvent>(RuleEventHandler);

        _ = await discountRuleFromJson.Evaluate(_conditionEngine.EvaluatorResolver, conditionData, _conditionEngine.EventPublisher)
                        .OnSuccessOrFailure(r => WriteLine($"The result evaluated to {r.IsSuccess}, the last set to be evaluated was {r.ConditionSetChain!.SetName}"));
        
        conditionSubscription.Dispose();
        ruleSubscription.Dispose();

        async Task ConditionEventHandler(CustomerConditionEvent theEvent, CancellationToken cancellationToken)
        { 
            await Console.Out.WriteLineAsync($"Received the event from the condition {theEvent.SenderName}, which evaluated to {theEvent.IsSuccessEvent}");
        }

        async Task RuleEventHandler(DecimalRuleEvent theEvent, CancellationToken cancellationToken)
        {
            await Console.Out.WriteLineAsync($"Received the event from the rule {theEvent.SenderName}, which evaluated to {theEvent.IsSuccessEvent}, success value: {theEvent.SuccessValue}");
        }


        await Task.Delay(10);//Just in case to stop mingling of demo write lines
    }

    
}
