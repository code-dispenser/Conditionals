using Conditionals.Core.Areas.Conditions;
using Conditionals.Core.Common.Seeds;

namespace Conditionals.Core.Tests.SharedDataAndFixtures.FormatConverters;

public class ErroneousPrecedencePrinter : IConditionPrecedencePrinter
{
    public string PrintPrecedenceOrder(BooleanConditionBase booleanCondition)
    {
        throw new NotImplementedException();
    }
}
