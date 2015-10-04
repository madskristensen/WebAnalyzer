using System;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using WebLinter;

namespace WebLinterTest
{
    [TestClass]
    public class EslintTest
    {
        [TestMethod, TestCategory("ESLint")]
        public void Standard()
        {
            var result = LinterFactory.Lint(Settings.Instance, "../../artifacts/eslint/a.js");
            Assert.IsTrue(result.HasErrors);
            Assert.IsFalse(string.IsNullOrEmpty(result.Errors.First().FileName), "File name is empty");
            Assert.AreEqual(3, result.Errors.Count, $"Found {result.Errors.Count} errors");
        }

        [TestMethod, TestCategory("ESLint")]
        public void MultipleInput()
        {
            var result = LinterFactory.Lint(Settings.Instance, "../../artifacts/eslint/a.js", "../../artifacts/eslint/b.js");
            Assert.IsTrue(result.HasErrors);
            Assert.AreEqual(6, result.Errors.Count, $"Found {result.Errors.Count} errors");
        }

        [TestMethod, TestCategory("ESLint")]
        public void JSX()
        {
            var result = LinterFactory.Lint(Settings.Instance, "../../artifacts/eslint/a.jsx");
            Assert.IsTrue(result.HasErrors);
            Assert.IsTrue(result.Errors.First().Message.Contains("react/display-name"), "Unexpected error message");
            Assert.AreEqual(2, result.Errors.Count, $"Found {result.Errors.Count} errors");
        }

        [TestMethod, TestCategory("ESLint")]
        public void FileNotExist()
        {
            var result = LinterFactory.Lint(Settings.Instance, "../../artifacts/eslint/doesntexist.js");
            Assert.IsTrue(result.HasErrors);
        }
    }
}
