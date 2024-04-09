using Conditionals.Core.Areas.Conditions;
using Conditionals.Core.Areas.Events;
using Conditionals.Core.Areas.Rules;
using Conditionals.Core.Common.Exceptions;
using Conditionals.Core.Common.FormatConverters;
using Conditionals.Core.Common.Models;
using Conditionals.Core.Common.Seeds;
using Conditionals.Core.Tests.SharedDataAndFixtures.Data;
using Conditionals.Core.Tests.SharedDataAndFixtures.Events;
using Conditionals.Core.Tests.SharedDataAndFixtures.Models;
using FluentAssertions;
using FluentAssertions.Execution;
using Xunit;

namespace Conditionals.Core.Tests.Unit.Common.FormatConverters;

public class JsonRuleConverterTests
{
    [Fact]
    public void The_create_json_condition_from_boolean_condition_should_create_the_correct_ast_tree_condition_structure_part_of_the_json_rule_model()
    {
        var testBooleanCondition = new PredicateCondition<Person>("PersonName", p => p.FirstName == "John", "Name should be John")
                                        .AndAlso(new PredicateCondition<Customer>("CustomerNo", c => c.CustomerNo == 123, "Customer number Should be 123"))
                                        .OrElse(new AndAlsoConditions(new PredicateCondition<Person>("PersonAge", p => p.Age > 21, "Person should be over 21"),
                                                new PredicateCondition<Supplier>("SupplierNo", s => s.SupplierNo == 123, "Supplier number number Should be 123")));

        //the precedence order is: "((PersonName AndAlso CustomerNo) OrElse (PersonAge AndAlso SupplierNo))"

        var jsonCondition = JsonRuleConverter.ConvertToJsonCondition(testBooleanCondition);

        // should produce same nesting as the testBooleanCondition in an AST format, top level Left Operand -  OrElse - Right Operand with each operand having two conditions with an AndAlso like the precedence order above

        using (new AssertionScope())
        {
            jsonCondition.Operator.Should().Be(OperatorType.OrElse.ToString());
            jsonCondition.LeftOperand!.LeftOperand!.ConditionName.Should().Be("PersonName");
            jsonCondition.LeftOperand!.Operator.Should().Be(OperatorType.AndAlso.ToString());
            jsonCondition.LeftOperand!.RightOperand!.ConditionName.Should().Be("CustomerNo");
            jsonCondition.RightOperand!.LeftOperand!.ConditionName.Should().Be("PersonAge");
            jsonCondition.RightOperand!.Operator.Should().Be(OperatorType.AndAlso.ToString());
            jsonCondition.RightOperand!.RightOperand!.ConditionName.Should().Be("SupplierNo");
        }

    }
    [Fact]
    public void The_create_json_condition_from_boolean_condition_should_throw_an_argument_exception_if_it_cannot_create_cast_the_condition_to_an_condition()
    {
        var testBooleanCondition = new PredicateCondition<Person>("PersonName", p => p.FirstName == "John", "Name should be John")
                                        .AndAlso(new PredicateCondition<Customer>("CustomerNo", c => c.CustomerNo == 123, "Customer number Should be 123"))
                                        .OrElse(new AndAlsoConditions(null!, null!));

        FluentActions.Invoking(() => JsonRuleConverter.ConvertToJsonCondition(testBooleanCondition)).Should().ThrowExactly<InvalidBooleanConditionTypeException>();

    }

    [Fact]
    public void Any_unhandled_exceptions_converting_a_json_string_to_a_rule_should_be_caught_and_thrown_as_a_rule_from_json_exception()

        => FluentActions.Invoking(() => JsonRuleConverter.RuleFromJson<None>("Not a json rule")).Should().ThrowExactly<RuleFromJsonException>();

    [Fact]
    public void Json_without_the_condition_to_evaluate_expression_should_throw_a_missing_expression_to_evaluate_property_value_exception()

        => FluentActions.Invoking(() => JsonRuleConverter.RuleFromJson<None>(StaticData.JsonWithoutExpressionToEvaluate)).Should().ThrowExactly<MissingExpressionToEvaluateException>();

    [Fact]
    public void Json_without_a_condition_context_type_name_should_throw_a_context_type_assembly_not_found_exception()

        => FluentActions.Invoking(() => JsonRuleConverter.RuleFromJson<None>(StaticData.JsonWithoutAContextTypeName)).Should().ThrowExactly<ContextTypeAssemblyNotFoundException>();

    [Fact]
    public void Json_with_an_event_type_that_cannot_be_found_and_or_created_should_throw_an_event_not_found_eException()

        => FluentActions.Invoking(() => JsonRuleConverter.RuleFromJson<int>(StaticData.JsonWithABadRuleEvent)).Should().ThrowExactly<EventNotFoundException>();


