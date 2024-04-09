using Conditionals.Core.Areas.Evaluators;
using Conditionals.Core.Areas.Events;
using Conditionals.Core.Areas.Rules;
using Conditionals.Core.Common.Models;
using Conditionals.Core.Common.Seeds;
using Conditionals.Core.Common.Exceptions;

namespace Conditionals.Core.Areas.Engine
{
    /*
        * I added this interface mainly for the xml comments as to not obstruct the code in the ConditionEngine class.
        * Currently the xml comments is its only purpose
    */

    /// <summary>
    /// A condition engine is responsible for storing and fetching rules from an internal cache, rule evaluation 
    /// and for creating and or fetching condition evaluators. It also provides methods for both publishing and subscribing to rule and condition events. 
    /// </summary>
    public interface IConditionEngine
    {
        /// <summary>
        /// Adds a rule to an internal cache using the combination of rule name, tenantID and cultureID as the cache key. If the rule does not exist in the 
        /// cache it is added otherwise the existing entry is replaced.
        /// </summary>
        /// <param name="rule">The rule to be added to or updated in the cache.</param>
        /// <exception cref="ArgumentException"></exception>
        void AddOrUpdateRule<T>(Rule<T> rule);

        /// <summary>
        /// Checks whether or not a rule is in the condition engines cache. 
        /// The cache key used is the combination of rule name, tenantID and cultureID.
        /// </summary>
        /// <param name="ruleName">The name of the rule.</param>
        /// <param name="tenantID">Optional, a specific tenantID value otherwise the default value of All_Tenants will be applied,</param>
        /// <param name="cultureID">Optional, a specific cultureID value otherwise the default value en-GB will be applied.</param>
        /// <returns>True if the rule is in the condition engines cache otherwise false.</returns>
        bool ContainsRule(string ruleName, string tenantID = "All_Tenants", string cultureID = "en-GB");

        /// <summary>
        /// Removes the rule from the conditions engines cache.
        /// </summary>
        /// <param name="ruleName">The name of the rule.</param>
        /// <param name="tenantID">Optional, a specific tenantID value otherwise the default value of All_Tenants will be applied,</param>
        /// <param name="cultureID">Optional, a specific cultureID value otherwise the default value en-GB will be applied.</param>
        void RemoveRule(string ruleName, string tenantID = "All_Tenants", string cultureID = "en-GB");

        /// <summary>
        /// Tries to get the rule from the condition engines cache.
        /// </summary>
        /// <param name="ruleName">The name of the rule.</param>
        /// <param name="rule">The rule to be returned in the out parameter if available. <see cref="Rule{T}" /></param>
        /// <param name="tenantID">Optional, a specific tenantID value otherwise the default value of All_Tenants will be applied,</param>
        /// <param name="cultureID">Optional, a specific cultureID value otherwise the default value en-GB will be applied.</param>
        /// <returns>True if the condition engine contains the rule otherwise false.</returns>
        bool TryGetRule<T>(string ruleName, out Rule<T>? rule, string tenantID = "All_Tenants", string cultureID = "en-GB");


        /// <summary>
        /// Gets the rule from cache and then starts the evaluation process using the provided condition data contexts.
        /// </summary>
        /// <param name="ruleName">The name of the rule to evaluate.</param>
        /// <param name="contexts">Contains the array of DataContexts for all conditions within a rule.</param>
        /// <param name="precedencePrinter">Optional, concrete implementation of an IConditionPrecedencePrinter that provides
        /// the ability to create a string representation of the order of precedence of the conditions within a condition set.
        /// If used the ConditionSetResult property EvaluationPrecedence will hold the output.
        /// </param>
        /// <exception cref="RuleNotFoundException">Thrown when the <paramref name="ruleName"/> is not found in the cache.</exception>
        /// <exception cref="MissingAllConditionDataException">Thrown when the <paramref name="contexts"/> is null or an empty array.</exception>
        /// <returns>Asynchronous operation returning a <see cref="RuleResult{T}" /> containing all of the information about the evaluation path, failure messages, exceptions, timings and the overall 
        /// outcome and return value .
        /// </returns>
        Task<RuleResult<T>> EvaluateRule<T>(string ruleName, ConditionData contexts, IConditionPrecedencePrinter? precedencePrinter = null);


        /// <summary>
        /// Gets the rule from cache and then starts the evaluation process using the provided condition data contexts.
        /// </summary>
        /// <param name="ruleName">The name of the rule to evaluate.</param>
        /// <param name="contexts">Contains the array of DataContexts for all conditions within a rule see <see cref="ConditionData" />.</param>
        /// <param name="cancellationToken">The cancellation token used to signify any cancellation requests.</param>
        /// <param name="precedencePrinter">Optional, implementation of an <see cref="IConditionPrecedencePrinter" /> that provides
        /// the ability to create a string representation of the order of precedence of the conditions within a condition set.
        /// </param>
        /// <exception cref="RuleNotFoundException">Thrown when the <paramref name="ruleName"/> is not found in the cache.</exception>
        /// <exception cref="MissingAllConditionDataException">Thrown when the <paramref name="contexts"/> is null or an empty array.</exception>
        /// <returns>An asynchronous operation that returns an instance of the <see cref="RuleResult{T}" /> class containing all of the information 
        /// about the evaluation path, failure messages, exceptions, timings and the overall 
        /// outcome and return value.
        /// </returns>
        Task<RuleResult<T>> EvaluateRule<T>(string ruleName, ConditionData contexts, CancellationToken cancellationToken, IConditionPrecedencePrinter? precedencePrinter = null);

