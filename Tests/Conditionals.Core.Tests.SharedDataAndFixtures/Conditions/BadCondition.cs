using Conditionals.Core.Areas.Conditions;
using Conditionals.Core.Areas.Events;
using Conditionals.Core.Common.Models;
using Conditionals.Core.Common.Seeds;

namespace Conditionals.Core.Tests.SharedDataAndFixtures.Conditions
{
    public class BadCondition : BooleanConditionBase, ICondition
    {
        public Dictionary<string, string> AdditionalInfo => [];

        public EventDetails? EventDetails => null;

        public string ConditionName => "No Name";

        public ConditionType ConditionType => ConditionType.LambdaPredicate;

        public Type ContextType => typeof(SystemException);

        public string EvaluatorTypeName => "No Evaluator";

        public string ExpressionToEvaluate => "No Expression to evaluate";

        public string FailureMessage => "No failure message";

        public override Task<ConditionResult> Evaluate(ConditionEvaluatorResolver evaluatorResolver, ConditionData dataContexts, EventPublisher? eventPublisher = null, ConditionResult? previousResult = null, CancellationToken cancellationToken = default)
        {
            throw new NotImplementedException();
        }
    }
}
