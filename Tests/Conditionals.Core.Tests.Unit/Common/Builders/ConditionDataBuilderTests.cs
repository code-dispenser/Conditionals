using Conditionals.Core.Common.Builders;
using Conditionals.Core.Common.Models;
using Conditionals.Core.Tests.SharedDataAndFixtures.Data;
using Conditionals.Core.Tests.SharedDataAndFixtures.Models;
using FluentAssertions;
using FluentAssertions.Execution;
using Xunit;

namespace Conditionals.Core.Tests.Unit.Common.Builders;

public class ConditionDataBuilderTests
{
    [Fact]
    public void Should_be_able_to_start_creation_with_add_for_all_or_add_for_condition_both_of_which_should_return_the_interface_iadddata_for_the_chaining()
    {
        var thisForAll          = ConditionDataBuilder.AddForAny(StaticData.CustomerOne());
        var thisForCondition    = ConditionDataBuilder.AddForCondition("ConditionName", StaticData.CustomerOne());

        using (new AssertionScope())
        {
            thisForAll.Should().BeAssignableTo<IAddData>();
            thisForCondition.Should().BeAssignableTo<IAddData>();
        }
    }
    [Fact]
    public void After_the_add_you_Should_be_able_to_use_and_for_all_or_and_for_condition_both_of_which_should_return_the_interface_iadddata_for_the_chaining()
    {
        var thisForAll       = ConditionDataBuilder.AddForAny(StaticData.CustomerOne()).AndForCondition("ConditionOne", StaticData.CustomerTwo()).AndForAny(StaticData.SupplierOne());
        var thisForCondition = ConditionDataBuilder.AddForCondition("ConditionTwo", StaticData.CustomerOne()).AndForAny(StaticData.SupplierTwo()).AndForCondition("ConditionThree", StaticData.SupplierThree());

        using (new AssertionScope())
        {
            thisForAll.Should().BeAssignableTo<IAddData>();
            thisForCondition.Should().BeAssignableTo<IAddData>();
        }
    }
    [Fact]
    public void Create_should_return_condition_data_that_has_an_array_of_data_contexts_without_duplicates_based_on_name()
    {
        var theConditionData = ConditionDataBuilder.AddForAny(StaticData.CustomerOne())
                                             .AndForCondition("ConditionOne", StaticData.CustomerTwo())
                                             .AndForAny(StaticData.SupplierOne())
                                             .AndForCondition("ConditionOne", StaticData.SupplierThree())
                                             .AndForAny(StaticData.CustomerOne())
                                             .Create();

        var theCustomerContext = theConditionData.Contexts[1];

        Type contextOne     = theConditionData.Contexts[0].Data.GetType();
        Type contextTwo     = theConditionData.Contexts[1].Data.GetType();
        Type contextThree   = theConditionData.Contexts[2].Data.GetType();

        using (new AssertionScope())
        {
            theConditionData.Should().BeOfType<ConditionData>();
            theConditionData.Contexts.Should().HaveCount(3);

            theCustomerContext.Should().Match<DataContext>(d => d.ConditionName == "ConditionOne");

            contextOne.Should().Be(typeof(Customer));
            contextTwo.Should().Be(typeof(Customer));
            contextThree.Should().Be(typeof(Supplier));

        }
    }

    [Fact]
    public void Unless_an_instance_of_a_type_is_assigned_to_a_named_condition_it_will_be_treated_just_by_its_type_and_not_returned_if_that_type_has_already_been_added()
    {
        var theRuleData = ConditionDataBuilder.AddForAny(StaticData.CustomerOne())
                                             .AndForAny(StaticData.CustomerTwo())
                                             .AndForAny(StaticData.CustomerThree())
                                             .Create();

        //Customer one was added first the rest are classed as duplicated and ditched
        Customer contextOne = theRuleData.Contexts[0].Data;

        using (new AssertionScope())
        {
            theRuleData.Should().BeOfType<ConditionData>();
            theRuleData.Length.Should().Be(1);
            contextOne.CustomerName.Should().Be(StaticData.CustomerOne().CustomerName);

        }
    }
}
