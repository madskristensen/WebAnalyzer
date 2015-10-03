using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WebLinter
{
    public interface ISettings
    {
        bool JSHintEnable { get; }
        bool JSHintAsErrors { get; }

        bool TSLintEnable { get; }
        bool TSLintAsErrors { get; }

        bool CoffeeLintEnable { get; }
        bool CoffeeLintAsErrors { get; }

        bool CssLintEnable { get; }
        bool CssLintAsErrors { get; }
    }
}
