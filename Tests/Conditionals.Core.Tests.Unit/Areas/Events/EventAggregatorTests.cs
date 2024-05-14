using Conditionals.Core.Areas.Events;
using Conditionals.Core.Tests.SharedDataAndFixtures.Data;
using Conditionals.Core.Tests.SharedDataAndFixtures.Events;
using FluentAssertions;
using FluentAssertions.Execution;
using Xunit;

namespace Conditionals.Core.Tests.Unit.Areas.Events;

public class EventAggregatorTests
{
    internal readonly EventAggregator _eventAggregator;

    public EventAggregatorTests()
    {
        _eventAggregator = new EventAggregator();
    }

    [Fact]
    public void Should_get_an_event_subscription_when_subscribing_to_an_event()
    {
        EventSubscription theEventSubscription = _eventAggregator.Subscribe<ConditionEventCustomer>((_, _) => Task.CompletedTask);

        theEventSubscription.Should().NotBeNull();
    }

    [Fact]
    public async Task Should_receive_condition_events_that_are_subscribed_to()
    {
        var conditionEvent = new ConditionEventCustomer("SomeSender", true, StaticData.CustomerOneAsJsonString(), "TenantID", []);

        EventSubscription theEventSubscription = _eventAggregator.Subscribe<ConditionEventCustomer>(HandleEvent);

        _eventAggregator.Publish(conditionEvent, CancellationToken.None);
        await Task.CompletedTask;
        /*
            * Publish is fire and forget, nothing to wait for so asserting in the handler
        */
        static async Task HandleEvent(ConditionEventCustomer theEvent, CancellationToken cancellationToken)
        {
            theEvent.SenderName.Should().Be("SomeSender");
            await Task.CompletedTask;
        }
    }
    [Fact]
    public async Task Should_receive_rule_events_that_are_subscribed_to()
    {
        var ruleEvent = new RuleEventInt("SomeSender", true, 42,20, "TenantID", []);

        EventSubscription theEventSubscription = _eventAggregator.Subscribe<RuleEventInt>(HandleEvent);

        _eventAggregator.Publish(ruleEvent, CancellationToken.None);
        await Task.CompletedTask;
        /*
            * Publish is fire and forget, nothing to wait for so asserting in the handler
        */
        static async Task HandleEvent(RuleEventInt theEvent, CancellationToken cancellationToken)
        {
            theEvent.SenderName.Should().Be("SomeSender");
            await Task.CompletedTask;
        }
    }
    [Fact]
    public async Task Should_be_able_to_unsubscribe_from_events_by_disposing_the_subscription()
    {
        int theHandleCount = 0;
        var conditionEvent = new ConditionEventCustomer("SomeSender", true, StaticData.CustomerOneAsJsonString(), "TenantID", []);

        using (EventSubscription theEventSubscription = _eventAggregator.Subscribe<ConditionEventCustomer>(HandleEvent))
        {
            _eventAggregator.Publish(conditionEvent, CancellationToken.None);
        }

        _eventAggregator.Publish(conditionEvent, CancellationToken.None);
        await Task.Delay(50);//
        
        theHandleCount.Should().Be(1);

        async Task HandleEvent(ConditionEventCustomer theEvent, CancellationToken cancellationToken)
        {
            theHandleCount++;
            await Task.CompletedTask;
        }
      
    }

    [Fact]
    public void The_weak_reference_comparer_should_return_true_for_identical_references()
    {
        var theComparer         = new EventAggregator.WeakReferenceDelegateComparer();
        var weakRefHandlerOne   = new WeakReference<Delegate>(RuleEventHandler);
        var weakRefHandlerTwo   = new WeakReference<Delegate>(RuleEventHandler);

        theComparer.Equals(weakRefHandlerOne, weakRefHandlerTwo).Should().BeTrue();

        static async Task RuleEventHandler(RuleEventInt ruleEvent, CancellationToken cancellationToken) { await Task.CompletedTask; }
    }
    [Fact]
    public void The_weak_reference_comparer_should_return_false_for_different_references()
    {
        var theComparer = new EventAggregator.WeakReferenceDelegateComparer();
        var weakRefHandlerOne = new WeakReference<Delegate>(RuleEventHandlerOne);
        var weakRefHandlerTwo = new WeakReference<Delegate>(RuleEventHandlerTwo);

        theComparer.Equals(weakRefHandlerOne, weakRefHandlerTwo).Should().BeFalse();

        async Task RuleEventHandlerOne(RuleEventInt ruleEvent, CancellationToken cancellationToken) { await Task.CompletedTask; }
        async Task RuleEventHandlerTwo(RuleEventInt ruleEvent, CancellationToken cancellationToken) { await Task.CompletedTask; }
    }

    [Fact]
    public void The_weak_reference_comparer_should_return_false_for_a_null_reference()
    {
        var theComparer = new EventAggregator.WeakReferenceDelegateComparer();
        WeakReference<Delegate>? weakRefHandlerOne = null;
        var weakRefHandlerTwo = new WeakReference<Delegate>(RuleEventHandlerTwo);

        theComparer.Equals(weakRefHandlerOne, weakRefHandlerTwo).Should().BeFalse();

        static async Task RuleEventHandlerTwo(RuleEventInt ruleEvent, CancellationToken cancellationToken) { await Task.CompletedTask; }
    }

    [Fact]
    public void The_weak_reference_comparer_should_return_false_for_a_null_reference_target()
    {
        var theComparer = new EventAggregator.WeakReferenceDelegateComparer();
        WeakReference<Delegate>? weakRefHandlerOne = new(null!);
        var weakRefHandlerTwo = new WeakReference<Delegate>(RuleEventHandlerTwo);

        theComparer.Equals(weakRefHandlerOne, weakRefHandlerTwo).Should().BeFalse();

        static async Task RuleEventHandlerTwo(RuleEventInt ruleEvent, CancellationToken cancellationToken) { await Task.CompletedTask; }
    }

    [Fact]
    public void The_weak_reference_comparer_get_hash_code_should_return_an_integer_value()
    {
        var theComparer         = new EventAggregator.WeakReferenceDelegateComparer();
        var weakRefHandlerOne   = new WeakReference<Delegate>(RuleEventHandler);
        var weakRefHandlerTwo   = new WeakReference<Delegate>(RuleEventHandler);

        var theCodeValue = theComparer.GetHashCode(weakRefHandlerOne);

        using (new AssertionScope())
        {
            theCodeValue.Should().BeGreaterThan(0);
            theCodeValue.Should().Be(theComparer.GetHashCode(weakRefHandlerTwo));
        }

        static async Task RuleEventHandler(RuleEventInt ruleEvent, CancellationToken cancellationToken) { await Task.CompletedTask; }
    }

    [Fact]
    public void The_weak_reference_comparer_get_hash_code_should_return_an_integer_value_of_the_weak_reference_if_the_delegate_is_null()
    {
        var theComparer         = new EventAggregator.WeakReferenceDelegateComparer();
        var weakRefHandlerOne   = new WeakReference<Delegate>(null!);

        theComparer.GetHashCode(weakRefHandlerOne).Should().BeGreaterThan(0);
    }    
}
