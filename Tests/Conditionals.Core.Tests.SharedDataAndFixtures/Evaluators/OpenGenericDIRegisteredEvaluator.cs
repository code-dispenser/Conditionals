using Conditionals.Core.Areas.Conditions;
using Conditionals.Core.Areas.Evaluators;
using Conditionals.Core.Common.Models;
using Conditionals.Core.Tests.SharedDataAndFixtures.Models;

namespace Conditionals.Core.Tests.SharedDataAndFixtures.Evaluators;

public class OpenGenericDIRegisteredEvaluator<TContext>(InjectedStrategy injectedStrategy) : ConditionEvaluatorBase<TContext>
{
    public InjectedStrategy InjectedStrategy { get; } = injectedStrategy;

    public override Task<EvaluationResult> Evaluate(Condition<TContext> condition, TContext data, CancellationToken cancellationToken, string tenantID = "All_Tenants")
    {
        throw new NotImplementedException();
    }
}
