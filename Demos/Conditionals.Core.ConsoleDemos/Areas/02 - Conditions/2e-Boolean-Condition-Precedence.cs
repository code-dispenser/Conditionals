using Conditionals.Core.Areas.Conditions;
using Conditionals.Core.Common.FormatConverters;
using Conditionals.Core.Common.Seeds;
using Conditionals.Core.ConsoleDemos.Common.Models;

using static Conditionals.Core.ConsoleDemos.Common.Utilities.GeneralUtils;

namespace Conditionals.Core.ConsoleDemos.Areas.Conditions;

public class BooleanConditionPrecedence
{
    public void RunConditionPrecedence()
    {
        var precedencePrinter = new ConditionPrecedencePrinter(PrecedencePrintType.ConditionNameOnly);

        /*
            * Be careful with closing parenthesis as they can change the precedence  
        */

        var exampleOne = new PredicateCondition<Customer>("ConditionOne", c => c.CustomerID == 101, "Failed customer number condition")
                                .AndAlso(new PredicateCondition<CustomerAccount>("ConditionTwo", a => a.AccountNo > 5555, "Failed account number conditions"))
                                .OrElse(new PredicateCondition<Address>("ConditionThree",a => a.Country == "United Kingdom", "Failed the country condition"));// < < < We did the Or after the And

        WriteLine("exampleOne:");
        WriteLine(precedencePrinter.PrintPrecedenceOrder(exampleOne));
        WriteLine();

        var exampleTwo = new PredicateCondition<Customer>("ConditionOne", c => c.CustomerID == 101, "Failed customer number condition")
                         .AndAlso(new PredicateCondition<CustomerAccount>("ConditionTwo", a => a.AccountNo > 5555, "Failed account number conditions") //< < < We did the Or on the second condition
                         .OrElse(new PredicateCondition<Address>("ConditionThree", a => a.Country == "United Kingdom", "Failed the country condition")));

        WriteLine("exampleTwo:");
        WriteLine(precedencePrinter.PrintPrecedenceOrder(exampleTwo));
        WriteLine();

        var exampleThree = new PredicateCondition<Customer>("ConditionOne", c => c.CustomerID == 101, "Failed customer number condition")
                                    .AndAlso(new PredicateCondition<CustomerAccount>("ConditionTwo", a => a.AccountNo > 5555, "Failed account number conditions"))
                            .OrElse(new PredicateCondition<Address>("ConditionThree", a => a.Country == "United Kingdom", "Failed the country condition") // < < < We did the And on the condition within the Or
                                     .AndAlso(new PredicateCondition<OrderHistoryView>("ConditionFour", o => o.TotalOrders >= 5,"Failed the total order condition")));

        WriteLine("exampleThree:");
        WriteLine(precedencePrinter.PrintPrecedenceOrder(exampleThree));
        WriteLine();

        /*
            * AndAlso and OrElse are also classes that inherit BooleanConditionBase and can also be be used
        */

        var conditionOne    = new PredicateCondition<Customer>("ConditionOne", c => c.CustomerID == 101, "Failed customer number condition");
        var conditionTwo    = new PredicateCondition<CustomerAccount>("ConditionTwo", a => a.AccountNo > 5555, "Failed account number conditions");
        var conditionThree  = new PredicateCondition<Address>("ConditionThree", a => a.Country == "United Kingdom", "Failed the country condition");
        var conditionFour   = new PredicateCondition<OrderHistoryView>("ConditionFour", o => o.TotalOrders >= 5, "Failed the total order condition");
        
        var exampleFour = new OrElseConditions(conditionOne,conditionTwo).AndAlso(new OrElseConditions(conditionThree,conditionFour));

        WriteLine("exampleFour:");
        WriteLine(precedencePrinter.PrintPrecedenceOrder(exampleFour));
        WriteLine();

        /*
            * There are any number of combinations and nesting's that can be produced. 
            * 
            * The ConditionPrecedencePrinter can also print the expression instead of just the condition name if desired. 
        */

        precedencePrinter = new ConditionPrecedencePrinter(PrecedencePrintType.ExpressionToEvaluateOnly);

        WriteLine("exampleFour showing expressions:");
        WriteLine(precedencePrinter.PrintPrecedenceOrder(exampleFour));
        WriteLine();
    }
}
