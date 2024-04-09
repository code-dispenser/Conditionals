using Conditionals.Core.Areas.Caching;
using Conditionals.Core.Areas.Evaluators;
using Conditionals.Core.Areas.Events;
using Conditionals.Core.Areas.Rules;
using Conditionals.Core.Common.Exceptions;
using Conditionals.Core.Common.FormatConverters;
using Conditionals.Core.Common.Models;
using Conditionals.Core.Common.Seeds;
using Conditionals.Core.Common.Validation;
using System.Reflection;
using System.Text.Json;

namespace Conditionals.Core.Areas.Engine;

/// <inheritdoc cref="IConditionEngine" />
public class ConditionEngine : IConditionEngine
{
    private readonly InternalCache _cache = new();
    private readonly EventAggregator _eventAggregator;
    private readonly Func<Type, dynamic>? _customTypeResolver;
    private readonly bool _allowDependencyInjectionEvaluators = false;

    /// <summary>
    /// Initialises a new instance of the <see cref="ConditionEngine" /> class.
    /// </summary>
    /// <param name="customTypeResolver">A func that acts as a call back to fetch instances of the requested type from an IOC container.</param>
    public ConditionEngine(Func<Type, object> customTypeResolver)
    {
        _customTypeResolver                  = customTypeResolver;
        _allowDependencyInjectionEvaluators  = true;
        _eventAggregator                     = new EventAggregator(_customTypeResolver);
    }

    /// <summary>
    /// Initialises a new instance of the <see cref="ConditionEngine" /> class.
    /// </summary>
    public ConditionEngine()

        => _eventAggregator = new();


    #region Caching

    ///<inheritdoc />
    public void AddOrUpdateRule<T>(Rule<T> rule)

        => this.AddOrUpdateRule(rule, false);

    ///<inheritdoc />
    ////// <param name="fromJson">Specifies whether the rule is being added or updated from JSON.</param>
    private void AddOrUpdateRule<T>(Rule<T> rule, bool fromJson)
    {
        Check.ThrowIfNullEmptyOrWhitespace(rule);

        var cacheKeyPart = String.Join("_", GlobalStrings.CacheKey_Part_Rule, rule.RuleName);

        var ruleToCache = fromJson == true ? rule : rule.DeepCloneRule();//clone it to stop any chance of the cache getting changed.

        _cache.AddOrUpdateItem(cacheKeyPart, ruleToCache, ruleToCache.TenantID, ruleToCache.CultureID);
    }

    ///<inheritdoc />
    public bool TryGetRule<T>(string ruleName, out Rule<T>? rule, string tenantID = GlobalStrings.Default_TenantID, string cultureID = GlobalStrings.Default_CultureID)
    {
        var cacheKeyPart = String.Join("_", GlobalStrings.CacheKey_Part_Rule, ruleName);

        if (true == _cache.TryGetItem<Rule<T>>(cacheKeyPart, out var cacheItem, tenantID, cultureID))
        {
            rule = cacheItem!.DeepCloneRule(); ;//clone it to stop any chance of the cache getting changed.
            return true;
        }

        rule = default;
        return false;
    }
    ///<inheritdoc />
    public bool ContainsRule(string ruleName, string tenantID = GlobalStrings.Default_TenantID, string cultureID = GlobalStrings.Default_CultureID)

        => _cache.ContainsItem(String.Join("_", GlobalStrings.CacheKey_Part_Rule, ruleName), tenantID, cultureID);

    ///<inheritdoc />
    public void RemoveRule(string ruleName, string tenantID = GlobalStrings.Default_TenantID, string cultureID = GlobalStrings.Default_CultureID)

        => _cache.RemoveItem(String.Join("_", GlobalStrings.CacheKey_Part_Rule, ruleName), tenantID, cultureID);

    #endregion


    #region Engine Rules

    ///<inheritdoc />
    public async Task<RuleResult<T>> EvaluateRule<T>(string ruleName, ConditionData contexts, IConditionPrecedencePrinter? precedencePrinter = null)

