using Conditionals.Core.Areas.Caching;
using Conditionals.Core.Common.Exceptions;
using Conditionals.Core.Tests.SharedDataAndFixtures.Data;
using Conditionals.Core.Tests.SharedDataAndFixtures.Evaluators;
using Conditionals.Core.Tests.SharedDataAndFixtures.Models;
using FluentAssertions;
using FluentAssertions.Execution;
using Xunit;

namespace Conditionals.Core.Tests.Unit.Areas.Caching;


public class InternalCacheTests
{
    [Fact]
    public void Contains_key_should_return_true_if_the_keyed_item_is_in_cache()
    {
        var cache       = new InternalCache();
        var itemToCache = StaticData.CustomerOne();

        cache.GetOrAddItem<Customer>("CustomerOne", () => itemToCache);

        cache.ContainsItem("CustomerOne").Should().BeTrue();    


    }
    [Fact]
    public void Should_be_able_add_an_item_to_cache_and_retrieve_it_with_the_same_key_used_to_store_it()
    {
        var cache = new InternalCache();
        var customer = StaticData.CustomerOne();
        var cacheKey = String.Join("_", "Customer", customer.CustomerName);

        cache.AddOrUpdateItem(cacheKey, customer);

        _ = cache.TryGetItem<Customer>(cacheKey, out var theRetrievedCustomer);

        theRetrievedCustomer.Should().NotBeNull().And.Match<Customer>(c => c.CustomerName == customer.CustomerName && c.CustomerNo == customer.CustomerNo
                                                                        && c.MemberYears == customer.MemberYears && c.Address == customer.Address);
    }

    [Fact]
    public void Should_be_able_add_an_item_to_cache_and_have_the_item_at_the_key_updated_if_adding_again_via_an_existing_key()
    {
        var cache = new InternalCache();
        var customer = StaticData.CustomerOne();
        var customerTwo = customer with { CustomerName = "New Name" };
        var cacheKey = String.Join("_", "Customer", customer.CustomerName);

        cache.AddOrUpdateItem(cacheKey, customer);
        cache.AddOrUpdateItem(cacheKey, customerTwo);

        _ = cache.TryGetItem<Customer>(cacheKey, out var theRetrievedCustomer);

        theRetrievedCustomer.Should().NotBeNull().And.Match<Customer>(c => c.CustomerName == "New Name" && c.CustomerNo == customer.CustomerNo
                                                                        && c.MemberYears == customer.MemberYears && c.Address == customer.Address);
    }

    [Fact]
    public void Should_be_able_to_remove_an_item_from_cache_if_it_exists()
    {
        var cache = new InternalCache();
        var customer = StaticData.CustomerOne();
        var cacheKey = String.Join("_", "Customer", customer.CustomerName);

        cache.AddOrUpdateItem(cacheKey, customer);

        var theItemIsInCache = cache.TryGetItem<Customer>(cacheKey, out _);

        cache.RemoveItem(cacheKey);

        var theRetrievedCustomer = cache.TryGetItem<Customer>(cacheKey, out var cacheItem) ? cacheItem : null;

        using (new AssertionScope())
        {
            theItemIsInCache.Should().BeTrue();
            theRetrievedCustomer.Should().BeNull();
        }
    }

    [Fact]
    public void Should_add_the_item_if_there_is_no_item_with_the_requested_key_already_in_the_cache()
    {
        var cache = new InternalCache();
        var customer = StaticData.CustomerOne();
        var cacheKey = String.Join("_", "Customer", customer.CustomerName);

        var theItemIsInCache = cache.TryGetItem<Customer>(cacheKey, out _);
        var theRetrievedCustomer = cache.GetOrAddItem<Customer>(cacheKey, () => customer);

        using (new AssertionScope())
        {
            theItemIsInCache.Should().BeFalse();
            theRetrievedCustomer.Should().NotBeNull().And.Match<Customer>(c => c.CustomerName == customer.CustomerName && c.CustomerNo == customer.CustomerNo
                                                                        && c.MemberYears == customer.MemberYears && c.Address == customer.Address);
        }
    }

