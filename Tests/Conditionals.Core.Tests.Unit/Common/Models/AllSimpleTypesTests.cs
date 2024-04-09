using Conditionals.Core.Common.Models;
using Conditionals.Core.Common.Seeds;
using Conditionals.Core.Tests.SharedDataAndFixtures.Data;
using Conditionals.Core.Tests.SharedDataAndFixtures.Models;
using FluentAssertions;
using FluentAssertions.Execution;
using Xunit;

namespace Conditionals.Core.Tests.Unit.Common.Models;

public class AllSimpleTypesTests
{
    [Fact]
    public void Data_context_should_be_able_to_set_properties_via_the_constructor()
    {
        dynamic testData    = new { Name = "John Doe", Age = 30 };
        var conditionName   = "Test Condition";

        var dataContext = new DataContext(testData, conditionName);
            
        using (new AssertionScope())
        {
            Assert.Equal(dataContext.Data, testData);//cant test dynamic with Fluent Assertions
            dataContext.ConditionName.Should().Be(conditionName);
        }
    }
    [Fact]
    public void Data_context_omitting_the_condition_name_should_set_it_to_empty()
    {
        dynamic testData = new { Name = "John Doe", Age = 30 };

        var dataContext = new DataContext(testData);

        using (new AssertionScope())
        {
            Assert.Equal(dataContext.Data, testData);//cant test dynamic with Fluent Assertions
            dataContext.ConditionName.Should().BeEmpty();
        }
    }
    [Fact]
    public void Data_context_the_setters_should_be_called_when_using_the_with_expression()//keep code coverage happy
    {
        dynamic testDataOne = new { Name = "John Doe", Age = 30 };
        dynamic testDataTwo = new { Name = "Jane Doe", Age = 21 };

        var conditionNameOne = "Test Condition";
        var conditionNameTwo = "Updated Condition";

        var dataContext = new DataContext(testDataOne,conditionNameOne);
        var withContext = dataContext with { Data = testDataTwo, ConditionName = conditionNameTwo };

        using (new AssertionScope())
        {
            Assert.Equal(withContext.Data, testDataTwo);//cant test dynamic with Fluent Assertions
            withContext.ConditionName.Should().Be(conditionNameTwo);
        }
    }

    [Fact]
    public void Evaluation_result_should_be_able_to_set_the_properties_via_the_constructor()

        =>  new EvaluationResult(false, "Failed", new SystemException())
                .Should().Match<EvaluationResult>(r => r.IsSuccess ==false && r.FailureMessage == "Failed" && r.Exception!.GetType() == typeof(SystemException));

    [Fact]
    public void Evaluation_result_should_allow_a_null_failure_message_and_or_a_null_exception()

        => new EvaluationResult(true,null,null)
                .Should().Match<EvaluationResult>(r => r.IsSuccess == true && r.FailureMessage == null && r.Exception == null);

    [Fact]
    public void Evaluation_result_the_setters_should_be_called_when_using_the_with_expression()//keep code coverage happy
    {
        var evaluationResult = new EvaluationResult(false, "failed", new SystemException());
        var newResult = evaluationResult with { IsSuccess = true, FailureMessage = null, Exception = null};

        newResult.Should().Match<EvaluationResult>(r => r.IsSuccess == true && r.FailureMessage == null && r.Exception == null);
    }


    [Fact]
    public void Condition_data_should_be_able_to_set_properties_via_the_constructor()
    {
        var dataContext     = new DataContext(new { Name = "John Doe", Age = 30 }, "Test Condition");
        var conditionData   = new ConditionData([dataContext], "TenantOne");

        using (new AssertionScope())
        {
            Assert.Equal(conditionData.Contexts[0], dataContext);//cant test dynamic with Fluent Assertions
            conditionData.TenantID.Should().Be("TenantOne");
        }
    }
    [Fact]
    public void Condition_data_the_optional_tenantID_property_should_default_to_all_tenants()
    {
        var dataContexts  = new DataContext[] { new DataContext(new { Name = "John Doe", Age = 30 }, "Test Condition") };
        var conditionData = new ConditionData(dataContexts, null);

        using (new AssertionScope())
        {
            Assert.Equal(conditionData.Contexts[0], dataContexts[0]);
            conditionData.TenantID.Should().Be(GlobalStrings.Default_TenantID);
        }
    }
    [Fact]
    public void Condition_data_the_data_length_property_should_return_the_correct_number_of_contexts()
    {
        var dataContexts    = new DataContext[] { new DataContext(new { Name = "John Doe", Age = 30 }, "Test Condition"), new DataContext(new { Name = "Jane Doe", Age = 21 }, "Another Condition") };
        var conditionData   = new ConditionData(dataContexts);
        
        conditionData.Length.Should().Be(2);
    }

    [Fact]
    public void Condition_data_null_contexts_should_cause_an_exception()
        
        => FluentActions.Invoking(() => new ConditionData(null!)).Should().Throw<ArgumentException>();

    [Fact]
    public void Condition_data_the_single_context_method_should_create_condition_data_with_a_single_data_context_with_default_tenant_id_if_not_specified()

        => ConditionData.SingleContext(new { Name = "John Doe", Age = 30 })
                            .Should().Match<ConditionData>(c => c.Length == 1 && c.TenantID == GlobalStrings.Default_TenantID);

    [Fact]
    public void Condition_data_the_single_context_method_should_create_condition_data_with_a_single_data_context()

    => ConditionData.SingleContext(new { Name = "John Doe", Age = 30 }, "TenantOne")
                        .Should().Match<ConditionData>(c => c.Length == 1 && c.TenantID == "TenantOne");

    [Fact]
    public void None_the_value_property_should_return_none_null_instances()
    {
        var none = None.Value;
        
        none.Should().NotBeNull();
    }
    [Fact]
    public void None_the_to_string_method_should_return_the_empty_set_character()
    {
        var none = None.Value;

        none.ToString().Should().Be("Ø");
    }


    [Fact]
    public void The_cache_key_should_return_the_values_it_was_assigned()
    {
        var theCacheKey = new CacheKey("TheItemName");

        var keepTestCoverageHappyCacheKey = theCacheKey with { ItemName = "NewItemName", TenantID = "TenantOne", CultureID = "en-US" };

        using (new AssertionScope())
        {
            theCacheKey.Should().Match<CacheKey>(c => c.ItemName == "TheItemName" && c.TenantID == GlobalStrings.Default_TenantID && c.CultureID == GlobalStrings.Default_CultureID);

            keepTestCoverageHappyCacheKey.Should().Match<CacheKey>(c => c.ItemName == "NewItemName" && c.TenantID == "TenantOne" && c.CultureID == "en-US");
        }

    }
    [Fact]
    public void The_cache_item_should_return_the_value_it_was_assigned()
    {
        var theCacheItem = new CacheItem(StaticData.CustomerOne());

        var keepTestCoverageHappyCacheItem = theCacheItem with { Value = StaticData.CustomerTwo() };

        using (new AssertionScope())
        {
            theCacheItem.Value.Should().BeOfType<Customer>().And.Match<Customer>(c => c.CustomerName == "CustomerOne");

            keepTestCoverageHappyCacheItem.Value.Should().BeOfType<Customer>().And.Match<Customer>(c => c.CustomerName == "CustomerTwo");
        }

    }
}
