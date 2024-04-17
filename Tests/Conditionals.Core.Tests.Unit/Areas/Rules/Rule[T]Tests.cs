using Conditionals.Core.Areas.Conditions;
using Conditionals.Core.Areas.Evaluators;
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

namespace Conditionals.Core.Tests.Unit.Areas.Rules;

public class RuleTests
{
    [Fact]
    public void The_internal_to_json_rule_method_should_convert_the_rule_to_a_json_rule_object()
    {
        Dictionary<string, string> additionalInfo = new() { ["Key"] = "Value" };

        var condition = new PredicateCondition<Customer>("CustomerName", c => c.CustomerName == "CustomerOne", "CustomerName should be CustomerOne", EventDetails.Create<ConditionEventCustomer>(EventWhenType.OnSuccessOrFailure))
                        .AndAlso(new CustomCondition<Customer>("CustomerNo", "Some expression", "The customer number should be greater than 100", "CustomEvaluator", additionalInfo));
     
        var conditionSetOne = new ConditionSet<int>("SetOne",42,condition);

        var rule = new Rule<int>("RuleOne", 10, conditionSetOne, EventDetails.Create<RuleEventInt>(EventWhenType.OnFailure));

        var jsonRule = rule.ToJsonRule();

        //AssertionOptions.FormattingOptions.MaxDepth = 10;
        
        using (new AssertionScope())
        {
            jsonRule.Should().Match<JsonRule<int>>(j => j.RuleName == "RuleOne" && j.IsDisabled == false && j.FailureValue == 10 && j.CultureID == GlobalStrings.Default_CultureID
                                                && j.TenantID == GlobalStrings.Default_TenantID && j.ValueTypeName == typeof(int).Name && j.ConditionSets.Count == 1
                                                && j.RuleEventDetails!.EventWhenType == EventWhenType.OnFailure.ToString() && j.RuleEventDetails.EventTypeName == typeof(RuleEventInt).FullName);

            var jsonConditionSet = jsonRule.ConditionSets.First();

            jsonConditionSet.Should().Match<JsonConditionSet<int>>(c => c.SetName == "SetOne" && c.SetValue == 42 && c.BooleanConditions != null);

            var jsonConditions = jsonConditionSet.BooleanConditions;

            jsonConditions.Should().Match<JsonCondition>(c => c.ConditionName == null && c.ConditionEventDetails == null && c.ConditionType == null
                                                      && c.AdditionalInfo == null && c.ContextTypeName == null && c.EvaluatorTypeName == null 
                                                      && c.ExpressionToEvaluate == null && c.FailureMessage == null && c.AdditionalInfo == null
                                                      && c.LeftOperand != null & c.RightOperand != null && c.Operator == OperatorType.AndAlso.ToString());

            var leftCondition  = jsonConditions.LeftOperand;
            var rightCondition = jsonConditions.RightOperand;

            leftCondition.Should().Match<JsonCondition>(c => c.ConditionName == "CustomerName" && c.ConditionType == ConditionType.LambdaPredicate.ToString()
                                                     && c.Operator == null && c.ContextTypeName == typeof(Customer).FullName
                                                     && c.EvaluatorTypeName == GlobalStrings.Predicate_Condition_Evaluator && c.FailureMessage == "CustomerName should be CustomerOne"
                                                     && c.LeftOperand == null && c.RightOperand == null && c.ConditionEventDetails!.EventWhenType == EventWhenType.OnSuccessOrFailure.ToString()
                                                     && c.ConditionEventDetails.EventTypeName == typeof(ConditionEventCustomer).FullName
                                                     && c.ExpressionToEvaluate == "c => (c.CustomerName == \"CustomerOne\")");

            rightCondition.Should().Match<JsonCondition>(c => c.ConditionName == "CustomerNo" && c.ConditionType == ConditionType.CustomExpression.ToString()
                                                      && c.Operator == null && c.ContextTypeName == typeof(Customer).FullName && c.AdditionalInfo!["Key"] == "Value" 
                                                      && c.EvaluatorTypeName == "CustomEvaluator" && c.FailureMessage == "The customer number should be greater than 100"
                                                      && c.LeftOperand == null && c.RightOperand == null && c.ConditionEventDetails == null && c.ExpressionToEvaluate == "Some expression");

        }


    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("  ")]
    public void A_rule_should_use_the_default_tenant_id_if_tenant_id_is_null_empty_or_whitespace(string? tenantID)
    { 
        var conditionSet = new ConditionSet<None>("SetOne", None.Value, new PredicateCondition<Customer>("ConditionName", c => c.CustomerName == "CustomerOne", "Customer name should be CustomerOne"));
        using(new AssertionScope())
        { 
            new Rule<None>("RuleOne",None.Value,null,tenantID!, null!).TenantID.Should().Be(GlobalStrings.Default_TenantID);
            new Rule<None>("RuleOne", None.Value, conditionSet, null, tenantID!, null!).TenantID.Should().Be(GlobalStrings.Default_TenantID);
        }
     }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("  ")]
    public void A_rule_should_use_the_default_culture_id_if_culture_id_is_null_empty_or_whitespace(string? cultureID)
    {
        var conditionSet = new ConditionSet<None>("SetOne", None.Value, new PredicateCondition<Customer>("ConditionName", c => c.CustomerName == "CustomerOne", "Customer name should be CustomerOne"));
        using(new AssertionScope())
        {
            new Rule<None>("RuleOne", None.Value, null, cultureID!, null!).CultureID.Should().Be(GlobalStrings.Default_CultureID);
            new Rule<None>("RuleOne", None.Value, conditionSet, null, cultureID!, null!).CultureID.Should().Be(GlobalStrings.Default_CultureID);
        }
       
    }

    [Fact]
    public void Should_be_able_to_set_the_rule_disabled_flag_after_creation()
    {
        var rule = new Rule<None>("RuleOne", None.Value);
        rule.IsDisabled = true;
        rule.IsDisabled.Should().BeTrue();  
    }
    //[Fact]
    //public void The_rule_to_json_string_should_not_process_an_incomplete_rule_and_should_throw_an_exception_if_the_rule_is_in_complete()

    //    => FluentActions.Invoking(() => new Rule<None>("InCompleteRule", None.Value).ToJsonString()).Should().ThrowExactly<InCompleteRuleException>();

    [Fact]
    public void A_rule_should_be_able_to_be_converted_to_a_json_string()
    {
        Dictionary<string, string> additionalInfo = new() { ["Key"] = "Value" };

        var condition       = new CustomPredicateCondition<Customer>("CustomerName", c => c.CustomerName == "CustomerOne", "CustomerName should be CustomerOne","CustomPredicateEvaluator", additionalInfo, EventDetails.Create<ConditionEventCustomer>(EventWhenType.OnSuccessOrFailure));
        var conditionSetOne = new ConditionSet<int>("SetOne", 42, condition);
        var rule            = new Rule<int>("RuleOne", 10, conditionSetOne, EventDetails.Create<RuleEventInt>(EventWhenType.OnFailure));

        using(new AssertionScope())
        {
            var jsonString = rule.ToJsonString();

            var convertedRuleCheck = JsonRuleConverter.RuleFromJson<int>(jsonString);

            convertedRuleCheck.Should().Match<Rule<int>>(r => r.RuleName == "RuleOne" && r.ConditionSets[0].BooleanConditions != null);
        }

    }

    [Theory]
    [InlineData(true)]
    [InlineData(false)]
    public void The_rule_to_json_string_method_should_not_escape_characters_when_set_to_false(bool useEscaped)
    {
        Dictionary<string, string> additionalInfo = new() { ["Key"] = "Value" };

        var condition       = new PredicateCondition<Customer>("CustomerName", c => c.CustomerName == "CustomerOne", "CustomerName should be CustomerOne");
        var conditionSetOne = new ConditionSet<int>("SetOne", 42, condition);
        var theRule            = new Rule<int>("RuleOne", 10, conditionSetOne);

        var theJsonString = theRule.ToJsonString(false, useEscaped);

        if (true  == useEscaped) theJsonString.Should().NotContain("=>");
        if (false == useEscaped) theJsonString.Should().Contain("=>");

    }

    [Fact]
    public void You_should_be_able_to_make_a_deep_clone_of_a_rule()
    {
        Dictionary<string, string> additionalInfo = new() { ["Key"] = "Value" };

        var condition = new PredicateCondition<Customer>("CustomerName", c => c.CustomerName == "CustomerOne", "CustomerName should be CustomerOne", EventDetails.Create<ConditionEventCustomer>(EventWhenType.OnSuccessOrFailure))
                        .AndAlso(new CustomCondition<Customer>("CustomerNo", "Some expression", "The customer number should be greater than 100", "CustomEvaluator", additionalInfo)); 
        
        var conditionSetOne = new ConditionSet<int>("SetOne", 42, condition);
       
        var rule        = new Rule<int>("RuleOne", 10, conditionSetOne, EventDetails.Create<RuleEventInt>(EventWhenType.OnFailure));
        var clonedRule  = rule.DeepCloneRule();
        
        using (new AssertionScope())
        {
            clonedRule.Should().Match<Rule<int>>(r => r.RuleName == "RuleOne" && r.FailureValue == 10 && r.RuleEventDetails!.EventWhenType == EventWhenType.OnFailure
                                             && r.RuleEventDetails.EventTypeName == typeof(RuleEventInt).AssemblyQualifiedName && r.ConditionSets.Count == 1
                                             && r.ConditionSets[0].SetValue == 42 && r.ConditionSets[0].SetName == "SetOne");

            var andOrCondition = clonedRule.ConditionSets[0].BooleanConditions as AndAlsoConditions;

            var leftCondition  = andOrCondition!.Left  as Condition<Customer>;
            var rightCondition = andOrCondition!.Right as Condition<Customer>;

            leftCondition.Should().Match<Condition<Customer>>(c => c.ConditionName == "CustomerName" && c.FailureMessage == "CustomerName should be CustomerOne" && c.ContextType == typeof(Customer)
                                                          && c.ConditionType == ConditionType.LambdaPredicate && c.ExpressionToEvaluate == "c => (c.CustomerName == \"CustomerOne\")"
                                                          && c.EvaluatorTypeName == GlobalStrings.Predicate_Condition_Evaluator && c.EventDetails!.EventWhenType == EventWhenType.OnSuccessOrFailure
                                                          && c.EventDetails.EventTypeName == typeof(ConditionEventCustomer).AssemblyQualifiedName);

            rightCondition.Should().Match<Condition<Customer>>(c => c.ConditionName == "CustomerNo" && c.FailureMessage == "The customer number should be greater than 100" && c.ContextType == typeof(Customer)
                                              && c.ConditionType == ConditionType.CustomExpression && c.ExpressionToEvaluate == "Some expression" && c.EvaluatorTypeName == "CustomEvaluator"
                                              && c.EventDetails == null);
        }

    }

    [Fact]
    public async Task A_disabled_rule_should_not_evaluate_any_conditions_and_should_return_a_rule_result_with_the_disabled_flag_set_as_true()
    {
        var condition       = new PredicateCondition<Customer>("CustomerName", c => c.CustomerName == "CustomerOne", "CustomerName should be CustomerOne");
        var conditionSetOne = new ConditionSet<int>("SetOne", 42, condition);
        var rule            = new Rule<int>("RuleOne", 10, conditionSetOne, EventDetails.Create<RuleEventInt>(EventWhenType.OnFailure));

        rule.IsDisabled     = true;
        ConditionData data  = new([new(StaticData.CustomerOne())]);

        var ruleResult = await rule.Evaluate(RuleTests.GetEvaluator, data);

        ruleResult.Should().Match<RuleResult<int>>(r => r.IsDisabled == true && r.EvaluationCount == 0);
    }



    [Fact]
    public async Task Should_throw_a_missing_all_condition_data_exception_in_the_evaluate_method_if_the_condition_data_is_null()
    {
        var conditionOne    = new PredicateCondition<Customer>("CustomerName", c => c.CustomerName == "WrongName", "CustomerName should be WrongName");
        var conditionSetOne = new ConditionSet<int>("SetOne", 20, conditionOne);
        var rule            = new Rule<int>("RuleOne", 10, conditionSetOne);
        
        await FluentActions.Invoking(() => rule.Evaluate(RuleTests.GetEvaluator,null!)).Should().ThrowExactlyAsync<MissingAllConditionDataException>();
    }

    [Fact]
    public async Task Should_throw_a_missing_evaluator_resolver_exception_in_the_evaluate_method_if_the_evaluation_resolver_is_null()
    {
        var conditionOne     = new PredicateCondition<Customer>("CustomerName", c => c.CustomerName == "WrongName", "CustomerName should be WrongName");
        var conditionSetOne  = new ConditionSet<int>("SetOne", 20, conditionOne);
        var rule             = new Rule<int>("RuleOne", 10, conditionSetOne);

        ConditionData data = new([new(StaticData.CustomerOne())]);

        await FluentActions.Invoking(() => rule.Evaluate(null!, data)).Should().ThrowExactlyAsync<MissingEvaluatorResolverException>();
    }

    [Fact]
    public async Task A_rule_with_two_condition_sets_should_process_both_if_the_first_fails()
    {
        var conditionOne    = new PredicateCondition<Customer>("CustomerNameOne", c => c.CustomerName == "WrongName", "CustomerName should be WrongName");
        var conditionSetOne = new ConditionSet<int>("SetOne", 10, conditionOne);

        var conditionTwo    = new PredicateCondition<Customer>("CustomerNameTwo", c => c.CustomerName == "AnotherWrongName", "CustomerName should be AnotherWrongName");
        var conditionSetTwo = new ConditionSet<int>("SetTwo", 20, conditionTwo);

        var rule = new Rule<int>("RuleOne", 0, conditionSetOne).OrConditionSet(conditionSetTwo);

        ConditionData data = new([new(StaticData.CustomerOne())]);

        var ruleResult = await rule.Evaluate(RuleTests.GetEvaluator,data);

        ruleResult.Should().Match<RuleResult<int>>(r => r.IsSuccess == false && r.EvaluationCount == 2 && r.SetResultChain!.SetName == "SetTwo" && r.SetResultChain.PreviousSetResult!.SetName == "SetOne");

    }

    [Fact]
    public async Task All_failure_messages_from_all_failing_conditions_across_all_condition_sets_should_populate_the_rule_results_failure_messages_list()
    {
        var conditionOne = new PredicateCondition<Customer>("CustomerNameOne", c => c.CustomerName == "WrongName", "CustomerName should be WrongName")
                                .OrElse(new PredicateCondition<Customer>("CustomerNumber", c => c.CustomerNo == 123, "Customer number should be 111"));

        var conditionSetOne = new ConditionSet<int>("SetOne", 10, conditionOne);

        var conditionTwo = new PredicateCondition<Customer>("CustomerNameTwo", c => c.CustomerName == "AnotherWrongName", "CustomerName should be AnotherWrongName");
        var conditionSetTwo = new ConditionSet<int>("SetTwo", 20, conditionTwo);

        var conditionThree = new PredicateCondition<Supplier>("PurchasesFromSupplier", s => s.TotalPurchases > 5000, "Purchases made with the supplier need bo be over £5000t");
        var conditionSetThree = new ConditionSet<int>("SetThree", 30, conditionThree);

        var rule = new Rule<int>("RuleOne", 0, conditionSetOne).OrConditionSet(conditionSetTwo).OrConditionSet(conditionSetThree);

        ConditionData data = new([new(StaticData.CustomerOne()), new(StaticData.SupplierOne())]);

        var ruleResult = await rule.Evaluate(RuleTests.GetEvaluator, data);

        ruleResult.Should().Match<RuleResult<int>>(r => r.IsSuccess == false && r.EvaluationCount == 4 && r.SetResultChain!.SetName == "SetThree" && r.SetResultChain.PreviousSetResult!.SetName == "SetTwo"
                                                && r.SetResultChain!.PreviousSetResult!.PreviousSetResult!.SetName == "SetOne" && r.FailureMessages.Count == 4
                                                && r.FailureMessages[1] == "Customer number should be 111");

    }

    private static IConditionEvaluator GetEvaluator(string name, Type type)

      => name switch
      {
          GlobalStrings.Predicate_Condition_Evaluator => (IConditionEvaluator)Activator.CreateInstance(typeof(PredicateConditionEvaluator<>).MakeGenericType(type))!,
          _ => null!
      };
}