        => await EvaluateRule<T>(ruleName, contexts, CancellationToken.None, precedencePrinter).ConfigureAwait(false);

    ///<inheritdoc />
    public async Task<RuleResult<T>> EvaluateRule<T>(string ruleName, ConditionData contexts, CancellationToken cancellationToken, IConditionPrecedencePrinter? precedencePrinter = null)

        => await EvaluateRule<T>(ruleName, contexts, cancellationToken, precedencePrinter, GlobalStrings.Default_TenantID, GlobalStrings.Default_CultureID).ConfigureAwait(false);

    ///<inheritdoc />
    public async Task<RuleResult<T>> EvaluateRule<T>(string ruleName, ConditionData contexts, CancellationToken cancellationToken, IConditionPrecedencePrinter? precedencePrinter = null, string tenantID = GlobalStrings.Default_TenantID, string cultureID = GlobalStrings.Default_CultureID)
    {
        var cacheKeyPart = String.Join("_", GlobalStrings.CacheKey_Part_Rule, ruleName);

        if (false == _cache.TryGetItem<Rule<T>>(cacheKeyPart, out var rule, tenantID, cultureID)) throw new RuleNotFoundException(String.Format(GlobalStrings.No_Rule_In_Cache_Exception_Message, ruleName));

        return await rule!.Evaluate(this.EvaluatorResolver, contexts, this.EventPublisher, precedencePrinter, cancellationToken).ConfigureAwait(false);
    }

    ///<inheritdoc />
    public void IngestRuleFromJson(string ruleJson)
    {
        Type? ruleType = default;

        using (var jsonDocument = JsonDocument.Parse(ruleJson))
        {
            var valueType = jsonDocument.RootElement.TryGetProperty("ValueTypeName", out JsonElement typeNameElement) ? typeNameElement.GetString() ?? String.Empty : String.Empty;
            var ruleName = jsonDocument.RootElement.TryGetProperty("RuleName", out var name) ? name.GetString() ?? GlobalStrings.Rule_Name_Property_Is_Missing_Or_Null_Message : GlobalStrings.Rule_Name_Property_Is_Missing_Or_Null_Message;

            ruleType = valueType.ToLower() switch
            {
                "none" => typeof(None),
                _ => Type.GetType($"System.{valueType}", false, true)
            };

            if (ruleType == null) throw new InvalidSystemDataTypeException(String.Format(GlobalStrings.Invalid_System_DataType_Exception_Message, ruleName));
        }

        MethodInfo methodInfo = typeof(JsonRuleConverter).GetMethod(nameof(RuleFromJson), BindingFlags.NonPublic | BindingFlags.Static)!.MakeGenericMethod([ruleType]);

        try
        {
            dynamic rule = methodInfo.Invoke(null, [ruleJson])!;
            this.AddOrUpdateRule(rule, true);
        }
        catch (Exception exception) { throw exception.InnerException!; }
        //Due to the invoke the exception will be of the type TargetInvocationException, as such we need to throw the inner exception so the caller gets the correct type.



    }
    ///<inheritdoc />
    public void IngestRuleFromJson<T>(string ruleJson)
    {
        var rule = RuleFromJson<T>(ruleJson);
        this.AddOrUpdateRule(rule, true);
    }

    /// <summary>
    /// Creates an instance of <see cref="Rule{T}" /> class from a Json formatted string.
    /// </summary>
    /// <typeparam name="T">The data type of the rules success and failure value properties.</typeparam>
    /// <param name="ruleJson">The Json formatted string to be converted.</param>
    /// <returns>An instance of the <see cref="Rule{T}" /> class.</returns>
    public static Rule<T> RuleFromJson<T>(string ruleJson)

        => JsonRuleConverter.RuleFromJson<T>(ruleJson);

    ///<inheritdoc />
    public void RegisterCustomEvaluator(string evaluatorName, Type evaluatorType)

