using Conditionals.Core.Areas.Conditions;
using Conditionals.Core.Areas.Engine;
using Conditionals.Core.Common.Builders;
using Conditionals.Core.Common.FormatConverters;
using Conditionals.Core.Common.Models;
using Conditionals.Core.Common.Seeds;
using Conditionals.Core.ConsoleDemos.Common.Models;
using Conditionals.Core.ConsoleDemos.Common.StaticData;
using static Conditionals.Core.ConsoleDemos.Common.Utilities.GeneralUtils;

namespace Conditionals.Core.ConsoleDemos.Areas.Results;

public class ConditionSetResults(ConditionEngine conditionEngine)
{
    private readonly ConditionEngine _conditionEngine = conditionEngine;

    public async Task CollatingSetResult()
    {
        /*
            * The purpose of the ConditionSet is to provide both a grouping of conditions and a success value for the parent rule.
            * The condition set collates any failure messages, exceptions and timing information from the individual condition results.
            * The condition set result houses the EvaluationPrecedence property that can hold the order of precedence that the conditions
            * within the set were processed.
            * 
            * The condition set has a ResultChain property that holds the chain of condition results as well a PreviousSetResult that
            * holds the previous evaluated set of conditions. 
        */

        /*
            * Using the conditions we saw in 1b, 
        */ 

        var conditions = new PredicateCondition<Customer>("AgeCondition", c => new DateTime(c.DOB.Year, c.DOB.Month, c.DOB.Day).AddYears(18) < DateTime.Now, "You must be over 18 to apply")
                 .AndAlso(new PredicateCondition<Address>("CountryCondition", a => a.Country == "United Kingdom", "You must be a resident of the United Kingdom"))
                 .AndAlso(new PredicateCondition<OrderHistoryView>("OrderCondition", o => o.TotalOrders >= 5, "ou must have made at least five purchases against your account"));

        var applicationSet = new ConditionSet<None>("ApplicantRequirements", None.Value, conditions);

        var customerID = 3;//Choose customer with ID 1,2,3, or 4
        var conditionData = ConditionDataBuilder.AddForAny(DemoData.GetAddress(customerID))
                                                    .AndForAny(DemoData.GetOrderHistory(customerID))
                                                        .AndForAny(DemoData.GetCustomer(customerID))
                                                            .Create();

        var precedencePrinter = new ConditionPrecedencePrinter(PrecedencePrintType.ConditionNameOnly);
        /*
            * Lets use the condition engine this time although we could just pass in a delegate lambda like 6a
        */

        var conditionSetResult = await applicationSet.Evaluate(_conditionEngine.EvaluatorResolver, conditionData, null, precedencePrinter);

        WriteLine($"The set: {conditionSetResult.SetName} evaluated to {conditionSetResult.IsSuccess} with a returning set value of: {conditionSetResult.SetValue}");
        WriteLine($"The order of precedence was: {conditionSetResult.EvaluationPrecedence}");
        WriteLine($"The set took a total of {conditionSetResult.TotalMilliseconds}ms to complete {conditionSetResult.TotalEvaluations} condition evaluations and build the result.");
        WriteLine($"There were {conditionSetResult.FailureMessages.Count} failure messages and {conditionSetResult.ExecutionExceptions.Count} execution exceptions");

        var conditionResult = conditionSetResult.ResultChain;
        var conditionNames  = String.Empty;
        
        while(conditionResult != null)
        {
            conditionNames = String.Concat($"({conditionResult.IsSuccess}) {conditionResult.ConditionName}", String.Concat(" <-- ", conditionNames));
            conditionResult = conditionResult.ResultChain;
        }
        WriteLine($"Result chain: {conditionNames}");
        WriteLine($"Previous set result {(conditionSetResult.PreviousSetResult is null ? "[null]" : conditionSetResult.PreviousSetResult.SetName)}");

    }
}
