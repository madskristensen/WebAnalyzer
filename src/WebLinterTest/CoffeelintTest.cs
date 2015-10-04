using System.Linq;
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
            var result = LinterFactory.Lint(Settings.Instance, "../../artifacts/coffeelint/a.coffee");
            Assert.IsTrue(result.HasErrors);
            Assert.IsFalse(string.IsNullOrEmpty(result.Errors.First().FileName), "File name is empty");
            Assert.AreEqual(2, result.Errors.Count, $"Found {result.Errors.Count} errors");
        }
        [TestMethod, TestCategory("CoffeeLint")]
        public void Multiple()
        {
            var result = LinterFactory.Lint(Settings.Instance, "../../artifacts/coffeelint/a.coffee", "../../artifacts/coffeelint/b.coffee");
            Assert.IsTrue(result.HasErrors);
            Assert.AreEqual(4, result.Errors.Count, $"Found {result.Errors.Count} errors");
        }


        [TestMethod, TestCategory("CoffeeLint")]
        public void FileDontExist()
        {
            var result = LinterFactory.Lint(Settings.Instance, "../../artifacts/coffeelint/doesntexist.coffee");
            Assert.IsTrue(result.HasErrors);
        }
    }
}
