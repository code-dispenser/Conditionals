using Conditionals.Core.Areas.Conditions;
using Conditionals.Core.Areas.Engine;
using Conditionals.Core.Areas.Evaluators;
using Conditionals.Core.Areas.Rules;
using Conditionals.Core.Common.Exceptions;
using Conditionals.Core.Common.Models;
using Conditionals.Core.Common.Seeds;
using Conditionals.Core.Tests.SharedDataAndFixtures.Data;
using Conditionals.Core.Tests.SharedDataAndFixtures.Evaluators;
using Conditionals.Core.Tests.SharedDataAndFixtures.Models;
using FluentAssertions;
using FluentAssertions.Execution;
using Xunit;

namespace Conditionals.Core.Tests.Unit.Areas.Engine;

public class ConditionEngineTests
{
    [Fact]
    public async Task Should_throw_a_rule_not_found_exception_trying_to_get_and_evaluate_a_rule_that_was_not_found_in_cache()
    
        =>  await FluentActions.Invoking(() => new ConditionEngine().EvaluateRule<None>("NoRule", null!)).Should().ThrowExactlyAsync<RuleNotFoundException>();

    [Fact]
    public void Should_throw_a_missing_condition_evaluator_exception_for_conditions_without_a_registered_evaluator()

        => FluentActions.Invoking(() => new ConditionEngine().EvaluatorResolver("FakeEvaluator", typeof(Customer))).Should().ThrowExactly<MissingConditionEvaluatorException>();


    [Fact]
    public void Given_valid_json_the_rule_from_json_method_should_return_a_valid_rule_without_adding_it_to_cache()
    {
        var conditionEngine = new ConditionEngine();
        var jsonString      = StaticData.JsonRuleText;

        //the rule name is RuleOne and the type is Rule<int>

        var convertedRule = ConditionEngine.RuleFromJson<int>(jsonString);
        var isRuleInCache = conditionEngine.ContainsRule("RuleOne",GlobalStrings.Default_TenantID,GlobalStrings.Default_CultureID);

        using(new AssertionScope())
        {
            isRuleInCache.Should().BeFalse();
            convertedRule.Should().Match<Rule<int>>(r => r.RuleName == "RuleOne" && r.ConditionSets[0].SetValue.GetType() == typeof(int) && r.ConditionSets[0].BooleanConditions != null);
        }
    }

    [Fact]
    public void Ingesting_a_valid_json_rule_should_get_converted_to_a_rule_and_added_to_and_or_update_any_existing_cached_version()
    {
        var conditionEngine = new ConditionEngine();
        var jsonString      = StaticData.JsonRuleText;

        //the rule name is RuleOne and the type is Rule<int>

        conditionEngine.IngestRuleFromJson<int>(jsonString);
        var isRuleInCache = conditionEngine.ContainsRule("RuleOne", GlobalStrings.Default_TenantID, GlobalStrings.Default_CultureID);

        isRuleInCache.Should().BeTrue();
    }
    [Fact]
    public void Ingesting_a_valid_json_rule_without_knowing_the_type_should_get_converted_to_a_rule_and_added_to_and_or_update_any_existing_cached_version()
    {
        var conditionEngine = new ConditionEngine();
        var jsonString      = StaticData.JsonRuleText;

        //the rule name is RuleOne and the type is Rule<int>
        conditionEngine.IngestRuleFromJson(jsonString);
        conditionEngine.ContainsRule("RuleOne").Should().BeTrue();

    }

    [Fact]
    public void Ingesting_a_json_rule_without_knowing_the_type_that_is_missing_the_value_type_should_throw_an_invalid_system_data_type_exception()
        => FluentActions.Invoking(() => new ConditionEngine().IngestRuleFromJson(StaticData.JsonWithoutAValueTypeName)).Should().ThrowExactly<InvalidSystemDataTypeException>();
    

    [Fact]
    public void Ingesting_a_json_rule_without_knowing_the_type_that_has_a_null_for_the_rule_name_should_throw_a_rule_from_json_exception()
        
        =>  FluentActions.Invoking(() => new ConditionEngine().IngestRuleFromJson(StaticData.JsonWithARuleNameOfNull)).Should().ThrowExactly<RuleFromJsonException>();
    
    [Fact]
    public void Ingesting_a_json_rule_without_knowing_the_type_that_is_missing_the_rule_name_property_should_throw_a_rule_from_json_exception()

        => FluentActions.Invoking(() => new ConditionEngine().IngestRuleFromJson(StaticData.JsonWithARuleNameProperty)).Should().ThrowExactly<RuleFromJsonException>();

    [Fact]
    public void Should_be_able_to_add_a_rule_to_cache()
    {
        var conditionEngine = new ConditionEngine();
        var condition       = new PredicateCondition<Customer>("ConditionOne", c => c.CustomerNo == 111, "Customer number should be 111");
        var conditionSet    = new ConditionSet<None>("SetOne", None.Value, condition);
        var rule            = new Rule<None>("RuleOne",None.Value,conditionSet);
        
        conditionEngine.AddOrUpdateRule(rule);

        conditionEngine.ContainsRule("RuleOne").Should().BeTrue();

    }
    [Fact]
    public void Should_be_able_to_remove_a_rule_from_cache()
    {
        var conditionEngine = new ConditionEngine();
        var condition       = new PredicateCondition<Customer>("ConditionOne", c => c.CustomerNo == 111, "Customer number should be 111");
        var conditionSet    = new ConditionSet<None>("SetOne", None.Value, condition);
        var rule            = new Rule<None>("RuleOne",None.Value,conditionSet);
        
        conditionEngine.AddOrUpdateRule(rule);
        bool IsInCache() => conditionEngine.ContainsRule("RuleOne");
        var isRuleInCache = IsInCache();

        conditionEngine.RemoveRule("RuleOne");

        var isRuleStillInCache = IsInCache();

        using(new AssertionScope())
        {
            isRuleInCache.Should().BeTrue();
            isRuleStillInCache.Should().BeFalse();
        }
    }