    [Fact]
    public void TenantID_and_cultureID_should_be_set_to_defaults_if_null_converting_from_json_rule_to_rule()
    {
        JsonRule<None> jsonRule = new() { RuleName="RuleOne", TenantID = null!, CultureID = null!, FailureValue = None.Value, IsDisabled = false, ConditionSets = null!, ValueTypeName = { } };

        var ruleFromJsonRule = JsonRuleConverter.RuleFromJsonRule(jsonRule);

        ruleFromJsonRule.Should().Match<Rule<None>>(r => r.TenantID == GlobalStrings.Default_TenantID && r.CultureID ==  GlobalStrings.Default_CultureID);
    }

    [Fact]
    public void Converting_a_json_rule_to_a_rule_with_a_bad_event_type_should_throw_an_event_not_found_eException()
    {
        JsonRule<None> jsonRule = new()
        {
            RuleName="RuleOne", TenantID = null!, CultureID = null!, FailureValue = None.Value, IsDisabled = false, ValueTypeName = { },
            RuleEventDetails = new JsonEventDetails { EventTypeName = "BadEventTypeName", EventWhenType = "OnFailure" }
        };

        FluentActions.Invoking(() => JsonRuleConverter.RuleFromJsonRule(jsonRule)).Should().ThrowExactly<EventNotFoundException>();
    }
    [Fact]
    public void Converting_a_json_rule_to_a_rule_with_null_rule_details_should_create_the_rule()
    {
        JsonRule<None> jsonRule = new() 
        {
            RuleName="RuleOne", TenantID = null!, CultureID = null!, FailureValue = None.Value, IsDisabled = false, ValueTypeName = { },
            RuleEventDetails = null
        };

        FluentActions.Invoking(() => JsonRuleConverter.RuleFromJsonRule(jsonRule)).Should().NotThrow();
    }

    [Fact]
    public void The_convert_to_boolean_condition_should_create_the_correct_boolean_condition_structure()
    {
        var testBooleanCondition = new PredicateCondition<Person>("PersonName", p => p.FirstName == "John", "Name should be John")
                                        .AndAlso(new PredicateCondition<Customer>("CustomerNo", c => c.CustomerNo == 123, "Customer number Should be 123"))
                                        .OrElse(new AndAlsoConditions(new PredicateCondition<Person>("PersonAge", p => p.Age > 21, "Person should be over 21"),
                                                new PredicateCondition<Supplier>("SupplierNo", s => s.SupplierNo == 123, "Supplier number number Should be 123")));

        //the precedence order is: "((PersonName AndAlso CustomerNo) OrElse (PersonAge AndAlso SupplierNo))"

        var jsonCondition = JsonRuleConverter.ConvertToJsonCondition(testBooleanCondition);
        var convertedCondition = JsonRuleConverter.ConvertToBooleanCondition(jsonCondition);
        // should produce original nesting conditions
        /*
            * I do not recreate PredicateCondition, CustomCondition, RegexCondition etc, only the base Condition. The derived classes are instead of lots of overloads on just one condition class. 
        */
        using (new AssertionScope())
        {
            testBooleanCondition.Should().Match<OrElseConditions>(a => a.Left.GetType() == typeof(AndAlsoConditions) && a.Right.GetType() == typeof(AndAlsoConditions));
            convertedCondition.Should().Match<OrElseConditions>(a => a.Left.GetType() == typeof(AndAlsoConditions) && a.Right.GetType() == typeof(AndAlsoConditions));

            var leftAndAlso = ((OrElseConditions)testBooleanCondition).Left;
            var rightAndAlso = ((OrElseConditions)testBooleanCondition).Right;

            leftAndAlso.Should().Match<AndAlsoConditions>(l => l.Left.GetType() == typeof(PredicateCondition<Person>) && l.Right.GetType() == typeof(PredicateCondition<Customer>));
            rightAndAlso.Should().Match<AndAlsoConditions>(l => l.Left.GetType() == typeof(PredicateCondition<Person>) && l.Right.GetType() == typeof(PredicateCondition<Supplier>));

            var convertedLeftAndAlso = ((OrElseConditions)convertedCondition).Left;
            var convertedRightAndAlso = ((OrElseConditions)convertedCondition).Right;

            convertedLeftAndAlso.Should().Match<AndAlsoConditions>(l => l.Left.GetType() == typeof(Condition<Person>) && l.Right.GetType() == typeof(Condition<Customer>));
            convertedRightAndAlso.Should().Match<AndAlsoConditions>(l => l.Left.GetType() == typeof(Condition<Person>) && l.Right.GetType() == typeof(Condition<Supplier>));

            var lastLeftCondition = ((AndAlsoConditions)convertedRightAndAlso).Left;
            var lastRightCondition = ((AndAlsoConditions)convertedRightAndAlso).Right;

            lastLeftCondition.Should().Match<Condition<Person>>(p => p.ConditionName == "PersonAge" && p.FailureMessage == "Person should be over 21");
            lastRightCondition.Should().Match<Condition<Supplier>>(s => s.ConditionName == "SupplierNo" && s.ConditionType == ConditionType.LambdaPredicate);


        }

    }

