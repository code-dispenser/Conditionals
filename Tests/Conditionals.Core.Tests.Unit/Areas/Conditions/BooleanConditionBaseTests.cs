using Conditionals.Core.Areas.Conditions;
using Conditionals.Core.Areas.Events;
using Conditionals.Core.Common.Exceptions;
using Conditionals.Core.Common.Seeds;
using Conditionals.Core.Tests.SharedDataAndFixtures.Conditions;
using Conditionals.Core.Tests.SharedDataAndFixtures.Events;
using Conditionals.Core.Tests.SharedDataAndFixtures.Models;
using FluentAssertions;
using FluentAssertions.Execution;
using Xunit;

namespace Conditionals.Core.Tests.Unit.Areas.Conditions;

public class BooleanConditionBaseTests
{
    [Fact]
    public void The_deep_clone_condition_should_create_a_deep_clone_of_all_boolean_conditions()
    {
        Dictionary<string, string> additionalInfoOne = new() { ["Key"] = "Value" };
        Dictionary<string, string> additionalInfoTwo = new() { ["Another Key"] = "Another Value" };

        var conditionToClone = new PredicateCondition<Person>("PredicateConditionPerson", p => p.FirstName == "John", "First name should be John", EventDetails.Create<ConditionEventPerson>(EventWhenType.OnSuccessOrFailure))
                                    .AndAlso(new CustomPredicateCondition<Customer>("CustomPredicateName", c => c.CustomerName == "CustomerOne", "Customer name should be CustomerOne", "CustomPredicateEvaluator", additionalInfoOne))
                                    .OrElse(new AndAlsoConditions(new CustomCondition<Person>("CustomConditionPerson", "person expressions", "Should be a person", "CustomConditionEvaluator"),
                                                                  new CustomCondition<Customer>("CustomConditionCustomer", "Customer expression", "Should be a customer", "CustomConditionEvaluator", additionalInfoTwo)));



        var deepClone = BooleanConditionBase.DeepCloneCondition(conditionToClone);

        using(new AssertionScope())
        {
            conditionToClone.Should().BeOfType<OrElseConditions>();
            

            deepClone.Should().Match<OrElseConditions>(o => o.Left.GetType() == typeof(AndAlsoConditions) && o.Right.GetType() == typeof(AndAlsoConditions));


            ((AndAlsoConditions)((OrElseConditions)deepClone).Left).Left.Should().Match<ICondition>(c => c.ConditionName == "PredicateConditionPerson" && c.ContextType == typeof(Person) && c.FailureMessage == "First name should be John"
                                                                                                 && c.ConditionType == ConditionType.LambdaPredicate && c.ExpressionToEvaluate == "p => (p.FirstName == \"John\")"
                                                                                                 && c.EvaluatorTypeName == GlobalStrings.Predicate_Condition_Evaluator
                                                                                                 && c.EventDetails!.EventWhenType == EventWhenType.OnSuccessOrFailure && c.EventDetails.EventTypeName == typeof(ConditionEventPerson).AssemblyQualifiedName);

            ((AndAlsoConditions)((OrElseConditions)deepClone).Left).Right.Should().Match<ICondition>(c => c.ConditionName == "CustomPredicateName" && c.ContextType == typeof(Customer) && c.FailureMessage == "Customer name should be CustomerOne"
                                                                                     && c.ConditionType == ConditionType.LambdaPredicate && c.ExpressionToEvaluate == "c => (c.CustomerName == \"CustomerOne\")" && c.EvaluatorTypeName == "CustomPredicateEvaluator"
                                                                                     && c.EventDetails == null && c.AdditionalInfo["Key"] == "Value");


            ((AndAlsoConditions)((OrElseConditions)deepClone).Right).Left.Should().Match<ICondition>(c => c.ConditionName == "CustomConditionPerson" && c.ContextType == typeof(Person) && c.FailureMessage == "Should be a person"
                                                                                   && c.ConditionType == ConditionType.CustomExpression && c.ExpressionToEvaluate == "person expressions" && c.EvaluatorTypeName == "CustomConditionEvaluator"
                                                                                   && c.EventDetails == null && c.AdditionalInfo.Count==0);

            ((AndAlsoConditions)((OrElseConditions)deepClone).Right).Right.Should().Match<ICondition>(c => c.ConditionName == "CustomConditionCustomer" && c.ContextType == typeof(Customer) && c.FailureMessage == "Should be a customer"
                                                                       && c.ConditionType == ConditionType.CustomExpression && c.ExpressionToEvaluate == "Customer expression" && c.EvaluatorTypeName == "CustomConditionEvaluator"
                                                                       && c.EventDetails == null && c.AdditionalInfo["Another Key"] == "Another Value");

            conditionToClone = null;
            conditionToClone = new AndAlsoConditions(null!,null!);

            conditionToClone.Should().Match<AndAlsoConditions>(a => a.Left == null && a.Right == null);

            deepClone.Should().Match<OrElseConditions>(a => a.Left != null && a.Right != null);

        }

    }

    [Fact]
    public void The_deep_clone_condition_should_throw_an_exception_if_it_encounters_invalid_conditions()
    {
        Dictionary<string, string> additionalInfoOne = new() { ["Key"] = "Value" };
        Dictionary<string, string> additionalInfoTwo = new() { ["Another Key"] = "Another Value" };

        var conditionToClone = new PredicateCondition<Person>("PredicateConditionPerson", p => p.FirstName == "John", "First name should be John", EventDetails.Create<ConditionEventPerson>(EventWhenType.OnSuccessOrFailure))
                                    .AndAlso(null!)
                                    .OrElse(new AndAlsoConditions(new CustomCondition<Person>("CustomConditionPerson", "person expressions", "Should be a person", "CustomConditionEvaluator"),
                                                                  null!));

        FluentActions.Invoking(() => BooleanConditionBase.DeepCloneCondition(conditionToClone)).Should().ThrowExactly<InvalidBooleanConditionTypeException>();

    }

    [Fact]
    public void The_deep_clone_condition_should_throw_an_exception_if_it_encounters_an_invalid_condition_in_the_private_create_condition_method()
    {
        Dictionary<string, string> additionalInfoOne = new() { ["Key"] = "Value" };
        Dictionary<string, string> additionalInfoTwo = new() { ["Another Key"] = "Another Value" };

        var conditionToClone = new PredicateCondition<Person>("PredicateConditionPerson", p => p.FirstName == "John", "First name should be John", EventDetails.Create<ConditionEventPerson>(EventWhenType.OnSuccessOrFailure))
                                    .AndAlso(new BadCondition());

        FluentActions.Invoking(() => BooleanConditionBase.DeepCloneCondition(conditionToClone)).Should().ThrowExactly<InvalidBooleanConditionTypeException>();

    }
}
