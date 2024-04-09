using Conditionals.Core.Areas.Conditions;
using Conditionals.Core.Common.Models;
using Conditionals.Core.Common.Seeds;

namespace Conditionals.Core.Areas.Evaluators;

/// <summary>
/// Marker interface to identify condition evaluators
/// </summary>
public interface IConditionEvaluator { }

/// <summary>
/// A context specific condition evaluator is responsible for evaluating <see cref="Condition{TContext}" /> instances. 
/// </summary>
/// <typeparam name="TContext">The data type of the condition to be evaluated.</typeparam>
public interface IConditionEvaluator<TContext> : IConditionEvaluator
{
    /// <summary>
    /// 
    /// </summary>
    /// <param name="condition">The <see cref="Condition{TContext}" /> to be evaluated.</param>
    /// <param name="data">The data used in the evaluation of the condition.</param>
    /// <param name="cancellationToken">The cancellation token used to signify any cancellation requests.</param>
    /// <param name="tenantID">Optional, a specific tenantID value otherwise the default value of All_Tenants will be applied,</param>
    /// <returns>An asynchronous operation returning a <see cref="EvaluationResult" /></returns>
    public Task<EvaluationResult> Evaluate(Condition<TContext> condition, TContext data, CancellationToken cancellationToken, string tenantID = GlobalStrings.Default_TenantID);
}
