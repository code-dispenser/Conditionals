using Conditionals.Core.Areas.Conditions;
using Conditionals.Core.Areas.Engine;
using Conditionals.Core.Areas.Events;
using Conditionals.Core.Areas.Rules;
using Conditionals.Core.Common.Models;
using Conditionals.Core.Common.Seeds;
using Conditionals.Core.ConsoleDemos.Common.Models;
using Conditionals.Core.ConsoleDemos.Common.Seeds;
using Conditionals.Core.ConsoleDemos.Common.Utilities;

using static Conditionals.Core.ConsoleDemos.Common.Utilities.GeneralUtils;

namespace Conditionals.Core.ConsoleDemos.Areas.JsonRules;

public class OutputToJson()
{

    public static void RunOutputToJson()
    {
        /*
            * You can create your rules in code, test and then output them to json as a starting point rather than trying to create the json manually. 
        */

        var studentRate     = new PredicateCondition<Customer>("IsStudent", c => c.CustomerType.ToString() == "Student", "Customer @{CustomerName} is not a student",EventDetails.Create<CustomerConditionEvent>());
        var pensionerRate   = new PredicateCondition<Customer>("IsPensioner", c => c.CustomerType.ToString() == "Pensioner", "Customer @{CustomerName} is not a pensioner");
        var subscriberRate  = new PredicateCondition<Customer>("IsSubscriber", c => c.CustomerType.ToString() == "Subscriber", "Customer @{CustomerName} is not a paid subscriber");

        var studentSet      = new ConditionSet<decimal>("StudentRate", 0.10M, studentRate);
        var pensionerSet    = new ConditionSet<decimal>("PensionerRate", 0.15M, pensionerRate);
        var subscriberSet   = new ConditionSet<decimal>("SubscriberRate", 0.20M, subscriberRate);

        var discountRule = new Rule<Decimal>("DiscountRule", 0.00M, studentSet,EventDetails.Create<DecimalRuleEvent>(EventWhenType.OnSuccess))
                                                        .OrConditionSet(pensionerSet)
                                                            .OrConditionSet(subscriberSet);

        /*
            * Save this to disc instead of handcrafting and just modify when needed.
            * To test just read back from disc and pass to the condition engine
            * The DiscountRule.json file in the JsonRules folder was created using the Rule<T>.ToJsonString() which uses System.Text.Json. 
            * The default will escape characters such as < and > as can be seen in the file, if necessary you can set useEscaped = false in the ToJsonString method to have unescaped characters)
            * Properties with null values are not written out
        */
        var jsonString = discountRule.ToJsonString(writeIndented: true, useEscaped: true);

        WriteLine(jsonString);


        /*
            * Now lets look at how the boolean conditions are written.
            * Despite using classes such as PredicateCondition, the underlying code just uses the base classes
            * These class are really instead of adding lots of overloads on the condition class, that is why you will not see names such
            * as PredicateCondition, RegexCondition, CustomCondition in the json.
        */

        var nestedConditions = new PredicateCondition<Customer>("ConditionOne", c => c.CustomerID == 101, "Failed customer number condition")
                            .AndAlso(new PredicateCondition<CustomerAccount>("ConditionTwo", a => a.AccountNo > 5555, "Failed account number conditions"))
                    .OrElse(new PredicateCondition<Address>("ConditionThree", a => a.Country == "United Kingdom", "Failed the country condition") // < < < We did the And on the condition within the Or
                             .AndAlso(new PredicateCondition<OrderHistoryView>("ConditionFour", o => o.TotalOrders >= 5, "Failed the total order condition")));

        var nestedRule = new Rule<None>("NestedConditionRule",None.Value,new ConditionSet<None>("NestedConditionSet",None.Value,nestedConditions));


        jsonString = nestedRule.ToJsonString(writeIndented: true, useEscaped: true);

        WriteLine(jsonString);

    }

}
