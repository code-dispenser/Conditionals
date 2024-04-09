using Conditionals.Core.Areas.Conditions;
using Conditionals.Core.Areas.Events;
using Conditionals.Core.Common.Seeds;
using Conditionals.Core.Tests.SharedDataAndFixtures.Events;
using Conditionals.Core.Tests.SharedDataAndFixtures.Models;
using FluentAssertions;
using System.Text.RegularExpressions;
using Xunit;

namespace Conditionals.Core.Tests.Unit.Areas.Conditions;

public class RegexConditionTests
{

    [Fact]
    public void All_properties_of_the_regex_condition_should_be_set_when_using_the_full_constructor()
    {
        var condition = new RegexCondition<Customer>("ConditionOne", c => c.Address!.Town, "^[A-Z][a-z]{2,49}$", 
                                                        "AddressLine must start with a capital letter followed by 2 or more lower case letters.Max length 50 characters.",
                                                        RegexOptions.IgnoreCase | RegexOptions.Multiline, EventDetails.Create<ConditionEventCustomer>());

        condition.Should().Match<RegexCondition<Customer>>(r => r.ConditionName == "ConditionOne" && r.ContextType == typeof(Customer) && r.EvaluatorTypeName == GlobalStrings.Regex_Condition_Evaluator
                                                       && r.EventDetails!.EventTypeName == typeof(ConditionEventCustomer).AssemblyQualifiedName && r.EventDetails.EventWhenType == EventWhenType.OnSuccessOrFailure
                                                       && r.FailureMessage ==   "AddressLine must start with a capital letter followed by 2 or more lower case letters.Max length 50 characters."
                                                       && r.ConditionType == ConditionType.CustomExpression && r.ExpressionToEvaluate == "Address.Town [IsMatch] ^[A-Z][a-z]{2,49}$"
                                                       && r.AdditionalInfo["RegexOptions"] == "IgnoreCase | Multiline");

    }

    [Fact]
    public void Properties_and_defaults_should_be_set_via_the_simpler_constructor_overload()
    {
        var condition = new RegexCondition<Customer>("ConditionOne", c => c.MemberYears, "^[1-5][0-9]?$",
                                                        "Members yeas can only be from 1 to 59 years.");

        condition.Should().Match<RegexCondition<Customer>>(r => r.ConditionName == "ConditionOne" && r.ContextType == typeof(Customer) && r.EvaluatorTypeName == GlobalStrings.Regex_Condition_Evaluator
                                                       && r.EventDetails == null  && r.AdditionalInfo.Count == 0 && r.FailureMessage ==  "Members yeas can only be from 1 to 59 years."
                                                       && r.ConditionType == ConditionType.CustomExpression && r.ExpressionToEvaluate == "MemberYears [IsMatch] ^[1-5][0-9]?$");
                                                     

    }
}
