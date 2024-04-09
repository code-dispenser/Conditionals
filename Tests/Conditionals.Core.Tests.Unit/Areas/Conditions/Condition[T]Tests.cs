using Conditionals.Core.Areas.Conditions;
using Conditionals.Core.Areas.Evaluators;
using Conditionals.Core.Areas.Events;
using Conditionals.Core.Common.Builders;
using Conditionals.Core.Common.Exceptions;
using Conditionals.Core.Common.FormatConverters;
using Conditionals.Core.Common.Models;
using Conditionals.Core.Common.Seeds;
using Conditionals.Core.Tests.SharedDataAndFixtures.Data;
using Conditionals.Core.Tests.SharedDataAndFixtures.Events;
using Conditionals.Core.Tests.SharedDataAndFixtures.Models;
using FluentAssertions;
using Xunit;

namespace Conditionals.Core.Tests.Unit.Areas.Conditions;

public class ConditionTests
{
    [Fact]
    public async Task The_sealed_evaluate_method_return_a_failed_condition_result_with_an_exception_if_the_data_context_is_missing()
    {
        var condition = new Condition<Person>("PersonAge", "p => p.Age >= 18", ConditionType.LambdaPredicate, "The person should be over 18", GlobalStrings.Predicate_Condition_Evaluator);

        ConditionData data = new([new(StaticData.CustomerOne())]);//Wrong data expects Person not Customer

        var result = await condition.Evaluate(ConditionTests.GetEvaluator, data);

        result.Should().Match<ConditionResult>(r => r.IsSuccess == false && r.Exceptions[0].GetType() == typeof(MissingConditionDataException));
    }

    [Fact]
    public async Task The_sealed_evaluate_method_should_throw_an_exception_if_the_condition_data_object_is_null()
    {
        var condition = new Condition<Person>("PersonAge", "p => p.Age >= 18", ConditionType.LambdaPredicate, "The person should be over 18", GlobalStrings.Predicate_Condition_Evaluator);

        ConditionData data = null!;

        await FluentActions.Invoking(() => condition.Evaluate(ConditionTests.GetEvaluator, data)).Should().ThrowAsync<MissingAllConditionDataException>();
    }

    [Fact]
    public async Task The_sealed_evaluate_method_should_throw_a_missing_evaluator_resolver_exception_if_the_resolver_is_null()
    {
        var condition = new Condition<Person>("PersonAge", "p => p.Age >= 18", ConditionType.LambdaPredicate, "The person should be over 18", GlobalStrings.Predicate_Condition_Evaluator);

        ConditionData data = new([new(StaticData.PersonUnder18())]);

       await FluentActions.Invoking(() => condition.Evaluate(null!, data)).Should().ThrowExactlyAsync<MissingEvaluatorResolverException>();
    }
    [Fact]
    public async Task The_sealed_evaluate_method_should_add_a_missing_evaluator_exception_to_the_condition_result_if_the_evaluator_was_null()
    {
        var condition = new Condition<Person>("PersonAge", "p => p.Age < 18", ConditionType.LambdaPredicate, "The person should be under 18", "Non-existent evaluator name");

        ConditionData data = new([new(StaticData.PersonUnder18())]);

        var conditionResult = await condition.Evaluate(ConditionTests.GetEvaluator, data);

        conditionResult.Should().Match<ConditionResult>(c => c.IsSuccess == false && c.Exceptions[0].GetType() == typeof(MissingEvaluatorException));
       
    }
    [Theory]
    [InlineData(null!)]
    [InlineData("")]
    [InlineData(" ")]
    public void Should_throw_an_exception_if_the_condition_name_is_null_empty_or_whitespace(string? conditionName)

        => FluentActions.Invoking(() => new Condition<Person>(conditionName!, "p => p.Age >= 18", ConditionType.LambdaPredicate, "The person should be over 18", GlobalStrings.Predicate_Condition_Evaluator))
                .Should().ThrowExactly<ArgumentException>();

    [Theory]
    [InlineData(null!)]
    [InlineData("")]
    [InlineData(" ")]
    public void Should_throw_an_exception_if_the_failure_message_is_null_empty_or_whitespace(string? failureMessage)

        => FluentActions.Invoking(() => new Condition<Person>("TestCondition", "p => p.Age >= 18", ConditionType.LambdaPredicate, failureMessage!, GlobalStrings.Predicate_Condition_Evaluator))
                .Should().ThrowExactly<ArgumentException>();

    [Theory]
    [InlineData(null!)]
    [InlineData("")]
    [InlineData(" ")]
    public void Should_throw_an_exception_if_the_evaluator_type_name_is_null_empty_or_whitespace(string? evaluatorTypeName)

        => FluentActions.Invoking(() => new Condition<Person>("TestCondition", "p => p.Age >= 18", ConditionType.LambdaPredicate, "The person should be over 18", evaluatorTypeName!))
                .Should().ThrowExactly<ArgumentException>();

    [Theory]
    [InlineData(null!)]
    [InlineData("")]
    [InlineData(" ")]
    public void Should_throw_an_exception_if_the_expression_to_evaluate_is_null_empty_or_whitespace(string? expressionToEvaluate)

