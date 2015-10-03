using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WebLinter
{
    public interface ISettings
    {
        bool ESLintEnable { get; }
        bool ESLintAsErrors { get; }

        bool TSLintEnable { get; }
        bool TSLintAsErrors { get; }

        bool CoffeeLintEnable { get; }
        bool CoffeeLintAsErrors { get; }

        bool CssLintEnable { get; }
        bool CssLintAsErrors { get; }
    }
}
