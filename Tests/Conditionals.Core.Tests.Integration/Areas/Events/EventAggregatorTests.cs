using Autofac;
using Conditionals.Core.Areas.Engine;
using Conditionals.Core.Areas.Events;
using Conditionals.Core.Common.Seeds;
using Conditionals.Core.Tests.SharedDataAndFixtures.Data;
using Conditionals.Core.Tests.SharedDataAndFixtures.Events;
using FluentAssertions;
using FluentAssertions.Execution;
using Xunit;

namespace Conditionals.Core.Tests.Integration.Areas.Events;

public class EventAggregatorTests
{
    private readonly IEventAggregator _eventAggregator;

    public int MyHandlerCallCount { get; set; } = 0;
    public int ParentClassMyHandlerCallCount { get; set; } = 0;

    public bool EventHandlerCancelled { get; set; } = false;

    public EventAggregatorTests()
        
        => _eventAggregator = ConfigureAutofac().Resolve<IEventAggregator>();

    private IContainer ConfigureAutofac()
    {
        var builder = new ContainerBuilder();

        builder.RegisterType<MyHandler>().As<IEventHandler<ConditionEventCustomer>>();
        builder.RegisterInstance(this).AsSelf();
        builder.Register<IEventAggregator>(c =>
        {
            var context = c.Resolve<IComponentContext>();
        return new EventAggregator(type => context.Resolve(type));
        }).SingleInstance();
        
        return builder.Build();
    }
    public class MyHandler(EventAggregatorTests parentClass) : IEventHandler<ConditionEventCustomer>
    {
        private readonly EventAggregatorTests _parentClass = parentClass;

        public async Task Handle(ConditionEventCustomer theEvent, CancellationToken cancellationToken)
        {
            if (cancellationToken.IsCancellationRequested)
            {
                _parentClass.EventHandlerCancelled = true;
                return;
            }
            _parentClass.MyHandlerCallCount++;
            await Task.CompletedTask;
        }
    }


    [Fact]
    public async Task DI_registered_event_handlers_should_get_called()
    {
        var conditionResultEvent = new ConditionEventCustomer("SomeSender", true, StaticData.CustomerOneAsJsonString(), "TenantID", [], null);

        MyHandlerCallCount = 0;
        _eventAggregator.Publish(conditionResultEvent, CancellationToken.None);
        /*
            * publish is fire and forget so need a delay for the handler to be created before checking, set higher than needed 
         */
        await Task.Delay(100);
        this.MyHandlerCallCount.Should().Be(1);
    }

    [Fact]
    public void Trying_to_get_a_registered_event_handler_from_a_bad_di_configuration_resolver_should_not_throw_an_exception()
    {
        var ruleEven = new RuleEventInt("SomeSender", true,20,0,"TenantID", []);

        var eventAggregator = new EventAggregator(t => null!);

        FluentActions.Invoking(() => eventAggregator.Publish(ruleEven, CancellationToken.None)).Should().NotThrow();
    }

    [Fact]
    public async Task Should_be_able_to_pass_a_cancellation_token_to_the_event()
    {
        CancellationTokenSource tokenSource = new CancellationTokenSource();

        var token = tokenSource.Token;
        var conditionResultEvent = new ConditionEventCustomer("SomeSender", true, StaticData.CustomerOneAsJsonString(), "TenantID", [], null);
        EventHandlerCancelled    = false;

        _eventAggregator.Publish(conditionResultEvent, token);
        tokenSource.Cancel();
        /*
            * publish is fire and forget so need a delay for the handler to be created before checking, set higher than needed 
         */
        await Task.Delay(300);
        this.EventHandlerCancelled.Should().BeTrue();
    }

    [Fact]
    public async Task Trying_to_add_a_duplicate_handler_should_just_return_a_subscription_without_adding_a_second_handler()
    {
        var ruleEvent = new DuplicateRuleEventInt("TheRule", false, 42, 10, GlobalStrings.Default_TenantID, []);
        var handlerOneSub = _eventAggregator.Subscribe<DuplicateRuleEventInt>(handlerOne);
        var duplicateSub = _eventAggregator.Subscribe<DuplicateRuleEventInt>(handlerOne);

        this.MyHandlerCallCount = 0;
        _eventAggregator.Publish(ruleEvent, CancellationToken.None);

        await Task.Delay(100);

        var firstCount = this.MyHandlerCallCount;

        handlerOneSub.Dispose();

        _eventAggregator.Publish(ruleEvent, CancellationToken.None);
        var secondCount = this.MyHandlerCallCount;

        duplicateSub.Dispose();

        using (new AssertionScope())
        {
            firstCount.Should().Be(1);
            secondCount.Should().Be(firstCount);
        }

        async Task handlerOne(DuplicateRuleEventInt ruleEvent, CancellationToken cancellationToken)
        {
            this.MyHandlerCallCount++;

            await Task.CompletedTask;
        }

    }

    [Fact]
    public void Tyring_to_register_a_null_handler_should_not_cause_an_issue()
    {
        ConditionEngine conditionEngine = new();

        var ruleEvent           = new RuleEventInt("Test", true, 42, 0, GlobalStrings.Default_TenantID, []);
        var eventSubscription   = _eventAggregator.Subscribe<RuleEventInt>(null!);

        using(new AssertionScope())
        {
            FluentActions.Invoking(() => _eventAggregator.Publish(ruleEvent,CancellationToken.None)).Should().NotThrow();
            FluentActions.Invoking(() => eventSubscription.Dispose()).Should().NotThrow();

        }
    }



























}
