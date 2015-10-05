using System.Linq;
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
        public void Standard()
        {
            var result = LinterFactory.Lint(Settings.CWD, Settings.Instance, "../../artifacts/eslint/a.js");
            var first = result.First();
            Assert.IsTrue(first.HasErrors);
            Assert.IsTrue(first.Errors.First().IsError, "Severity is not 'error'");
            Assert.IsFalse(string.IsNullOrEmpty(first.Errors.First().FileName), "File name is empty");
            Assert.AreEqual(3, first.Errors.Count);
        }

        [TestMethod, TestCategory("ESLint")]
        public void MultipleInput()
        {
            var result = LinterFactory.Lint(Settings.CWD, Settings.Instance, "../../artifacts/eslint/a.js", "../../artifacts/eslint/b.js");
            Assert.IsTrue(result.First().HasErrors);
            Assert.AreEqual(6, result.First().Errors.Count);
        }

        [TestMethod, TestCategory("ESLint")]
        public void JSX()
        {
            var result = LinterFactory.Lint(Settings.CWD, Settings.Instance, "../../artifacts/eslint/a.jsx");
            Assert.IsTrue(result.First().HasErrors);
            Assert.IsTrue(result.First().Errors.First().Message.Contains("react/display-name"), "Unexpected error message");
            Assert.AreEqual(2, result.First().Errors.Count);
        }

        [TestMethod, TestCategory("ESLint")]
        public void FileNotExist()
        {
            var result = LinterFactory.Lint(Settings.CWD, Settings.Instance, "../../artifacts/eslint/doesntexist.js");
            Assert.IsTrue(result.First().HasErrors);
        }
    }
}
