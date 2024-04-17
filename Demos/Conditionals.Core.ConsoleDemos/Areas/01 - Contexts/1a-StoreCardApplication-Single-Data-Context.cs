using Conditionals.Core.Areas.Conditions;
using Conditionals.Core.Areas.Engine;
using Conditionals.Core.Areas.Rules;
using Conditionals.Core.Common.Extensions;
using Conditionals.Core.Common.Models;
using Conditionals.Core.ConsoleDemos.Common.Models;
using Conditionals.Core.ConsoleDemos.Common.StaticData;

using static Conditionals.Core.ConsoleDemos.Common.Utilities.GeneralUtils;

namespace Conditionals.Core.ConsoleDemos.Areas.Contexts;

public class StoreCardApplicationSingleContext(ConditionEngine conditionEngine)
{
    private readonly ConditionEngine _conditionEngine = conditionEngine;

    public async Task CheckApplicant()
    {
        /*
            * The units that get evaluated are conditions, these conditions can be of varying types such as lambda predicates and can be grouped and combined with others in a multitude of ways.
            * Conditions are added to a ConditionSet, condition sets can hold a value that is used for the success result value of a rule.
            * A rule can hold many condition sets. Condition sets within rules are evaluated using short-circuit Or logic i.e if the first set succeeds then the other sets are not run/evaluated.
            * Every Rule and ConditionSet must have a return value if no return value is required other than knowing that the rule succeeded or failed you can use the None type for the return value.
            * A rule can have a failure value. A RuleResult has both a set value and failure value, the failure value comes from the rule the set value comes from the last evaluated set.
            * As sets use short-circuit logic the last set could have returned either a success or failure, so the term set value is used.
            * The RuleResult will return a ResultValue which will be the SetValue if successful or the FailureValue if the rule evaluated to false.
            * Every condition needs a data context to be evaluated against. 
        */

        var applicantCondition = new PredicateCondition<StoreCardApplication>("ApplicantCondition", s => s.Age >= 18 && s.CountryOfResidence == "United Kingdom" && s.TotalOrders >= 5,
                                                                              failureMessage: "To be eligible for a store credit card you need to be over 18, living in the United Kingdom and have at least 5 completed orders against your account");
        
        var applicationSet  = new ConditionSet<None>("ApplicantRequirements",None.Value, applicantCondition);
        var storeCardRule   = new Rule<None>("StoreCreditCardRule",None.Value, applicationSet);
        var applicantData   = DemoData.StoreCardApplicationForCustomer(1);//choose customer with ID 1,2,3 or 4
        
        /*
            * In order to evaluate a rule and/or condition set and/or a condition you must provide an instance of ConditionData which could contain one or more differing data types. 
            * This can be done manually or via the ConditionDataBuilder. For rules/conditions only requiring a single data context the ConditionData class  
            * has a SingleContext factory method that simplifies creation.
         */
        
        var conditionData = ConditionData.SingleContext(applicantData!);//equivalent to: new ConditionData(new DataContext[] { new DataContext(Data:applicantData!, ConditionName:"") });

        /*
            * You can evaluate a rule using the rules evaluate method but for this demo we will use the engines evaluate method i.e adding a rule to the engines cache of rules and then
            * evaluating the cached rule which will be the majority of use cases, especially when used in in conjunction with rules obtained/updated from json files/strings.
        */

        _conditionEngine.AddOrUpdateRule(storeCardRule);

        var ruleResult = await _conditionEngine.EvaluateRule<None>(storeCardRule.RuleName, conditionData);

        /*
            * The RuleResult is a composition of condition set and condition results.
            * Instead of using if/else statements there are various extension methods such as OnSuccess, OnFailure and OnResult to simplify/chain various actions. 
            * These are in both Task and non Task variants i.e you can chain directly on awaited results shown in other examples.
        */

        ruleResult.OnResult(act_onSuccess: result => Console.WriteLine($"Applicant {applicantData!.CustomerID}, application approved, return value {result.ResultValue}."),
                            act_onFailure: result =>
                            {
                                WriteLine($"Applicant {applicantData!.CustomerID} application rejected: {result.FailureMessages[0]}");
                                WriteLine($"No. evaluations: {result.EvaluationCount}");
                                WriteLine($"The result was {result.IsSuccess} with a return value of {result.ResultValue}");
                            });
                         

    }

}
