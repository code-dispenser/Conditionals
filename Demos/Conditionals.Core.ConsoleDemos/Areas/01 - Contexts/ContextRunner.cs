using Conditionals.Core.Areas.Engine;
using static Conditionals.Core.ConsoleDemos.Common.Utilities.GeneralUtils;

namespace Conditionals.Core.ConsoleDemos.Areas.Contexts;

public static class ContextRunner
{
    public static async Task Start(ConditionEngine conditionEngine)
    {
        WriteLineSeparator();
        WriteLine("1a-StoreCardApplication-Single-Data-Context");
        WriteLine();

        await new StoreCardApplicationSingleContext(conditionEngine).CheckApplicant();

        WriteLineSeparator();
        WriteLine("1b-StoreCardApplication-Multiple-Data-Contexts");
        WriteLine();

        await new StoreCardApplicationMultipleDataContexts(conditionEngine).CheckApplicant();


        WriteLineSeparator();
        WriteLine("1c-SampleOrders-Named_Data-Contexts");
        WriteLine();

        await new SampleOrdersNamedDataContexts(conditionEngine).CheckOrders();
    }


}
