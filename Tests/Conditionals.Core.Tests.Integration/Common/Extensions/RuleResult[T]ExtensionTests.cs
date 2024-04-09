using Conditionals.Core.Areas.Conditions;
using Conditionals.Core.Areas.Engine;
using Conditionals.Core.Areas.Rules;
using Conditionals.Core.Common.Extensions;
using Conditionals.Core.Common.Models;
using Conditionals.Core.Tests.SharedDataAndFixtures.Data;
using Conditionals.Core.Tests.SharedDataAndFixtures.Models;
using FluentAssertions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace Conditionals.Core.Tests.Integration.Common.Extensions;

public class RuleResultExtensionTests
{
    [Fact]
    public async Task Should_be_able_to_chain_an_unsuccessful_rule_result_in_order_to_run_another_rule_from_the_engine()
    {
        var conditionEngine = new ConditionEngine();

        var customerCondition = new PredicateCondition<Customer>("HasAnAddress", c => c.Address != null, "The customer needs to have an address");
        var customerRule      = new Rule<None>("CustomerRule", None.Value, new ConditionSet<None>("CustomerSet",None.Value,customerCondition));

        var supplierCondition = new PredicateCondition<Supplier>("SupplierNo", s => s.SupplierNo == 111, "Supplier No. should be 111");
        var supplierRule      = new Rule<None>("SupplierRule",None.Value,new ConditionSet<None>("SupplierSet", None.Value,supplierCondition));

        conditionEngine.AddOrUpdateRule(customerRule);
        conditionEngine.AddOrUpdateRule(supplierRule);

        var customerData = new ConditionData([new(StaticData.CustomerOne())]);
        var supplierData = new ConditionData([new(StaticData.SupplierOne())]);


        var theResult = await conditionEngine.EvaluateRule<None>("CustomerRule", customerData)
                                    .OnFailure("SupplierRule", conditionEngine, supplierData);

        theResult.Should().Match<RuleResult<None>>(r => r.IsSuccess == true && r.PreviousRuleResult!.IsSuccess == false);

    }
    [Fact]
    public async Task Should_be_able_to_chain_an_unsuccessful_rule_result_in_order_to_return_another_result_using_a_func()
    {
        var conditionEngine = new ConditionEngine();

        var customerCondition = new PredicateCondition<Customer>("HasAnAddress", c => c.Address != null, "The customer needs to have an address");
        var customerRule = new Rule<None>("CustomerRule", None.Value, new ConditionSet<None>("CustomerSet", None.Value, customerCondition));

        var supplierCondition = new PredicateCondition<Supplier>("SupplierNo", s => s.SupplierNo == 111, "Supplier No. should be 111");
        var supplierRule = new Rule<None>("SupplierRule", None.Value, new ConditionSet<None>("SupplierSet", None.Value, supplierCondition));

        conditionEngine.AddOrUpdateRule(customerRule);
        conditionEngine.AddOrUpdateRule(supplierRule);

        var customerData = new ConditionData([new(StaticData.CustomerOne())]);
        var supplierData = new ConditionData([new(StaticData.SupplierOne())]);


        var theResult = await conditionEngine.EvaluateRule<None>("CustomerRule", customerData)
                                    .OnFailure(async _ => await conditionEngine.EvaluateRule<None>("SupplierRule", supplierData));


        theResult.Should().Match<RuleResult<None>>(r => r.IsSuccess == true && r.PreviousRuleResult!.IsSuccess == false);

    }


