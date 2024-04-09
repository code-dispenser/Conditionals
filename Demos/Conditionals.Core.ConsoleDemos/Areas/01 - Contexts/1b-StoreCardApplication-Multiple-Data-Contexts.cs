using Conditionals.Core.Areas.Conditions;
using Conditionals.Core.Areas.Engine;
using Conditionals.Core.Areas.Rules;
using Conditionals.Core.Common.Builders;
using Conditionals.Core.Common.Extensions;
using Conditionals.Core.Common.Models;
using Conditionals.Core.ConsoleDemos.Common.Models;
using Conditionals.Core.ConsoleDemos.Common.StaticData;

using static Conditionals.Core.ConsoleDemos.Common.Utilities.GeneralUtils;

namespace Conditionals.Core.ConsoleDemos.Areas.Contexts;

public class StoreCardApplicationMultipleDataContexts(ConditionEngine conditionEngine)
{
    private readonly ConditionEngine _conditionEngine = conditionEngine;

    public async Task CheckApplicant()
    {
        var conditions = new PredicateCondition<Customer>("AgeCondition", c => new DateTime(c.DOB.Year, c.DOB.Month, c.DOB.Day).AddYears(18) < DateTime.Now, "You must be over 18 to apply")
                         .AndAlso(new PredicateCondition<Address>("CountryCondition", a => a.Country == "United Kingdom", "You must be a resident of the United Kingdom"))
                         .AndAlso(new PredicateCondition<OrderHistoryView>("OrderCondition", o => o.TotalOrders >= 5, "ou must have made at least five purchases against your account"));
        /*
            * conditions form boolean expression pairs that short-circuit using AndAlso (&&) or OrElse (||) i.e the above nesting has a left and right, with the left having a left and right
            * ((AgeCondition AndAlso CountryCondition) AndAlso OrderCondition). There can be any depth of these pairings which will be shown/discussed later.
        */ 
        
        var storeCardRule = new Rule<None>("StoreCreditCardRule",None.Value,new ConditionSet<None>("ApplicantRequirements",None.Value,conditions));

        /*
            * We need three separate data contexts for this rule so we will use the ConditionDataBuilder. As these are three separate data types and only a single instance of each
            * we do not need match the type (instance of type) to the condition (by name). Again this will be discussed later, these can be added in any order.
        */
  
        var customerID = 2;//Choose customer with ID 1,2,3, or 4
        var conditionData = ConditionDataBuilder.AddForAny(DemoData.GetAddress(customerID))
                                                    .AndForAny(DemoData.GetOrderHistory(customerID))
                                                        .AndForAny(DemoData.GetCustomer(customerID))
                                                            .Create();

        _conditionEngine.AddOrUpdateRule(storeCardRule);

        /*
             * The return type is a RuleResult<None> meaning its a result that has no success or failure values just a bool IsSuccess property.
             * As we are using chaining via the extension methods we can access the result directly from Action<RuleResult<None>> so in this instance
             * we can just discard the returned result.
        */

        _ = await _conditionEngine.EvaluateRule<None>(storeCardRule.RuleName, conditionData)
                                    .OnResult(success => Console.WriteLine($"Applicant {customerID}, application approved in {success.RuleTimeMilliseconds}ms with {success.EvaluationCount} evaluations"),
                                                failure =>
                                                {
                                                    WriteLine($"Applicant {customerID} application rejected. {failure.EvaluationCount} evaluation(s) in {failure.RuleTimeMilliseconds}ms");
                                                    WriteLine($"Rejected due to: {String.Join("/r/n", failure.FailureMessages)}");

                                                });

    }
}
