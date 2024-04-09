using Conditionals.Core.Areas.Conditions;
using Conditionals.Core.Areas.Events;
using Conditionals.Core.Common.Exceptions;
using Conditionals.Core.Common.Models;
using Conditionals.Core.Common.Seeds;

namespace Conditionals.Core.Areas.Rules;

/*
   * I added this interface mainly for the xml comments as to not obstruct the code in the Rule class.
   * Currently the xml comments is its only purpose
*/

/// <summary>
/// A rule is a container for condition sets, each containing a condition tree to be evaluated.
/// Rules use short-circuit OrElse logic between each condition set.
/// </summary>
/// <typeparam name="T">The type of value that will be returned by <see cref="RuleResult{T}" />. The rule stores the failure value, the success value is assigned from the <see cref="ConditionSet{T}" /></typeparam>
public interface IRule<T>
{
    /// <summary>
    /// Gets a readonly list of the condition sets.
    /// </summary>
    IReadOnlyList<ConditionSet<T>> ConditionSets { get; }

    /// <summary>
    /// Gets the cultureID assigned to the rule.
    /// </summary>
    string CultureID                { get; }

    /// <summary>
    /// Gets the failure value. This value is assigned to an instance of the <see cref="RuleResult{T}" /> class.
    /// </summary>
    T FailureValue                  { get; }

    /// <summary>
    /// Gets or sets the rule as being disabled.
    /// </summary>
    bool IsDisabled                 { get; set; }

    /// <summary>
    /// Gets the <see cref="EventDetails" /> if associated with the rule.
    /// </summary>
    EventDetails? RuleEventDetails  { get; }

    /// <summary>
    /// Gets the name of the rule.
    /// </summary>
    string RuleName                 { get; }

    /// <summary>
    /// Gets the specific TenantID assigned to the rule. 
    /// The system default is All_Tenants, meaning the rule is valid for all tenants within a multitenant application..
    /// </summary>
    string TenantID                 { get; }



    /// <summary>
    /// Evaluates the condition tree held in each condition set. 
    /// The rule uses short-circuiting OrElse logic between each set meaning, that as soon as one set evaluates to true the processing is stopped and the result returned.
    /// </summary>
    /// <param name="evaluatorResolver">An implementation of the ConditionEvaluatorResolver delegate that returns a condition evaluator by name and context type <see cref="ConditionEvaluatorResolver" />.</param>
    /// <param name="conditionData">The data for all conditions see the <see cref="ConditionData" /> class.</param>
    /// <param name="eventPublisher">Optional, an implementation of the <see cref="EventPublisher" /> delegate for publishing events, see the <see cref="EventPublisher" /> class.</param>
    /// <param name="precedencePrinter">Optional, an implementation of the <see cref="IConditionPrecedencePrinter"/> interface
    /// used to create a string showing the order of precedence for the condition tree, within a condition set, applied to the <see cref="ConditionSetResult{T}.EvaluationPrecedence" /> property. 
    /// </param>
    /// <param name="cancellationToken">The cancellation token used to signify any cancellation requests.</param>
    /// <returns>An asynchronous operation that returns an instance of the <see cref="RuleResult{T}" /> class.</returns>
    Task<RuleResult<T>> Evaluate(ConditionEvaluatorResolver evaluatorResolver, ConditionData conditionData, EventPublisher? eventPublisher = null, IConditionPrecedencePrinter? precedencePrinter = null, CancellationToken cancellationToken = default);
    
    /// <summary>
    /// Adds another instance of the <see cref="ConditionSet{T}" /> class to list of condition sets. 
    /// Only when a condition set evaluates to false is the next (Or) condition set evaluated. 
    /// </summary>
    /// <param name="conditionSet">The instance of the <see cref="ConditionSet{T}"/> class to be added to the rules list of <see cref="Rule{T}.ConditionSets"/>.</param>
    /// <returns>An instance of the <see cref="Rule{T}"/> class to enable chaining.</returns>
    Rule<T> OrConditionSet(ConditionSet<T> conditionSet);

    /// <summary>
    /// Serializes the rule to a Json formatted string that can be written to a file or stored in a database, for example.
    /// </summary>
    /// <param name="writeIndented">When true will pretty print the JSON.</param>
    /// <param name="useEscaped">When true will escape certain characters such as &lt; using UTF8 encoding.</param>
    /// <returns>A rule serialized to JSON.</returns>
    string ToJsonString(bool writeIndented = false, bool useEscaped = true);
}