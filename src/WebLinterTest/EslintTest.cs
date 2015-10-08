using System.Linq;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using WebLinter;

namespace WebLinterTest
{
    [TestClass]
    public class EslintTest
    {
        [TestInitialize]
        public void Setup()
        {
            Telemetry.Enabled = false;
        }

        [TestMethod, TestCategory("ESLint")]
        public async Task Standard()
        {
            var result = await LinterFactory.Lint(Settings.Instance, "../../artifacts/eslint/a.js");
            var first = result.First();
            Assert.IsTrue(first.HasErrors);
            Assert.IsFalse(first.Errors.First().IsError, "Severity is not 'warning'");
            Assert.IsFalse(string.IsNullOrEmpty(first.Errors.First().FileName), "File name is empty");
            Assert.AreEqual(4, first.Errors.Count);
        }

        [TestMethod, TestCategory("ESLint")]
        public async Task MultipleInput()
        {
            var result = await LinterFactory.Lint(Settings.Instance, "../../artifacts/eslint/a.js", "../../artifacts/eslint/b.js");
            Assert.IsTrue(result.First().HasErrors);
            Assert.AreEqual(8, result.First().Errors.Count);
        }

        [TestMethod, TestCategory("ESLint")]
        public async Task JSX()
        {
            var result = await LinterFactory.Lint(Settings.Instance, "../../artifacts/eslint/a.jsx");
            Assert.IsTrue(result.First().HasErrors);
            Assert.AreEqual("react/display-name", result.First().Errors.First().ErrorCode, "Unexpected error message");
            Assert.AreEqual(2, result.First().Errors.Count);
        }

        [TestMethod, TestCategory("ESLint")]
        public async Task FileNotExist()
        {
            var result = await LinterFactory.Lint(Settings.Instance, "../../artifacts/eslint/doesntexist.js");
            Assert.IsTrue(result.First().HasErrors);
        }
    }
}
