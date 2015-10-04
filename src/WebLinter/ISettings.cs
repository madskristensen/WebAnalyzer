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
