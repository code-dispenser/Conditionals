using Conditionals.Core.Common.Utilities;
using Conditionals.Core.Tests.SharedDataAndFixtures.Events;
using FluentAssertions;
using FluentAssertions.Execution;
using Xunit;

namespace Conditionals.Core.Tests.Unit.Common.Utilities;

public class GeneralUtilsTests
{
    [Fact]
    public void Should_return_all_assembly_names_excluding_those_that_start_with_microsoft_or_system()
    {
        IReadOnlyList<(string qualifiedName, string fullName)> assemblyNames = GeneralUtils.AssemblyTypeNames;

        using (new AssertionScope())
        {
            assemblyNames.Count(f => f.qualifiedName.StartsWith("Microsoft.") || f.qualifiedName.StartsWith("System")).Should().Be(0);
            assemblyNames.Count.Should().BeGreaterThan(0);
        }

    }

    [Fact]
    public void Should_return_all_event_names_that_are_assignable_from_the_event_base_classes()
    {
        RuleEventInt r = new RuleEventInt("Some Rule", false, 1, 1, "TenantID", []);
        /*
            * Needed to add a rule event just to ensure that the Conditionals.Core.Tests.SharedDataAndFixtures assembly gets included in
            * AppDomain.CurrentDomain.GetAssemblies within the general utils class
        */
        IReadOnlyList<(string qualifiedName, string fullName)> eventTypeNames = GeneralUtils.EventTypeNames;

        using (new AssertionScope())
        {
            eventTypeNames.Count(f => f.qualifiedName.StartsWith("Microsoft.") || f.qualifiedName.StartsWith("System")).Should().Be(0);
            eventTypeNames.Count.Should().BeGreaterThan(2).And.BeLessThan(10);//I have 3
        }

    }
}
