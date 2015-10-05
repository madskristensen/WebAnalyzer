namespace WebLinter
{
    public interface ISettings
    {
        bool ESLintEnable { get; }

        bool TSLintEnable { get; }

        bool CoffeeLintEnable { get; }

        bool CssLintEnable { get; }
    }
}