    [Fact]
    public async Task Should_not_run_the_next_chained_on_success_result_from_the_engine_if_the_result_was_false()
    {
        var conditionEngine = new ConditionEngine();

        var customerCondition = new PredicateCondition<Customer>("HasAnAddress", c => c.Address != null, "The customer needs to have an address");
        var customerRule = new Rule<None>("CustomerRule", None.Value, new ConditionSet<None>("CustomerSet", None.Value, customerCondition));

        var supplierCondition = new PredicateCondition<Supplier>("SupplierNo", s => s.SupplierNo == 111, "Supplier No. should be 111");
        var supplierRule = new Rule<None>("SupplierRule", None.Value, new ConditionSet<None>("SupplierSet", None.Value, supplierCondition));

        conditionEngine.AddOrUpdateRule(customerRule);
        conditionEngine.AddOrUpdateRule(supplierRule);

        var customerData = new ConditionData([new(StaticData.CustomerOne())]);
        var supplierData = new ConditionData([new(StaticData.SupplierOne())]);


        var theResult = await conditionEngine.EvaluateRule<None>("CustomerRule", customerData)
                                    .OnSuccess("SupplierRule", conditionEngine, supplierData);

        theResult.Should().Match<RuleResult<None>>(r => r.IsSuccess == false && r.PreviousRuleResult == null);

    }
    [Fact]
    public async Task Should_not_run_the_next_chained_on_success_result_using_a_func_if_the_result_was_false()
    {
        var conditionEngine = new ConditionEngine();

        var customerCondition = new PredicateCondition<Customer>("HasAnAddress", c => c.Address != null, "The customer needs to have an address");
        var customerRule = new Rule<None>("CustomerRule", None.Value, new ConditionSet<None>("CustomerSet", None.Value, customerCondition));

        var supplierCondition = new PredicateCondition<Supplier>("SupplierNo", s => s.SupplierNo == 111, "Supplier No. should be 111");
        var supplierRule = new Rule<None>("SupplierRule", None.Value, new ConditionSet<None>("SupplierSet", None.Value, supplierCondition));

        conditionEngine.AddOrUpdateRule(customerRule);
        conditionEngine.AddOrUpdateRule(supplierRule);

        var customerData = new ConditionData([new(StaticData.CustomerOne())]);
        var supplierData = new ConditionData([new(StaticData.SupplierOne())]);


        var theResult = await conditionEngine.EvaluateRule<None>("CustomerRule", customerData)
                                    .OnSuccess(async _ => await conditionEngine.EvaluateRule<None>("SupplierRule", supplierData));

        theResult.Should().Match<RuleResult<None>>(r => r.IsSuccess == false && r.PreviousRuleResult == null);

    }


    [Fact]
    public async Task Should_be_able_to_chain_a_successful_rule_result_in_order_to_run_another_rule_from_the_engine()
    {
        var conditionEngine = new ConditionEngine();

        var customerCondition = new PredicateCondition<Customer>("HasAnAddress", c => c.Address != null, "The customer needs to have an address");
        var customerRule = new Rule<None>("CustomerRule", None.Value, new ConditionSet<None>("CustomerSet", None.Value, customerCondition));

        var supplierCondition = new PredicateCondition<Supplier>("SupplierNo", s => s.SupplierNo == 111, "Supplier No. should be 111");
        var supplierRule = new Rule<None>("SupplierRule", None.Value, new ConditionSet<None>("SupplierSet", None.Value, supplierCondition));

        conditionEngine.AddOrUpdateRule(customerRule);
        conditionEngine.AddOrUpdateRule(supplierRule);

        var customerData = new ConditionData([new(StaticData.CustomerOne())]);
        var supplierData = new ConditionData([new(StaticData.SupplierOne())]);


        var theResult = await conditionEngine.EvaluateRule<None>("SupplierRule", supplierData)
                                    .OnSuccess("CustomerRule", conditionEngine, customerData);

        theResult.Should().Match<RuleResult<None>>(r => r.IsSuccess == false && r.PreviousRuleResult!.IsSuccess == true);

    }

