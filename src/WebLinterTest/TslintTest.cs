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
            var result = LinterFactory.Lint(Settings.Instance, "../../artifacts/tslint/a.ts");
            Assert.IsTrue(result.HasErrors);
            Assert.IsFalse(string.IsNullOrEmpty(result.Errors.First().FileName), "File name is empty");
            Assert.AreEqual(7, result.Errors.Count, $"Found {result.Errors.Count} errors");
            Assert.AreEqual("if statements must be braced", result.Errors.First().Message);
        }

        [TestMethod, TestCategory("TSLint")]
        public void Multiple()
        {
            var result = LinterFactory.Lint(Settings.Instance, "../../artifacts/tslint/a.ts", "../../artifacts/tslint/b.ts");
            Assert.IsTrue(result.HasErrors);
            Assert.IsFalse(string.IsNullOrEmpty(result.Errors.First().FileName), "File name is empty");
            Assert.AreEqual(14, result.Errors.Count, $"Found {result.Errors.Count} errors");
            Assert.AreEqual("if statements must be braced", result.Errors.First().Message);
        }

        [TestMethod, TestCategory("TSLint")]
        public void FileNotExist()
        {
            var result = LinterFactory.Lint(Settings.Instance, "../../artifacts/tslint/doesntexist.js");
            Assert.IsTrue(result.HasErrors);
        }
    }
}
