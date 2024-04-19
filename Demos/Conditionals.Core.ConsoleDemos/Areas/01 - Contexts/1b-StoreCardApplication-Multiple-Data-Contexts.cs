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
                         .AndAlso(new PredicateCondition<OrderHistoryView>("OrderCondition", o => o.TotalOrders >= 5, "You must have made at least five purchases against your account"));
        /*
            * conditions form boolean expression pairs that short-circuit using AndAlso (&&) or OrElse (||) i.e the above nesting has a left and right, with the left having a left and right
            * ((AgeCondition AndAlso CountryCondition) AndAlso OrderCondition). There can be any depth of these pairings which will be shown/discussed later.
        */ 
        
        var storeCardRule = new Rule<None>("StoreCreditCardRule",None.Value,new ConditionSet<None>("ApplicantRequirements",None.Value,conditions));

        /*
            * We need three separate data contexts for this rule so we will use the ConditionDataBuilder for readability. As there are three separate data types and only a single instance of each
            * we do not need to match the instance of a type to a condition by name, the AddForAny/AndForAny methods are fine. However, you could if you so wish use the 
            * AddForCondition or AndForCondition methods and specify the condition name that the data is for.
        */
  
        var customerID = 2;//Choose customer with ID 1,2,3, or 4
        var conditionData = ConditionDataBuilder.AddForAny(DemoData.GetAddress(customerID))
                                                    .AndForAny(DemoData.GetOrderHistory(customerID))
                                                        .AndForAny(DemoData.GetCustomer(customerID))
                                                            .Create();
        /*
            * The above could be written as
            * var conditionData2 = new ConditionData(new DataContext[] {new DataContext(Data: DemoData.GetAddress(customerID),ConditionName: ""),
            *                                                           new DataContext(DemoData.GetOrderHistory(customerID)),
            *                                                           new DataContext(DemoData.GetCustomer(customerID)) });                                                         
            * or
            * var conditionData = new ConditionData([new(DemoData.GetAddress(customerID)), new(DemoData.GetOrderHistory(customerID)), new(DemoData.GetCustomer(customerID))])
        */

        _conditionEngine.AddOrUpdateRule(storeCardRule);

        /*
             * The return type is a RuleResult<None> meaning its a result that has no success or failure values just a bool IsSuccess property.
             * As we are using chaining via the extension methods we can access the result directly from Action<RuleResult<None>> so in this instance
             * we can just discard the returned result.
        */

        _ = await _conditionEngine.EvaluateRule<None>(storeCardRule.RuleName, conditionData)
                                    .OnResult(success => WriteLine($"Applicant {customerID}, application approved in {success.RuleTimeMilliseconds}ms with {success.EvaluationCount} evaluations"),
                                                failure =>
                                                {
                                                    WriteLine($"Applicant {customerID} application rejected. {failure.EvaluationCount} evaluation(s) in {failure.RuleTimeMilliseconds}ms");
                                                    WriteLine($"Rejected due to: {String.Join("/r/n", failure.FailureMessages)}");

                                                });

    }
}
