using Conditionals.Core.Areas.Conditions;
using Conditionals.Core.Areas.Events;
using Conditionals.Core.Common.Seeds;
using Conditionals.Core.Tests.SharedDataAndFixtures.Events;
using Conditionals.Core.Tests.SharedDataAndFixtures.Models;
using FluentAssertions;
using Xunit;

namespace Conditionals.Core.Tests.Unit.Areas.Conditions;

public class CustomConditionTests
{
    //[Fact]
    //public void All_properties_of_a_custom_lambda_condition_should_be_set_via_the_constructor()
    //{
    //    Dictionary<string, string> additionalInfo = new() { ["Key"]="Value" };

    //    var theCondition = new CustomLambdaCondition<Person>("Under18", p => p.Age, "The person should be under 18", "CustomEvaluator", additionalInfo, EventDetails.Create<ConditionEventPerson>());

    //    theCondition.Should().Match<Condition<Person>>(c => c.ConditionName == "Under18" && c.ConditionType == ConditionType.LambdaPredicate && c.ContextType == typeof(Person)
    //                                                && c.EventDetails  != null && c.AdditionalInfo.Count == 1 && c.EvaluatorTypeName == "CustomEvaluator" && c.FailureMessage == "The person should be under 18"
    //                                                && c.CompiledPredicate != null && c.ExpressionToEvaluate == "p => (p.Age > 18)");

    //}

    [Fact]
    public void All_properties_of_a_custom_condition_should_be_set_via_the_constructor()
    {
        Dictionary<string, string> additionalInfo = new() { ["Key"]="Value" };

        var theCondition = new CustomCondition<Person>("Under18", "p => p.Age > 18", "The person should be under 18", "CustomEvaluator", additionalInfo, EventDetails.Create<ConditionEventPerson>());

        theCondition.Should().Match<Condition<Person>>(c => c.ConditionName == "Under18" && c.ConditionType == ConditionType.CustomExpression && c.ContextType == typeof(Person)
                                                    && c.EventDetails  != null && c.AdditionalInfo.Count == 1 && c.EvaluatorTypeName == "CustomEvaluator" && c.FailureMessage == "The person should be under 18"
                                                    && c.CompiledPredicate == null && c.ExpressionToEvaluate == "p => p.Age > 18");

    }

}
