using Conditionals.Core.Areas.Events;
using FluentAssertions;
using Xunit;

namespace Conditionals.Core.Tests.Unit.Areas.Events;

public class EventSubscriptionTests
{
    [Fact]
    public void Calling_dispose_on_an_event_subscription_that_has_a_null_unsubscribe_action_should_not_throw_an_exception()
    {
        EventSubscription subscription = new EventSubscription(null!, null!);

        FluentActions.Invoking(() => subscription.Dispose()).Should().NotThrow();
    }
}
