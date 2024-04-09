using Conditionals.Core.Areas.Events;
using Conditionals.Core.Common.Seeds;

namespace Conditionals.Core.Areas.Conditions
{
    /// <summary>
    /// The condition to be evaluated. The condition itself will most likely be a lambda predicate held as a string in the ExpressionToEvaluate property.
    /// </summary>
    public interface ICondition
    {
        /// <summary>
        /// Gets the dictionary that can be used to hold additional information for custom conditions and their respective evaluators.
        /// </summary>
        Dictionary<string, string> AdditionalInfo   { get; }

        /// <summary>
        /// Gets the event details regarding any associated event <see cref="EventDetails" />
        /// </summary>
        EventDetails? EventDetails                  { get; }

        /// <summary>
        /// Gets the name used to identify this condition.
        /// </summary>
        string ConditionName                        { get; }

        /// <summary>
        /// Gets the enumerated type that indicates whether the conditions expression is a Lambda predicate or a custom expression.  
        /// </summary>
        ConditionType ConditionType                 { get; }

        /// <summary>
        /// Gets the data type of the condition.
        /// </summary>
        Type ContextType                            { get; }

        /// <summary>
        /// Gets the name of the evaluator used to evaluate the condition.
        /// </summary>
        string EvaluatorTypeName                    { get; }

        /// <summary>
        /// Gets the expression to be evaluated by an evaluator.
        /// </summary>
        string ExpressionToEvaluate                 { get; }

        /// <summary>
        /// Gets the failure message to be used when the condition evaluates to false.
        /// </summary>
        string FailureMessage                       { get; }
    }
}