using System;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using WebLinter;

namespace WebLinterTest
{
    [TestClass]
    public class TshintTest
    {
        [TestMethod, TestCategory("TSLint")]
        public void Standard()
        {
            var result = LinterFactory.Lint("../../artifacts/tslint/a.ts", Settings.Instance);
            Assert.IsTrue(result.HasErrors);
            Assert.AreEqual(7, result.Errors.Count, $"Found {result.Errors.Count} errors");
            Assert.AreEqual("if statements must be braced", result.Errors.First().Message);
        }

        [TestMethod, TestCategory("TSLint")]
        public void FileDontExist()
        {
            var result = LinterFactory.Lint("../../artifacts/tslint/doesntexist.js", Settings.Instance);
            Assert.IsTrue(result.HasErrors);
        }
    }
}
