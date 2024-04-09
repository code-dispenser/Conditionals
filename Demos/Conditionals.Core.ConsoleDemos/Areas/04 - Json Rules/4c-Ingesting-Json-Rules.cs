using Conditionals.Core.Areas.Engine;
using Conditionals.Core.Common.Builders;
using Conditionals.Core.Common.Extensions;
using Conditionals.Core.ConsoleDemos.Common.Seeds;
using Conditionals.Core.ConsoleDemos.Common.StaticData;
using Conditionals.Core.ConsoleDemos.Common.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using static Conditionals.Core.ConsoleDemos.Common.Utilities.GeneralUtils;

namespace Conditionals.Core.ConsoleDemos.Areas.JsonRules;

public class IngestingJsonRules(ConditionEngine conditionEngine)
{
    private readonly ConditionEngine _conditionEngine = conditionEngine;

    public async Task RunIngestingJsonRules()
    {
        /*
            * The condition engine can ingest json rules in one of two ways.
            * It can ingest the rules given a type argument or it can first load the json to obtain the type argument by inspecting and using the value 
            * held in the ValueTypeName property after which it will use reflection to call the IngestRuleFromJson<T> with the type argument via reflection.
            * the typed method.
        */
        
        var jsonString = await GeneralUtils.ReadJsonRuleFile(Path.Combine(DemoGlobalStrings.Json_Rules_Folder_Path, "DiscountRule.json"));

        _conditionEngine.IngestRuleFromJson<decimal>(jsonString);
        /*
            * The discount rule has now been converted to a Rule<T> and added to the cache engines cache using its AddOrUpdateRule<T> method. 
            * Lets use the customer with the ID of 3 this time. 
        */
        var customerData  = DemoData.GetCustomer(3);
        var conditionData = ConditionDataBuilder.AddForAny(customerData).Create(); 

        var result = await _conditionEngine.EvaluateRule<decimal>("DiscountRule", conditionData);
        /*
            * Lets chain the OnSuccess and OnFailure methods instead of using OnResult and passing both actions at once. 
            * Only one will get call if its a failure then the OnSuccess will just return immediately for the OnFailure
            * If its a success the OnSuccess will get called and then it will be passed to OnFailure which will not run
            * its action, it will just pass the result to the next item in the chain if there is one etc.
        */
        _ = result.OnSuccess(r => WriteLine($"The success value was {r.SetValue} from the last evaluated set {r.ConditionSetChain!.SetName}"))
                        .OnFailure(r => WriteLine($"The failure value was {r.FailureValue} due to {String.Concat("\r\n",String.Join("\r\n", r.FailureMessages),"\r\n")}"));

        WriteLine("Without the type argument and with different customer data.");
        /*
            * Lets load the same rule again without the type argument 
        */ 

        _conditionEngine.IngestRuleFromJson(jsonString);

        conditionData = ConditionDataBuilder.AddForAny(DemoData.GetCustomer(4)).Create();//different customer 

        _ = await _conditionEngine.EvaluateRule<decimal>("DiscountRule", conditionData)
                                        .OnResult(act_onSuccess:  r => WriteLine($"The success value was {r.SetValue} from the last evaluated set {r.ConditionSetChain!.SetName}, no. of evaluations {r.EvaluationCount}"),
                                                  act_onFailure:  r => WriteLine($"The failure value was {r.FailureValue} due to {String.Concat("\r\n", String.Join("\r\n", r.FailureMessages))}"));
    }
}
