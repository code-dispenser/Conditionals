using Conditionals.Core.Areas.Conditions;
using Conditionals.Core.Common.Exceptions;
using Conditionals.Core.Common.Models;
using Conditionals.Core.Common.Seeds;

namespace Conditionals.Core.Areas.Evaluators;

/// <summary>
/// An implementation of <see cref="ConditionEvaluatorBase{TContext}" /> abstract class that calls the compiled lambda predicate of an <see cref="PredicateCondition{TContext}" /> class.> 
/// </summary>
/// <inheritdoc cref="ConditionEvaluatorBase{TContext}" /> 
public sealed class PredicateConditionEvaluator<TContext> : ConditionEvaluatorBase<TContext>
{
    /// <summary>
    /// Evaluates instances of the <see cref="PredicateCondition{TContext}" /> class.
    /// </summary>
    /// <param name="condition">The condition to be evaluated.</param>
    /// <param name="data">The data used in the evaluation.</param>
    /// <param name="cancellationToken">The cancellation token used to signify any cancellation requests.</param>
    /// <param name="tenantID">The id of the tenant who the data belongs to.</param>
    /// <returns>An asynchronous operation that returns an instance of the <see cref="EvaluationResult"/> class.</returns>
    public override Task<EvaluationResult> Evaluate(Condition<TContext> condition, TContext data, CancellationToken cancellationToken, string tenantID = GlobalStrings.Default_TenantID)
    {
        cancellationToken.ThrowIfCancellationRequested();

        if (condition.ConditionType != ConditionType.LambdaPredicate)
        {
            return Task.FromResult(new EvaluationResult(false, String.Empty, new PredicateConditionCompilationException(GlobalStrings.Predicate_Condition_Compilation_Exception_Message)));
        }

        var result = condition.CompiledPredicate!(data);
        var failureMessage = result ? String.Empty : base.BuildFailureMessage(condition.FailureMessage, data!, ConditionEvaluatorBase<TContext>.MessageRegex);

        return Task.FromResult(new EvaluationResult(result, failureMessage));
    }
}
