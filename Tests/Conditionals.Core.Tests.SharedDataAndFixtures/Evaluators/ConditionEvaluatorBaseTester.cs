using Conditionals.Core.Areas.Conditions;
using Conditionals.Core.Areas.Evaluators;
using Conditionals.Core.Common.Models;

namespace Conditionals.Core.Tests.SharedDataAndFixtures.Evaluators;

public class ConditionEvaluatorBaseTester<TContext> : ConditionEvaluatorBase<TContext>
{
    private readonly bool _useBadFailureMessage = true;
    private readonly string _replacementText    = "N/A";

    public ConditionEvaluatorBaseTester()
    {
        
    }
    public ConditionEvaluatorBaseTester(bool useBadFailureMessage = true, string replacementText = "N/A")
    
        => (_useBadFailureMessage, _replacementText) = (useBadFailureMessage,replacementText);

    public override Task<EvaluationResult> Evaluate(Condition<TContext> condition, TContext data, CancellationToken cancellationToken, string tenantID = "All_Tenants")
    {
        var failureMessage = _useBadFailureMessage ? base.BuildFailureMessage(null!, data!, ConditionEvaluatorBase<TContext>.MessageRegex, "Property Not Found")
                                                   : base.BuildFailureMessage(condition.FailureMessage, data!, ConditionEvaluatorBase<TContext>.MessageRegex, _replacementText);
        return Task.FromResult(new EvaluationResult(false, failureMessage, null));
    }
}