    [Fact]
    public void Should_squash_exceptions_when_tying_to_dispose_objects_in_the_add_or_update_method()
    {
        var cache = new InternalCache();
        var badEvaluatorOne = new ExceptionInDisposeEvaluator<Customer>();
        var badEvaluatorTwo = new ExceptionInDisposeEvaluator<Supplier>();
        var cacheKey = String.Join("_", "Evaluator", badEvaluatorOne.GetType().FullName);

        cache.AddOrUpdateItem(cacheKey, badEvaluatorOne);
        cache.AddOrUpdateItem(cacheKey, badEvaluatorTwo);

        var theEvaluator = cache.TryGetItem<ExceptionInDisposeEvaluator<Supplier>>(cacheKey, out var evaluator) ? evaluator : null;

        theEvaluator.Should().NotBeNull().And.BeOfType<ExceptionInDisposeEvaluator<Supplier>>();
    }

    [Fact]
    public void Should_throw_disposing_removed_Item_exception_when_tying_to_dispose_objects_in_the_remove_Item_method()
    {
        var cache = new InternalCache();
        var badEvaluatorOne = new ExceptionInDisposeEvaluator<Customer>();
        var cacheKey = String.Join("_", "Evaluator", badEvaluatorOne.GetType().Name);

        var evaluatorIsInCache = cache.GetOrAddItem<ExceptionInDisposeEvaluator<Customer>>(cacheKey, () => badEvaluatorOne);

        using (new AssertionScope())
        {
            evaluatorIsInCache.Should().NotBeNull();

            FluentActions.Invoking(() => cache.RemoveItem(cacheKey)).Should().Throw<DisposingRemovedItemException>();
        }
    }
    [Fact]
    public void the_Should_not_throw_exception_when_tying_to_dispose_objects_in_the_remove_Item_method()
    {
        var cache = new InternalCache();
        var badEvaluatorOne = new ExceptionInDisposeEvaluator<Customer>(supressException:true);
        var cacheKey = String.Join("_", "Evaluator", badEvaluatorOne.GetType().Name);

        var evaluatorIsInCache = cache.GetOrAddItem<ExceptionInDisposeEvaluator<Customer>>(cacheKey, () => badEvaluatorOne);

        using (new AssertionScope())
        {
            evaluatorIsInCache.Should().NotBeNull();

            FluentActions.Invoking(() => cache.RemoveItem(cacheKey)).Should().NotThrow();
        }
    }


    [Fact]
    public void Should_add_the_item_via_the_passed_in_function_if_there_is_no_item_with_the_requested_key_already_in_the_cache()
    {
        var cache = new InternalCache();
        var customer = StaticData.CustomerOne();
        var cacheKey = String.Join("_", "Customer", customer.CustomerName);

        /*
            * The test code is non-sensical but does test the method. The method being tested is needed for the caching and creation of evaluators that
            * is being done dynamically.
        */ 

        Func<Type, Customer> createItem = t => customer;

        var theItemIsInCache = cache.TryGetItem<Customer>(cacheKey, out _);
        var theRetrievedCustomer = cache.GetOrAddItem<Customer>(cacheKey,typeof(Customer),createItem);

        using (new AssertionScope())
        {
            theItemIsInCache.Should().BeFalse();
            theRetrievedCustomer.Should().NotBeNull().And.Match<Customer>(c => c.CustomerName == customer.CustomerName && c.CustomerNo == customer.CustomerNo
                                                                        && c.MemberYears == customer.MemberYears && c.Address == customer.Address);
        }
    }

    [Theory]
    [InlineData(true)]
    [InlineData(false)]
    public void Should_not_throw_exception_trying_to_dispose_of_a_cached_item_that_implements_dispose(bool supressException)
    {
        var cache = new InternalCache();
        var badEvaluatorOne = new ExceptionInDisposeEvaluator<Customer>(supressException);
        var cacheKey = String.Join("_", "Evaluator", badEvaluatorOne.GetType().Name);

        cache.AddOrUpdateItem<ExceptionInDisposeEvaluator<Customer>>(cacheKey, badEvaluatorOne);

        FluentActions.Invoking(() => cache.AddOrUpdateItem<ExceptionInDisposeEvaluator<Customer>>(cacheKey, badEvaluatorOne)).Should().NotThrow();

    }

  
}