    [Fact]
    public async Task Should_be_able_to_chain_a_successful_rule_result_and_return_another_result_using_a_func()
    {
        var conditionEngine = new ConditionEngine();

        var customerCondition = new PredicateCondition<Customer>("HasAnAddress", c => c.Address != null, "The customer needs to have an address");
        var customerRule = new Rule<None>("CustomerRule", None.Value, new ConditionSet<None>("CustomerSet", None.Value, customerCondition));

        var supplierCondition = new PredicateCondition<Supplier>("SupplierNo", s => s.SupplierNo == 111, "Supplier No. should be 111");
        var supplierRule = new Rule<None>("SupplierRule", None.Value, new ConditionSet<None>("SupplierSet", None.Value, supplierCondition));

        conditionEngine.AddOrUpdateRule(customerRule);
        conditionEngine.AddOrUpdateRule(supplierRule);

        var customerData = new ConditionData([new(StaticData.CustomerOne())]);
        var supplierData = new ConditionData([new(StaticData.SupplierOne())]);


        var theResult = await conditionEngine.EvaluateRule<None>("SupplierRule", supplierData)
                                    .OnSuccess(async _ => await conditionEngine.EvaluateRule<None>("CustomerRule",customerData));

        theResult.Should().Match<RuleResult<None>>(r => r.IsSuccess == false && r.PreviousRuleResult!.IsSuccess == true);

    }


    [Fact]
    public async Task Should_not_run_the_next_chained_on_failure_result_from_the_engine_if_the_result_was_true()
    {
        var conditionEngine = new ConditionEngine();

        var customerCondition = new PredicateCondition<Customer>("HasAnAddress", c => c.Address != null, "The customer needs to have an address");
        var customerRule = new Rule<None>("CustomerRule", None.Value, new ConditionSet<None>("CustomerSet", None.Value, customerCondition));

        var supplierCondition = new PredicateCondition<Supplier>("SupplierNo", s => s.SupplierNo == 111, "Supplier No. should be 111");
        var supplierRule = new Rule<None>("SupplierRule", None.Value, new ConditionSet<None>("SupplierSet", None.Value, supplierCondition));

        conditionEngine.AddOrUpdateRule(customerRule);
        conditionEngine.AddOrUpdateRule(supplierRule);

        var customerData = new ConditionData([new(StaticData.CustomerOne())]);
        var supplierData = new ConditionData([new(StaticData.SupplierOne())]);


        var theResult = await conditionEngine.EvaluateRule<None>("SupplierRule", supplierData)
                                    .OnFailure("CustomerRule", conditionEngine, customerData);

        theResult.Should().Match<RuleResult<None>>(r => r.IsSuccess == true && r.PreviousRuleResult == null);

    }
    [Fact]
    public async Task Should_not_run_the_next_chained_on_failure_result_using_the_func_if_the_result_was_true()
    {
        var conditionEngine = new ConditionEngine();

        var customerCondition = new PredicateCondition<Customer>("HasAnAddress", c => c.Address != null, "The customer needs to have an address");
        var customerRule = new Rule<None>("CustomerRule", None.Value, new ConditionSet<None>("CustomerSet", None.Value, customerCondition));

        var supplierCondition = new PredicateCondition<Supplier>("SupplierNo", s => s.SupplierNo == 111, "Supplier No. should be 111");
        var supplierRule = new Rule<None>("SupplierRule", None.Value, new ConditionSet<None>("SupplierSet", None.Value, supplierCondition));

        conditionEngine.AddOrUpdateRule(customerRule);
        conditionEngine.AddOrUpdateRule(supplierRule);

        var customerData = new ConditionData([new(StaticData.CustomerOne())]);
        var supplierData = new ConditionData([new(StaticData.SupplierOne())]);


        var theResult = await conditionEngine.EvaluateRule<None>("SupplierRule", supplierData)
                                    .OnFailure(async _ => await conditionEngine.EvaluateRule<None>("CustomerRule", customerData));

        theResult.Should().Match<RuleResult<None>>(r => r.IsSuccess == true && r.PreviousRuleResult == null);

    }
}
