using System;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using WebLinter;

namespace WebLinterTest
{
    [TestClass]
    public class EshintTest
    {
        [TestMethod, TestCategory("ESLint")]
        public void Standard()
        {
            var result = LinterFactory.Lint("../../artifacts/eslint/a.js", Settings.Instance);
            Assert.IsTrue(result.HasErrors);
            Assert.AreEqual(3, result.Errors.Count, $"Found {result.Errors.Count} errors");
        }

        [TestMethod, TestCategory("ESLint")]
        public void JSX()
        {
            var result = LinterFactory.Lint("../../artifacts/eslint/a.jsx", Settings.Instance);
            Assert.IsTrue(result.HasErrors);
            Assert.IsTrue(result.Errors.First().Message.Contains("react/display-name"), "Unexpected error message");
            Assert.AreEqual(2, result.Errors.Count, $"Found {result.Errors.Count} errors");
        }

        [TestMethod, TestCategory("ESLint")]
        public void FileDontExist()
        {
            var result = LinterFactory.Lint("../../artifacts/eslint/doesntexist.js", Settings.Instance);
            Assert.IsTrue(result.HasErrors);
        }
    }
}