        /// <summary>
        /// Gets the rule from cache and then starts the evaluation process using the provided condition data contexts.
        /// </summary>
        /// <param name="ruleName">The name of the rule to evaluate.</param>
        /// <param name="contexts">Contains the array of DataContexts for all conditions within a rule see <see cref="ConditionData" />.</param>
        /// <param name="cancellationToken">The cancellation token used to signify any cancellation requests.</param>
        /// <param name="precedencePrinter">Optional, concrete implementation of an IConditionPrecedencePrinter that provides
        /// the ability to create a string representation of the order of precedence of the conditions within a condition set.
        /// If used the ConditionSetResult property EvaluationPrecedence will hold the output.
        /// </param>
        /// <param name="tenantID">Optional, value used to identify the Tenant in multi-tenant applications; defaults to All_Tenants.</param>
        /// <param name="cultureID">Optional, value used to specify / filter cached rules that may have conditions with failure messages in differing languages</param>
        /// <exception cref="RuleNotFoundException">Thrown when the <paramref name="ruleName"/> is not found in the cache.</exception>
        /// <exception cref="MissingAllConditionDataException">Thrown when the <paramref name="contexts"/> is null or an empty array.</exception>
        /// <returns>An asynchronous operation that returns an instance of the <see cref="RuleResult{T}" /> class containing all of the information 
        /// about the evaluation path, failure messages, exceptions, timings and the overall 
        /// outcome and return value.
        /// </returns>
        Task<RuleResult<T>> EvaluateRule<T>(string ruleName, ConditionData contexts, CancellationToken cancellationToken, IConditionPrecedencePrinter? precedencePrinter = null, string tenantID = "All_Tenants", string cultureID = "en-GB");

        /// <summary>
        /// Method used to return an implementation of a <see cref="IConditionEvaluator" />
        /// </summary>
        /// <param name="evaluatorName">The name of the evaluator to be returned.</param>
        /// <param name="contextType">The data type used by the evaluator.</param>
        /// <returns>An implementation of the <see cref="IConditionEvaluator" /> interface.</returns>
        IConditionEvaluator EvaluatorResolver(string evaluatorName, Type contextType);

        /// <summary>
        /// Method that returns an event publisher used to raise events.
        /// </summary>
        /// <typeparam name="TEvent">The type of event. This will be an implementation of either the <see cref="ConditionEventBase{T}" /> or <see cref="RuleEventBase{T}" /> abstract classes.</typeparam>
        /// <param name="eventToPublish">The event to be published.</param>
        /// <param name="cancellationToken">The cancellation token used to signify any cancellation requests.</param>
        void EventPublisher<TEvent>(TEvent eventToPublish, CancellationToken cancellationToken) where TEvent : IEvent;

        /// <summary>
        /// Ingests a rule in json format into the engine.
        /// The rules value type is obtained from the json.
        /// </summary>
        /// <param name="ruleJson">The string representing the rule in the Json format.</param>
        void IngestRuleFromJson(string ruleJson);

        /// <summary>
        /// Ingests a rule in json format into the engine.
        /// </summary>
        /// <typeparam name="T">The return data type of the rule.</typeparam>
        /// <param name="ruleJson">The string representing the rule in the Json format.</param>
        void IngestRuleFromJson<T>(string ruleJson);


        /// <summary>
        /// Registers a custom evaluator to be used by the condition engine.
        /// </summary>
        /// <param name="evaluatorName">The name of the evaluator to be registered.</param>
        /// <param name="evaluatorType">The <see cref="Type" /> of evaluator.</param>
        void RegisterCustomEvaluator(string evaluatorName, Type evaluatorType);

        /// <summary>
        /// Method used to inform the condition engine of a custom evaluator registered for dependency injection.
        /// Please note that the evaluator also has to be registered with the chosen IOC container.
        /// </summary>
        /// <param name="evaluatorName">The name of the evaluator to be registered.</param>
        /// <param name="evaluatorType">The <see cref="Type" /> of evaluator.</param>
        void RegisterCustomEvaluatorForDependencyInjection(string evaluatorName, Type evaluatorType);
                
        /// <summary>
        /// Associates a local event handler to an event that you would like to receive. Note, event handlers must be implement the <see cref="HandleEvent{TEvent}" /> delegate.>
        /// </summary>
        /// <typeparam name="TEvent">The type of event. This will be an implementation of either the <see cref="ConditionEventBase{T}" /> or <see cref="RuleEventBase{T}" /> abstract class.</typeparam>
        /// <param name="eventHandler">A local handler that implements the <see cref="HandleEvent{TEvent} "/> delegate.</param>
        /// <returns>An instance of the <see cref="EventSubscription"/> class.</returns>
        EventSubscription SubscribeToEvent<TEvent>(HandleEvent<TEvent> eventHandler) where TEvent : IEvent;

    }
}