    [Fact]
    public void The_converter_should_create_a_rule_from_a_json_rule_with_sets_and_conditions_and_events()
    {
        Dictionary<string, string> additionalInfo = new() { ["SomeKey"] = "Some Value" };
        var condition = new CustomPredicateCondition<Customer>("ConditionOne", c => c.CustomerName == "CustomerOne", "The customer should be named CustomerOne", "CustomPredicateEvaluator", additionalInfo, EventDetails.Create<ConditionEventCustomer>());
        var rule = new Rule<int>("RuleOne", 0, new ConditionSet<int>("SetOne", 42, condition), EventDetails.Create<RuleEventInt>(EventWhenType.OnSuccess));

        var jsonRule = JsonRuleConverter.JsonRuleFromRule(rule);
        var convertedToRule = JsonRuleConverter.RuleFromJsonRule(jsonRule);

        using (new AssertionScope())
        {
            convertedToRule.Should().Match<Rule<int>>(r => r.RuleName == "RuleOne" && r.IsDisabled == false && r.FailureValue == 0 && r.RuleEventDetails!.EventWhenType == EventWhenType.OnSuccess
                                                  && r.RuleEventDetails.EventTypeName == typeof(RuleEventInt).AssemblyQualifiedName && r.CultureID == GlobalStrings.Default_CultureID && r.TenantID == GlobalStrings.Default_TenantID
                                                  && r.ConditionSets[0].SetValue == 42 && r.ConditionSets[0].SetName == "SetOne");

            var convertedCondition = convertedToRule.ConditionSets[0].BooleanConditions;

            convertedCondition.Should().Match<Condition<Customer>>(c => c.ConditionName == "ConditionOne" && c.ContextType == typeof(Customer) && c.FailureMessage == "The customer should be named CustomerOne"
                                                                && c.ConditionType == ConditionType.LambdaPredicate && c.EvaluatorTypeName == "CustomPredicateEvaluator"
                                                                && c.ExpressionToEvaluate == "c => (c.CustomerName == \"CustomerOne\")" && c.AdditionalInfo["SomeKey"] == "Some Value"
                                                                && c.EventDetails!.EventWhenType == EventWhenType.OnSuccessOrFailure && c.EventDetails.EventTypeName == typeof(ConditionEventCustomer).AssemblyQualifiedName);
        }
    }

    [Fact]
    public void Converting_json_to_a_rule_without_a_condition_name_Should_throw_a_rule_from_json_exception()

        => FluentActions.Invoking(() => JsonRuleConverter.RuleFromJson<None>(StaticData.JsonWithoutConditionName)).Should()
                .Throw<RuleFromJsonException>().WithInnerException<ArgumentException>().WithMessage("Invalid JSON condition");

    [Fact]
    public void Converting_json_to_a_rule_without_a_failure_message_should_substitute_a_default_message()
        
        => JsonRuleConverter.RuleFromJson<None>(StaticData.JsonWithoutAFailureMessage)
            .ConditionSets[0].BooleanConditions.Should().Match<ICondition>(c => c.FailureMessage == GlobalStrings.Default_Condition_Failure_Message);

    [Fact]
    public void Converting_json_to_a_rule_without_an_evaluator_type_name_should_default_to_not_available()

        => JsonRuleConverter.RuleFromJson<None>(StaticData.JsonWithoutAnEvaluatorTypeName)
            .ConditionSets[0].BooleanConditions.Should().Match<ICondition>(c => c.EvaluatorTypeName == "N/A");

    [Fact]
    public void Converting_json_to_a_rule_with_an_incorrect_condition_event_type_name_should_throw_an_event_not_found_exception()

        => FluentActions.Invoking(() => JsonRuleConverter.RuleFromJson<None>(StaticData.JsonWithABadConditionEvent)).Should().ThrowExactly<EventNotFoundException>();

    [Fact]
    public void Converting_json_to_a_rule_with_a_short_condition_context_type_name_should_be_ok_if_there_are_no_clashes()
    
        => JsonRuleConverter.RuleFromJson<None>(StaticData.JsonWithAShortPersonContextTypeName)
                                .ConditionSets[0].BooleanConditions.Should().Match<Condition<Person>>(p => p.ContextType == typeof(Person));

    [Fact]
    public void Converting_json_to_a_rule_with_an_incorrect_condition_context_type_name_should_throw_an_e_context_type_assembly_not_found_exception()

     => FluentActions.Invoking(() => JsonRuleConverter.RuleFromJson<None>(StaticData.JsonWithABadContextTypeName)).Should().ThrowExactly<ContextTypeAssemblyNotFoundException>();


    [Fact]
    public void Converting_json_with_incorrect_event_when_types_should_default_to_the_event_when_type_never()
    {
        var jsonString = StaticData.JsonWithBadRuleEventWhenTypes;

        var rule = JsonRuleConverter.RuleFromJson<int>(jsonString);

        rule.Should().Match<Rule<int>>(r => r.RuleEventDetails!.EventWhenType == EventWhenType.Never 
                                    && ((ICondition)(r.ConditionSets[0].BooleanConditions)).EventDetails!.EventWhenType == EventWhenType.Never);
    }


}