    [Fact]
    public void The_try_get_rule_should_return_false_if_the_rule_is_not_in_cache()
    
        => new ConditionEngine().TryGetRule<Rule<int>>("RuleOne", out _).Should().BeFalse();

    [Fact]
    public async Task Should_be_able_to_evaluate_a_rule_that_was_found_in_cache()
    {
        var conditionEngine = new ConditionEngine();
        var condition       = new PredicateCondition<Customer>("ConditionOne", c => c.CustomerNo == 111, "Customer number should be 111");
        var conditionSet    = new ConditionSet<None>("SetOne", None.Value, condition);
        var rule            = new Rule<None>("RuleOne", None.Value, conditionSet);
        var data            = new ConditionData([new(StaticData.CustomerOne())]);

        conditionEngine.AddOrUpdateRule(rule);

        var ruleResult = await conditionEngine.EvaluateRule<None>("RuleOne", data);

        ruleResult.Should().Match<RuleResult<None>>(r => r.IsSuccess == true && r.EvaluationCount == 1);
    }

    [Fact]
    public void Should_be_able_to_register_an_open_generic_condition_evaluator_with_the_engine()
    {
        var conditionEngine = new ConditionEngine();
        conditionEngine.RegisterCustomEvaluator("ConditionEvaluatorBaseTester", typeof(ConditionEvaluatorBaseTester<>));

        var evaluator = conditionEngine.EvaluatorResolver("ConditionEvaluatorBaseTester", typeof(Customer));

        evaluator.Should().BeOfType<ConditionEvaluatorBaseTester<Customer>>();

    }

    [Fact]
    public void Should_be_able_to_register_a_closed_generic_condition_evaluator_with_the_engine()
    {
        var conditionEngine = new ConditionEngine();
        conditionEngine.RegisterCustomEvaluator("ClosedGenericCustomerEvaluator", typeof(ClosedGenericCustomerEvaluator));

        var evaluator = conditionEngine.EvaluatorResolver("ClosedGenericCustomerEvaluator", typeof(Customer));

        evaluator.Should().BeOfType<ClosedGenericCustomerEvaluator>();

    }

    [Fact]
    public void Should_be_able_to_add_a_rule_to_and_get_the_rule_back_from_cache()
    {
        var conditionEngine = new ConditionEngine();
        var condition       = new PredicateCondition<Customer>("ConditionOne", c => c.CustomerNo == 111, "Customer number should be 111");
        var conditionSet    = new ConditionSet<None>("SetOne", None.Value, condition);
        var rule            = new Rule<None>("RuleOne", None.Value, conditionSet);

        conditionEngine.AddOrUpdateRule(rule);

        var ruleFromCache = conditionEngine.TryGetRule<None>("RuleOne", out var cachedRule) ? cachedRule : null;

        ruleFromCache.Should().Match<Rule<None>>(r => r.RuleName == "RuleOne" && r.ConditionSets[0].SetName == "SetOne");

    }

    [Fact]
    public void Should_be_able_to_get_the_correct_evaluator_type()
    {
        var conditionEngine = new ConditionEngine();

        var addressRegexEvaluator       = conditionEngine.EvaluatorResolver(GlobalStrings.Regex_Condition_Evaluator, typeof(Address));
        var customerRegexEvaluator      = conditionEngine.EvaluatorResolver(GlobalStrings.Regex_Condition_Evaluator, typeof(Customer));
        var personPredicateEvaluator    = conditionEngine.EvaluatorResolver(GlobalStrings.Predicate_Condition_Evaluator, typeof(Person));
        var supplierPredicateEvaluator  = conditionEngine.EvaluatorResolver(GlobalStrings.Predicate_Condition_Evaluator, typeof(Supplier));

        using(new AssertionScope())
        {
            addressRegexEvaluator.Should().BeOfType<RegexConditionEvaluator<Address>>();
            customerRegexEvaluator.Should().BeOfType<RegexConditionEvaluator<Customer>>();
            personPredicateEvaluator.Should().BeOfType<PredicateConditionEvaluator<Person>>();
            supplierPredicateEvaluator.Should().BeOfType<PredicateConditionEvaluator<Supplier>>();
        }


    }

    [Fact]
    public void Ingesting_a_rule_without_a_value_type_Property_should_cause_an_invalid_system_data_type_exception()
    
        =>  FluentActions.Invoking(() => new ConditionEngine().IngestRuleFromJson(StaticData.JsonWithoutAValueTypeName)).Should().ThrowExactly<InvalidSystemDataTypeException>();

    [Fact]
    public void Ingesting_a_rule_with_a_null_value_type_property_should_cause_an_invalid_system_data_type_exception()

        =>  FluentActions.Invoking(() => new ConditionEngine().IngestRuleFromJson(StaticData.JsonWithANullValueTypeName)).Should().ThrowExactly<InvalidSystemDataTypeException>();


}
