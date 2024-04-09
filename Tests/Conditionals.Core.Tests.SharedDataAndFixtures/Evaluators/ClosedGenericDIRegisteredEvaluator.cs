using Conditionals.Core.Areas.Conditions;
using Conditionals.Core.Areas.Evaluators;
using Conditionals.Core.Common.Models;
using Conditionals.Core.Tests.SharedDataAndFixtures.Models;

namespace Conditionals.Core.Tests.SharedDataAndFixtures.Evaluators;

public class ClosedGenericDIRegisteredEvaluator(InjectedStrategy injectedStrategy) : ConditionEvaluatorBase<Customer>
{
    public InjectedStrategy InjectedStrategy { get; } = injectedStrategy;

    public override Task<EvaluationResult> Evaluate(Condition<Customer> condition, Customer data, CancellationToken cancellationToken, string tenantID = "All_Tenants")
    {
        throw new NotImplementedException();
    }

}
