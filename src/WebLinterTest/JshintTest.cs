using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using WebLinter;

namespace WebLinterTest
{
    [TestClass]
    public class JshintTest
    {
        [TestMethod, TestCategory("JSHint")]
        public void Standard()
        {
            var result = LinterFactory.Lint("../../artifacts/jshint/a.js", Settings.Instance);
            Assert.IsTrue(result.HasErrors);
            Assert.AreEqual(4, result.Errors.Count);
        }

        [TestMethod, TestCategory("JSHint")]
        public void FileDontExist()
        {
            var result = LinterFactory.Lint("../../artifacts/jshint/doesntexist.js", Settings.Instance);
            Assert.IsTrue(result.HasErrors);
        }
    }
}
