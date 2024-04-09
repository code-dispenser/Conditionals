using Conditionals.Core.Common.Models;
using Conditionals.Core.Common.Seeds;

namespace Conditionals.Core.Areas.Conditions
{
    /*
        * I added this interface mainly for the xml comments as to not obstruct the code in the ConditionSet class.
        * Currently the xml comments is its only purpose
    */
    /// <summary>
    /// A Container that holds an instance of <see cref="BooleanConditionBase" /> class which forms a condition tree for evaluation.
    /// The container also holds a value to be used as a <see cref="RuleResult{T}" /> success value.
    /// </summary>
    public interface IConditionSet<T>
    {
        /// <summary>
        /// Gets the root of the conditions / abstract syntax tree <see cref="BooleanConditionBase" />.
        /// </summary>
        BooleanConditionBase BooleanConditions  { get; }

        /// <summary>
        /// Gets the name of the condition set.
        /// </summary>
        string SetName                          { get; }

        /// <summary>
        /// Gets the value of set.
        /// </summary>
        T SetValue                              { get; }




        /// <summary>
        /// Evaluates the condition tree.
        /// </summary>
        /// <param name="evaluatorResolver">An implementation of the <see cref="ConditionEvaluatorResolver"/> delegate that returns a condition evaluator by name and context type <see cref="ConditionEvaluatorResolver" />.</param>
        /// <param name="conditionData">The data for all conditions <see cref="ConditionData" /></param>
        /// <param name="eventPublisher">Optional, an implementation of the <see cref="EventPublisher" /> delegate for publishing events <see cref="EventPublisher" />.</param>
        /// <param name="precedencePrinter">Optional, an implementation of the <see cref="IConditionPrecedencePrinter" /> interface 
        /// that will create a string showing the order of precedence for the conditions that are to be evaluated. 
        /// </param>
        /// <param name="cancellationToken">The cancellation token used to signify any cancellation requests.</param>
        /// <returns>Asynchronous operation that returns an instance of the <see cref="ConditionResult" /> class.</returns>
        Task<ConditionSetResult<T>> Evaluate(ConditionEvaluatorResolver evaluatorResolver, ConditionData conditionData, EventPublisher? eventPublisher = null, IConditionPrecedencePrinter? precedencePrinter = null, CancellationToken cancellationToken = default);
    }
}