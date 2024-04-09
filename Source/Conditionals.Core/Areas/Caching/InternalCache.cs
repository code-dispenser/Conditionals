using Conditionals.Core.Common.Exceptions;
using Conditionals.Core.Common.Models;
using Conditionals.Core.Common.Seeds;
using System.Collections.Concurrent;

namespace Conditionals.Core.Areas.Caching;


internal class InternalCache
{

    private readonly ConcurrentDictionary<CacheKey, SemaphoreSlim> _lockManager = new();
    private readonly ConcurrentDictionary<CacheKey, CacheItem> _cachedItems = new();

    public bool ContainsItem(string itemKey, string tenantID = GlobalStrings.Default_TenantID, string cultureID = GlobalStrings.Default_CultureID)

        => _cachedItems.ContainsKey(new CacheKey(itemKey, tenantID, cultureID));


    public bool TryGetItem<T>(string itemKey, out T? cacheItem, string tenantID = GlobalStrings.Default_TenantID, string cultureID = GlobalStrings.Default_CultureID)
    {
        var cacheKey = new CacheKey(itemKey, tenantID, cultureID);

        if (true == _cachedItems.TryGetValue(cacheKey, out var itemInCache))
        {
            cacheItem = (T)itemInCache.Value;
            return true;
        }

        cacheItem = default;
        return false;
    }

    public T GetOrAddItem<T>(string itemKey, Type typeParam, Func<Type, T> createItemForCache, string tenantID = GlobalStrings.Default_TenantID, string cultureID = GlobalStrings.Default_CultureID)
    {
        var cacheKey = new CacheKey(itemKey, tenantID, cultureID);
        var semaphoreSlim = _lockManager.GetOrAdd(cacheKey, new SemaphoreSlim(1, 1));
        var lockAcquired = false;//incase of exception trying to get a lock we don't want to do a release.

        try
        {
            semaphoreSlim.Wait(100);

            lockAcquired = true;

            CacheItem buildCacheItem(CacheKey key) => new(createItemForCache(typeParam)!);

            CacheItem cachedItem = _cachedItems.GetOrAdd(cacheKey, buildCacheItem);

            return (T)cachedItem.Value;
        }
        finally
        {
            if (true == lockAcquired) semaphoreSlim.Release();
        }

    }
    public T GetOrAddItem<T>(string itemKey, Func<T> createItemForCache, string tenantID = GlobalStrings.Default_TenantID, string cultureID = GlobalStrings.Default_CultureID)
    {
        var cacheKey = new CacheKey(itemKey, tenantID, cultureID);
        var semaphoreSlim = _lockManager.GetOrAdd(cacheKey, new SemaphoreSlim(1, 1));
        var lockAcquired = false;

        semaphoreSlim.Wait();
        lockAcquired = true;

        try
        {
            CacheItem buildCacheItem(CacheKey key) => new(createItemForCache()!);

            CacheItem cachedItem = _cachedItems.GetOrAdd(cacheKey, buildCacheItem);

            return (T)cachedItem.Value;

        }
        finally
        {
            if (true == lockAcquired) semaphoreSlim.Release();
        }

    }

    public void AddOrUpdateItem<T>(string itemKey, T itemToCache, string tenantID = GlobalStrings.Default_TenantID, string cultureID = GlobalStrings.Default_CultureID) where T : notnull
    {
        var cacheKey = new CacheKey(itemKey, tenantID, cultureID);
        var semaphoreSlim = _lockManager.GetOrAdd(cacheKey, new SemaphoreSlim(1, 1));
        var lockAcquired = false;

        semaphoreSlim.Wait();
        lockAcquired = true;

        try
        {
            static CacheItem updateItem(CacheKey cacheKey, CacheItem existingItem, T T)
            {
                try
                {
                    if (existingItem.Value is IDisposable doDispose) doDispose.Dispose();
                }
                catch { } //decided just to squash this error as its not likely to occur. It would have to be a custom evaluator being updated that throws an error in its dispose method assuming it has one;

                return new CacheItem(T);
            }

            _ = _cachedItems.AddOrUpdate(cacheKey, (cacheKey, T) => new CacheItem(T), updateItem, itemToCache);
        }
        finally
        {
            if (true == lockAcquired) semaphoreSlim.Release();
        }
    }

    public void RemoveItem(string itemKey, string tenantID = GlobalStrings.Default_TenantID, string cultureID = GlobalStrings.Default_CultureID)
    {
        var cacheKey = new CacheKey(itemKey, tenantID, cultureID);

        if (true == _cachedItems.TryRemove(cacheKey, out var cachedItem))
        {
            try
            {
                if (cachedItem.Value is IDisposable doDispose) doDispose.Dispose();
            }
            catch (Exception ex) { throw new DisposingRemovedItemException(String.Format(GlobalStrings.Disposing_Removed_Item_Exception_Message, cacheKey.ItemName), ex); }
        }
    }
}
