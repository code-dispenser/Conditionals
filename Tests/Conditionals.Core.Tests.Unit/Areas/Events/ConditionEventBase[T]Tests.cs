using Conditionals.Core.Areas.Events;
using Conditionals.Core.Common.Exceptions;
using Conditionals.Core.Common.Seeds;
using Conditionals.Core.Tests.SharedDataAndFixtures.Data;
using Conditionals.Core.Tests.SharedDataAndFixtures.Events;
using Conditionals.Core.Tests.SharedDataAndFixtures.Models;
using FluentAssertions;
using FluentAssertions.Execution;
using System.Text.Json;
using Xunit;

namespace Conditionals.Core.Tests.Unit.Areas.Events;

public class ConditionEventBaseTests
{
    [Fact]
    public void All_properties_should_be_set_via_the_base_constructor()
    {
        var jsonString             = "Fake Context Data";
        var customerConditionEvent = new ConditionEventCustomer("ConditionTest", false, jsonString, GlobalStrings.Default_TenantID, [new SystemException()], new SystemException());
        
        customerConditionEvent.Should().Match<ConditionEventBase<Customer>>(c => c.SenderName == "ConditionTest" && c.IsSuccessEvent == false && c.TenantID == GlobalStrings.Default_TenantID 
                                                                         && c.ExecutionExceptions.Count == 1 && c.ConversionException!.GetType() == typeof(SystemException));
    }

    [Fact]
    public void Serializable_context_data_should_be_deserialized_via_the_try_get_data_method()
    {
        var jsonString              = JsonSerializer.Serialize<Customer>(StaticData.CustomerOne()); 
        var customerConditionEvent  = new ConditionEventCustomer("ConditionTest", false, jsonString, GlobalStrings.Default_TenantID, [new SystemException()], null);

        var baseConditionEvent      = customerConditionEvent as ConditionEventBase<Customer>;

        _ = baseConditionEvent.TryGetData(out var customerData);

        customerData.Should().BeEquivalentTo(StaticData.CustomerOne());
    }

    [Fact]
    public void A_serialization_issue_should_set_the_conversion_exception_property_and_the_try_get_data_result_to_false()
    {
        string jsonString = "null";
        var customerConditionEvent = new ConditionEventCustomer("ConditionTest", false, jsonString, GlobalStrings.Default_TenantID, [new SystemException()], new SystemException());

        var baseConditionEvent  = customerConditionEvent as ConditionEventBase<Customer>;
        var tryGetResult        = baseConditionEvent.TryGetData(out _);

        tryGetResult.Should().BeFalse();


    }

    //[Fact]
    //public void Deserialization_issues_should_set_the_conversion_exception_with_the_try_get_data_method_returning_false()
    //{
    //    var claim                   = new Claim("ClaimType", "Claim Value");
    //    var jsonString              = JsonSerializer.Serialize<Claim>(claim);
    //    var claimConditionEvent     = new ConditionEventCustomer("ConditionTest", false, jsonString, GlobalStrings.Default_TenantID, [new SystemException()], null);

    //    var baseConditionEvent = claimConditionEvent as ConditionEventBase<Customer>;//Customer is not a claim

    //    var tryGetResult = baseConditionEvent.TryGetData(out var exceptionData);

    //    using(new AssertionScope())
    //    {
    //        tryGetResult.Should().BeFalse();
    //        exceptionData.Should().BeEquivalentTo(default(Claim));
    //        baseConditionEvent.ConversionException.Should().BeOfType<NotSupportedException>();

    //    }
       
    //}
    [Fact]
    public void A_deserialization_that_results_in_a_null_object_should_cause_the_try_get_data_method_to_return_false_and_set_the_conversion_exception_with_a_deserialization_to_null_exception()
    {
        string jsonString = JsonSerializer.Serialize<Customer>(null!);//just outputs "null" without exception.
        var customerConditionEvent = new ConditionEventCustomer("ConditionTest", false, jsonString, GlobalStrings.Default_TenantID, [new SystemException()], null);

        var baseConditionEvent = customerConditionEvent as ConditionEventBase<Customer>;

        var tryGetResult = baseConditionEvent.TryGetData(out _);

        using(new AssertionScope())
        {
            tryGetResult.Should().BeFalse();
            baseConditionEvent.ConversionException.Should().BeOfType<DeserializationToNullException>();
            
        }
       

    }
    [Fact]
    public void A_null_passed_as_the_Json_string_should_cause_the_try_get_data_method_to_return_false()
    {
        var customerConditionEvent = new ConditionEventCustomer("ConditionTest", false, null!, GlobalStrings.Default_TenantID, [new SystemException()], null);

        var baseConditionEvent = customerConditionEvent as ConditionEventBase<Customer>;

        var tryGetResult = baseConditionEvent.TryGetData(out _);

        using (new AssertionScope())
        {
            tryGetResult.Should().BeFalse();
            baseConditionEvent.ConversionException.Should().BeNull();

        }
    }

    [Fact]
    public void A_null_execution_exception_list_should_default_to_an_empty_list()
    {
        var customerConditionEvent = new ConditionEventCustomer("ConditionTest", false, null!, GlobalStrings.Default_TenantID, null!, null);

        var baseConditionEvent = customerConditionEvent as ConditionEventBase<Customer>;

        baseConditionEvent.ExecutionExceptions.Count.Should().Be(0);

    }
}
