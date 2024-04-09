using Conditionals.Core.Areas.Engine;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using static Conditionals.Core.ConsoleDemos.Common.Utilities.GeneralUtils;

namespace Conditionals.Core.ConsoleDemos.Areas.TenantsAndCulture;

public static class TenantsAndCultureRunner
{
    public static async Task Start(ConditionEngine conditionEngine)
    {
        WriteLineSeparator();
        WriteLine("7a-Tenants-And-Culture");
        WriteLine();

        await new TenantsAndCulture(conditionEngine).RunTenantIDAndCulture();
    }
}