        => _cache.AddOrUpdateItem(String.Join("_", GlobalStrings.CacheKey_Part_Evaluator_Type, evaluatorName), evaluatorType);
    /*
        * Made two separate methods to negate mistakes with one method and a flag 
    */
    ///<inheritdoc />
    public void RegisterCustomEvaluatorForDependencyInjection(string evaluatorName, Type evaluatorType)

        => _cache.AddOrUpdateItem(String.Join("_", GlobalStrings.CacheKey_Part_Evaluator_Type_DI, evaluatorName), evaluatorType);

    private Type? CheckGetEvaluatorInDI(string evaluatorName)

        => _cache.TryGetItem(String.Join("_", GlobalStrings.CacheKey_Part_Evaluator_Type_DI, evaluatorName), out Type? evaluatorType) ? evaluatorType : null;

    ///<inheritdoc />
    public IConditionEvaluator EvaluatorResolver(string evaluatorName, Type contextType)
    {
        if (true == _allowDependencyInjectionEvaluators)//skip if engine not initialised for DI
        {
            var evaluatorType = CheckGetEvaluatorInDI(evaluatorName);
            if (evaluatorType != null)
            {
                var customType = evaluatorType.IsGenericType ? evaluatorType.MakeGenericType(contextType) : evaluatorType;

                return _customTypeResolver!(customType);
            }
        }

        var cacheKey = String.Join("_", evaluatorName, contextType.FullName);

        Func<Type, IConditionEvaluator> makeEvaluator = (theContextType) =>
        {
            Type evaluatorType;

            switch (evaluatorName)
            {
                case GlobalStrings.Predicate_Condition_Evaluator: evaluatorType = typeof(PredicateConditionEvaluator<>).MakeGenericType(theContextType); break;
                case GlobalStrings.Regex_Condition_Evaluator: evaluatorType = typeof(RegexConditionEvaluator<>).MakeGenericType(theContextType); break;
                default:

                    var evaluatorTypeKey = String.Join("_", GlobalStrings.CacheKey_Part_Evaluator_Type, evaluatorName);

                    if (true == _cache.TryGetItem<Type>(evaluatorTypeKey, out var customEvaluatorType))
                    {
                        evaluatorType = customEvaluatorType!.IsGenericType && customEvaluatorType.ContainsGenericParameters == true ? customEvaluatorType.MakeGenericType(theContextType) : customEvaluatorType;
                    }
                    else
                    {
                        throw new MissingConditionEvaluatorException(String.Format(GlobalStrings.Missing_Condition_Evaluator_Exception_Message, evaluatorName));
                    }

                    break;
            }

            return (IConditionEvaluator)Activator.CreateInstance(evaluatorType)!;

        };

        return _cache.GetOrAddItem(cacheKey, contextType, makeEvaluator);
    }
    #endregion



    #region Eventing
    ///<inheritdoc />
    public EventSubscription SubscribeToEvent<TEvent>(HandleEvent<TEvent> eventHandler) where TEvent : IEvent

        => _eventAggregator.Subscribe<TEvent>(eventHandler);

    ///<inheritdoc />
    public void EventPublisher<TEvent>(TEvent eventToPublish, CancellationToken cancellationToken) where TEvent : IEvent
    {
        /*
             * If this method is called via the delegate void EventPublisher(Event eventToPublish), in 99% cases the
             * TEvent is always IEvent so we need to call the publish method via reflection using its actual type that is registered/subscribed to
             * Suggestions welcome on how to solve this type in a better way?
         */

        if (false == typeof(TEvent).Equals(eventToPublish.GetType()))
        {
            Type actualType = eventToPublish.GetType();

            MethodInfo openMethod = _eventAggregator.GetType().GetMethod("Publish")!;
            MethodInfo closedMethod = openMethod.MakeGenericMethod(actualType);

            closedMethod.Invoke(_eventAggregator, [eventToPublish, cancellationToken]);

            return;
        }

        _eventAggregator.Publish<TEvent>(eventToPublish, cancellationToken);

    }

    #endregion
}
