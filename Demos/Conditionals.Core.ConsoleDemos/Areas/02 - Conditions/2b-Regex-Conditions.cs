using Conditionals.Core.Areas.Conditions;
using Conditionals.Core.Areas.Engine;
using Conditionals.Core.Areas.Rules;
using Conditionals.Core.Common.Extensions;
using Conditionals.Core.Common.FormatConverters;
using Conditionals.Core.Common.Models;
using Conditionals.Core.Common.Seeds;
using Conditionals.Core.ConsoleDemos.Common.Models;
using Conditionals.Core.ConsoleDemos.Common.StaticData;
using System.Text.RegularExpressions;

using static Conditionals.Core.ConsoleDemos.Common.Utilities.GeneralUtils;

namespace Conditionals.Core.ConsoleDemos.Areas.Conditions;

public class RegexConditions(ConditionEngine conditionEngine)
{
    private readonly ConditionEngine _conditionEngine = conditionEngine;

    public async Task RunRegexConditions()
    {
        /*
            * There is also a built-in RegexCondition<TContext> that can be used to match a regex pattern against a property.
            * Instead of capturing a lambda predicate expression an expression is used to get the objects property path.
            * You can pass the desired combination of the .net RegexOptions if necessary.
        */

        var customerConditions = new RegexCondition<CustomerInfo>("TownCondition", a => a.Address.TownCity, "^(?=.{3,100}$)[A-Z](?!.* {2})(?!.*'{2})(?!.*-{2})[\\-A-Za-z ']+[a-z]+$",
                                                                "The Town name must start with a capital letter, be a minimum of 3 characters in length with no double dashes, double spaces or double apostrophes")

                                    .AndAlso(new RegexCondition<CustomerInfo>("CountryCondition", a => a.Address.Country, "^[A-Z](?!.* {2})[A-Z ]{1,98}[A-Z]$",
                                                                              "The Country name should be in upper case with no double spaces, with a minimum length of 3", RegexOptions.IgnoreCase))

                                    .AndAlso(new RegexCondition<CustomerInfo>("AccountNoCondition", a => a.AccountNo, "^\\d{5}$", "Account number should be five numbers"));

        var regexRules      = new Rule<None>("RegexRule",None.Value,new ConditionSet<None>("RegexSet",None.Value,customerConditions));
        var conditionData   = ConditionData.SingleContext(DemoData.GetCustomerInfo(3));

        _conditionEngine.AddOrUpdateRule(regexRules);

        //customer 3 country of residence is "United States" both upper and lower case characters.

        var result = await _conditionEngine.EvaluateRule<None>(regexRules.RuleName, conditionData, new ConditionPrecedencePrinter(PrecedencePrintType.ExpressionToEvaluateOnly))
                                                .OnSuccess(r => WriteLine($"The regex options altered the second condition to make it pass. No of evaluations {r.EvaluationCount}"))
                                                    .OnFailure(r => WriteLine("You altered the patterns and/or code and made it fail"));

        /*
            * Instead of using the OnResult extension and passing in two actions for success and failure you can chain results and use.
            * the separate OnSuccess and OnFailure methods that only accept a single action. 
            * you can also capture the result in conjunction with the extension methods
         */

        WriteLine(String.Concat("\r\n", result.SetResultChain!.EvaluationPrecedence));
    }
}