        => FluentActions.Invoking(() => new Condition<Person>("TestCondition", expressionToEvaluate!, ConditionType.LambdaPredicate, "The person should be over 18", GlobalStrings.Predicate_Condition_Evaluator))
                .Should().ThrowExactly<ArgumentException>();

    [Fact]
    public void Should_trim_condition_name()

        => new Condition<Person>("  PersonAge  ", "p => p.Age >= 18", ConditionType.LambdaPredicate, "The person should be over 18", GlobalStrings.Predicate_Condition_Evaluator)
                    .ConditionName.Should().Be("PersonAge");

    [Fact]
    public void Should_trim_evaluator_type_name()

        => new Condition<Person>("  PersonAge  ", "p => p.Age >= 18", ConditionType.LambdaPredicate, "The person should be over 18", "  PredicateConditionEvaluator  ")
            .EvaluatorTypeName.Should().Be("PredicateConditionEvaluator");

    [Fact]
    public void All_properties_should_be_set_via_the_constructor()
    {
        Dictionary<string, string> additionalInfo = new(){ ["Key"]="Value"};

        var theCondition = new Condition<Person>("Under18", "p => p.Age > 18", ConditionType.LambdaPredicate, "The person should be under 18", GlobalStrings.Predicate_Condition_Evaluator,additionalInfo, EventDetails.Create<ConditionEventPerson>());


        theCondition.Should().Match<Condition<Person>>(c => c.ConditionName == "Under18" && c.ConditionType == ConditionType.LambdaPredicate && c.ContextType == typeof(Person)
                                                    && c.EventDetails  != null && c.AdditionalInfo.Count == 1 && c.EvaluatorTypeName == GlobalStrings.Predicate_Condition_Evaluator && c.FailureMessage == "The person should be under 18"
                                                    && c.CompiledPredicate != null && c.ExpressionToEvaluate == "p => p.Age > 18");

    }

    [Fact]
    public void A_null_additional_info_dictionary_by_default_should_get_converted_to_an_empty_dictionary()
    {
        var theCondition = new Condition<Person>("Under18", "p => p.Age > 18", ConditionType.LambdaPredicate, "The person should be under 18", GlobalStrings.Predicate_Condition_Evaluator, null!, EventDetails.Create<ConditionEventPerson>());

        theCondition.AdditionalInfo.Should().NotBeNull().And.BeEmpty();
    }

    [Fact]
    public async Task The_sealed_evaluate_method_should_evaluate_all_conditions_according_to_boolean_short_circuit_rules()
    {
        var testConditions = new PredicateCondition<Person>("Under18Condition", p => p.Age < 18, "Person should be under 18")
                                .AndAlso(new PredicateCondition<Customer>("CustomerNoCondition", c => c.CustomerNo == 111, "Customer number should be 111"))
                                .AndAlso(new OrElseConditions(new PredicateCondition<Supplier>("SupplierNameCondition", s => s.SupplierName == "WrongName", "Supplier name should be WrongName"),
                                         new PredicateCondition<Supplier>("SupplierTwoNameCondition", s => s.SupplierName == "SupplierTwo", "Supplier name should be SupplierTwo")));

        var precedencePath = new ConditionPrecedencePrinter().PrintPrecedenceOrder(testConditions);
        //"((Under18Condition AndAlso CustomerNoCondition) AndAlso (SupplierNameCondition OrElse SupplierTwoNameCondition))"
        // data should produce (True && True) && (False || True) 4 evaluations with one false.

        var data = ConditionDataBuilder.AddForAny(StaticData.PersonUnder18())
                                       .AndForCondition("CustomerNoCondition",StaticData.CustomerOne())
                                       .AndForAny(StaticData.SupplierOne())
                                       .AndForCondition("SupplierTwoNameCondition", StaticData.SupplierTwo())
                                       .Create();

        var conditionResult = await testConditions.Evaluate(ConditionTests.GetEvaluator, data);

        //the result chain is from the last evaluation to the first so the order should be:
        //SupplierTwoNameCondition (pass) - SupplierNameCondition (fail) - CustomerNoCondition (pass) - Under18Condition (pass)

        conditionResult.Should().Match<ConditionResult>(r => r.IsSuccess == true && r.ConditionName == "SupplierTwoNameCondition"
                                                    && r.ResultChain!.IsSuccess == false & r.ResultChain.ConditionName == "SupplierNameCondition"
                                                    && r.ResultChain.ResultChain!.IsSuccess == true & r.ResultChain.ResultChain.ConditionName == "CustomerNoCondition"
                                                    && r.ResultChain.ResultChain.ResultChain!.IsSuccess == true & r.ResultChain.ResultChain.ResultChain.ConditionName == "Under18Condition"
                                                    && r.ResultChain.ResultChain.ResultChain.ResultChain == null);
        

    }


    private static IConditionEvaluator GetEvaluator(string name, Type type)

      => name switch
        {
            GlobalStrings.Predicate_Condition_Evaluator => (IConditionEvaluator)Activator.CreateInstance(typeof(PredicateConditionEvaluator<>).MakeGenericType(type))!,
            _ => null!
        };



}
