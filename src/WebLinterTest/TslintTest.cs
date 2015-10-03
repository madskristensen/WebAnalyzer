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
            var result = LinterFactory.Lint("../../artifacts/tslint/a.ts");
            Assert.IsTrue(result.HasErrors);
            Assert.AreEqual("if statements must be braced", result.Errors.First().Message);
        }

        [TestMethod, TestCategory("TSLint")]
        public void FileDontExist()
        {
            var result = LinterFactory.Lint("../../artifacts/tslint/doesntexist.js");
            Assert.IsTrue(result.HasErrors);
        }
    }
}
