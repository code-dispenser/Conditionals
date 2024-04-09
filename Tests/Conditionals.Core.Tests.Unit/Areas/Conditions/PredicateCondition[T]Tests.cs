using Conditionals.Core.Areas.Conditions;
using Conditionals.Core.Areas.Events;
using Conditionals.Core.Common.Seeds;
using Conditionals.Core.Tests.SharedDataAndFixtures.Events;
using Conditionals.Core.Tests.SharedDataAndFixtures.Models;
using FluentAssertions;
using Xunit;

namespace Conditionals.Core.Tests.Unit.Areas.Conditions;

public class PredicateConditionTests
{
    [Fact]
    public void All_properties_of_the_predicate_condition_should_be_set_via_the_constructor()
    {
        var theCondition = new PredicateCondition<Person>("Under18", p => p.Age > 18, "The person should be under 18", EventDetails.Create<ConditionEventPerson>());

        theCondition.Should().Match<PredicateCondition<Person>>(c => c.ConditionName == "Under18" && c.ConditionType == ConditionType.LambdaPredicate && c.ContextType == typeof(Person)
                                                    && c.EventDetails  != null && c.AdditionalInfo.Count == 0 && c.EvaluatorTypeName == GlobalStrings.Predicate_Condition_Evaluator && c.FailureMessage == "The person should be under 18"
                                                    && c.CompiledPredicate != null && c.ExpressionToEvaluate == "p => (p.Age > 18)");

    }

    [Fact]
    public void All_properties_of_a_custom_predicate_condition_should_be_set_via_the_constructor()
    {
        Dictionary<string, string> additionalInfo = new() { ["Key"]="Value" };

        var theCondition = new CustomPredicateCondition<Person>("Under18", p => p.Age > 18, "The person should be under 18","CustomPredicateEvaluator",additionalInfo, EventDetails.Create<ConditionEventPerson>());

        theCondition.Should().Match<CustomPredicateCondition<Person>>(c => c.ConditionName == "Under18" && c.ConditionType == ConditionType.LambdaPredicate && c.ContextType == typeof(Person)
                                                    && c.EventDetails  != null && c.AdditionalInfo.Count == 1 && c.EvaluatorTypeName == "CustomPredicateEvaluator" && c.FailureMessage == "The person should be under 18"
                                                    && c.CompiledPredicate != null && c.ExpressionToEvaluate == "p => (p.Age > 18)");

    }
}
