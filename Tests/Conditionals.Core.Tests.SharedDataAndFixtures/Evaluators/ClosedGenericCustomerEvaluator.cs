using Conditionals.Core.Areas.Conditions;
using Conditionals.Core.Areas.Evaluators;
using Conditionals.Core.Common.Models;
using Conditionals.Core.Tests.SharedDataAndFixtures.Models;

namespace Conditionals.Core.Tests.SharedDataAndFixtures.Evaluators;

public class ClosedGenericCustomerEvaluator : ConditionEvaluatorBase<Customer>
{
    public override async Task<EvaluationResult> Evaluate(Condition<Customer> condition, Customer data, CancellationToken cancellationToken, string tenantID = "All_Tenants")
    
        => await Task.FromResult(new EvaluationResult(true, null, new SystemException()));
    
}
