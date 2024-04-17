using Conditionals.Core.Areas.Conditions;
using Conditionals.Core.Areas.Engine;
using Conditionals.Core.Areas.Rules;
using Conditionals.Core.Common.Builders;
using Conditionals.Core.Common.FormatConverters;
using Conditionals.Core.Common.Seeds;
using Conditionals.Core.ConsoleDemos.Common.Models;
using Conditionals.Core.ConsoleDemos.Common.StaticData;
using System.Text;
using static Conditionals.Core.ConsoleDemos.Common.Utilities.GeneralUtils;

namespace Conditionals.Core.ConsoleDemos.Areas.Results;

public class RuleResults(ConditionEngine conditionEngine)
{
    private readonly ConditionEngine _conditionEngine = conditionEngine;

    public async Task ReturningResults()
    {
        /*
            * The RuleResult holds the chain of condition set results which in turn holds the chain of condition results.
            * Given that you chain RuleResults a rule result also has a PreviousRuleResult property that can hold the 
            * RuleResult that may have been run before it and so on.
            * 
            * Rule results always return a boolean indicating success or failure and are required to have a return value.
            * We use the type None (None.Value) to indicate that we do not have a need for a returned value.
            * 
            * Rule results collate all of the information from the condition sets so at the root object you can get a list
            * of any failure messages or exceptions that may have occurred as well as to all of the timing information.
        */

        var studentRate     = new PredicateCondition<Customer>("IsStudent", c => c.CustomerType.ToString() == "Student", "Customer @{CustomerName} is not a student");
        var pensionerRate   = new PredicateCondition<Customer>("IsPensioner", c => c.CustomerType.ToString() == "Pensioner", "Customer @{CustomerName} is not a pensioner");
        var subscriberRate  = new PredicateCondition<Customer>("IsSubscriber", c => c.CustomerType.ToString() == "Subscriber", "Customer @{CustomerName} is not a paid subscriber");

        var studentSet      = new ConditionSet<decimal>("StudentRate", 0.10M, studentRate);
        var pensionerSet    = new ConditionSet<decimal>("PensionerRate", 0.15M, pensionerRate);
        var subscriberSet   = new ConditionSet<decimal>("SubscriberRate", 0.20M, subscriberRate);

        var discountRule = new Rule<Decimal>("DiscountRule", 0.00M, studentSet)
                                                        .OrConditionSet(pensionerSet)
                                                            .OrConditionSet(subscriberSet);

        /*
            * We saw the above rule in the demos 4a and 4b, which also had events attached. I have removed the events as they are not
            * required. Lets update the condition engine with the modified rule.
        */
        _conditionEngine.AddOrUpdateRule(discountRule);

        var conditionData = ConditionDataBuilder.AddForAny(DemoData.GetCustomer(2)).Create();

        var ruleResult = await _conditionEngine.EvaluateRule<decimal>(discountRule.RuleName, conditionData, new ConditionPrecedencePrinter(PrecedencePrintType.ConditionNameOnly));

        var stringBuilder = new StringBuilder();

        WriteLine($"The rule named {ruleResult.RuleName} evaluated to {ruleResult.IsSuccess}. The last set evaluated was {ruleResult.FinalSetName}.");
        WriteLine($"The rule was created for the tenant id {ruleResult.TenantID}.");
        WriteLine($"The rule return value was {ruleResult.ResultValue}, the failure value was set to {ruleResult.FailureValue}, the value from the final set was {ruleResult.SetValue}.");
        WriteLine($"The rule evaluated {ruleResult.EvaluationCount} conditions with {ruleResult.FailureMessages.Count} failures and {ruleResult.Exceptions.Count} exceptions.");
        WriteLine($"Failure messages: {String.Join("\r\n", ruleResult.FailureMessages)}");
        WriteLine($"The rule took {ruleResult.RuleTimeMilliseconds}ms to complete ({ruleResult.RuleTimeMicroseconds} microseconds).")
;       WriteLine();
        WriteLine("Drilling into the results we see:");
        WriteLine($"The previous rule result was {(ruleResult.PreviousRuleResult is null ? "[null]" : ruleResult.PreviousRuleResult.RuleName)}");
        WriteLine();

        var setInformation      = new List<string>();
        var conditionSetResult  = ruleResult.SetResultChain;
      

        while( conditionSetResult != null )
        {
            stringBuilder.AppendLine($"The condition set {conditionSetResult.SetName} with the set value of {conditionSetResult.SetValue} evaluated to {conditionSetResult.IsSuccess}");
            stringBuilder.AppendLine($"The set evaluated {conditionSetResult.TotalEvaluations} conditions with {conditionSetResult.FailureMessages.Count} failures and {ruleResult.Exceptions.Count} exceptions.");
            stringBuilder.AppendLine($"The set took {conditionSetResult.TotalMilliseconds}ms ({conditionSetResult.TotalMicroseconds} microseconds) to complete.");
            stringBuilder.AppendLine($"The precedence order of conditions was: {conditionSetResult.EvaluationPrecedence}");

            var conditionResult = conditionSetResult.ResultChain;
            stringBuilder.AppendLine($"Details:");
            
            while(conditionResult != null)
            {
                stringBuilder.AppendLine($"The condition {conditionResult.ConditionName} evaluated to {conditionResult.IsSuccess}");
                stringBuilder.AppendLine($"The total processing time was {conditionResult.TotalMicroseconds} microseconds with an evaluation time of {conditionResult.EvaluationMicroseconds} microseconds.");
                stringBuilder.AppendLine($"The condition was evaluated with the {conditionResult.EvaluatedBy} using the data for tenant id {conditionResult.TenantID}.");
                stringBuilder.AppendLine($"The data type was {Type.GetType(conditionResult.ContextType)!.FullName}");
                stringBuilder.AppendLine(conditionResult.EvaluationData!.ToString());
                stringBuilder.AppendLine();
                conditionResult = conditionResult.ResultChain;
            }
            
            setInformation.Insert(0, stringBuilder.ToString());
            stringBuilder.Clear();

            conditionSetResult = conditionSetResult.PreviousSetResult;
        }
        WriteLine("The sets were evaluated in the following order using OrElse logic between each set:\r\n");
        setInformation.ForEach(info => WriteLine(info));
    }
}
