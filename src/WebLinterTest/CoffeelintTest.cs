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
            var result = LinterFactory.Lint(Settings.CWD, Settings.Instance, "../../artifacts/coffeelint/a.coffee");
            Assert.IsTrue(result.First().HasErrors);
            Assert.IsTrue(result.First().Errors.First().IsError, "Severity is not 'error'");
            Assert.IsFalse(result.First().Errors.ElementAt(1).IsError, "Severity is not 'warning'");
            Assert.IsFalse(string.IsNullOrEmpty(result.First().Errors.First().FileName), "File name is empty");
            Assert.AreEqual(2, result.First().Errors.Count);
        }
        [TestMethod, TestCategory("CoffeeLint")]
        public void Multiple()
        {
            var result = LinterFactory.Lint(Settings.CWD, Settings.Instance, "../../artifacts/coffeelint/a.coffee", "../../artifacts/coffeelint/b.coffee");
            Assert.IsTrue(result.First().HasErrors);
            Assert.AreEqual(3, result.First().Errors.Count);
        }


        [TestMethod, TestCategory("CoffeeLint")]
        public void FileDontExist()
        {
            var result = LinterFactory.Lint(Settings.CWD, Settings.Instance, "../../artifacts/coffeelint/doesntexist.coffee");
            Assert.IsTrue(result.First().HasErrors);
        }
    }
}
