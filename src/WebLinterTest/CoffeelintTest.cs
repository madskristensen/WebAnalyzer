using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using WebLinter;

namespace WebLinterTest
{
    [TestClass]
    public class CoffeelintTest
    {
        [TestMethod, TestCategory("CoffeeLint")]
        public void Standard()
        {
            var result = LinterFactory.Lint("../../artifacts/coffeelint/a.coffee");
            Assert.IsTrue(result.HasErrors);
            Assert.AreEqual(1, result.Errors.Count, $"Found {result.Errors.Count} errors");
        }

        [TestMethod, TestCategory("CoffeeLint")]
        public void FileDontExist()
        {
            var result = LinterFactory.Lint("../../artifacts/coffeelint/doesntexist.coffee");
            Assert.IsTrue(result.HasErrors);
        }
    }
}
