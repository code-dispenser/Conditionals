using Conditionals.Core.Areas.Engine;

using static Conditionals.Core.ConsoleDemos.Common.Utilities.GeneralUtils;

namespace Conditionals.Core.ConsoleDemos.Areas.JsonRules;

public static class JsonRunner
{
    public static async Task Start(ConditionEngine conditionEngine)
    {
        WriteLineSeparator();
        WriteLine("4a-Output-To-Json");
        WriteLine();

        OutputToJson.RunOutputToJson();

        WriteLineSeparator();
        WriteLine("4b-Rules-From-Json");
        WriteLine();

        await new RulesFromJson(conditionEngine).RunRuleWithoutAddingToEngine();

        WriteLineSeparator();
        WriteLine("4c-Ingesting-Json-Rules");
        WriteLine();

        await new IngestingJsonRules(conditionEngine).RunIngestingJsonRules();
    }
}
