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
            var result = LinterFactory.Lint(Settings.CWD, Settings.Instance, "../../artifacts/eslint/a.js");
            Assert.IsTrue(result.First().HasErrors);
            Assert.IsFalse(string.IsNullOrEmpty(result.First().Errors.First().FileName), "File name is empty");
            Assert.AreEqual(3, result.First().Errors.Count);
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
