using System.Linq;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using WebLinter;

namespace WebLinterTest
{
    [TestClass]
    public class CoffeelintTest
    {
        [TestInitialize]
        public void Setup()
        {
            Telemetry.Enabled = false;
        }

        [TestMethod, TestCategory("CoffeeLint")]
        public async Task Standard()
        {
            var result = await LinterFactory.LintAsync(Settings.Instance, "../../artifacts/coffeelint/a.coffee");
            Assert.IsTrue(result.First().HasErrors);
            Assert.IsTrue(result.First().Errors.First().IsError, "Severity is not 'error'");
            Assert.IsFalse(string.IsNullOrEmpty(result.First().Errors.First().FileName), "File name is empty");
            Assert.AreEqual(2, result.First().Errors.Count);
        }
        [TestMethod, TestCategory("CoffeeLint")]
        public async Task Multiple()
        {
            var result = await LinterFactory.LintAsync(Settings.Instance, "../../artifacts/coffeelint/a.coffee", "../../artifacts/coffeelint/b.coffee");
            Assert.IsTrue(result.First().HasErrors);
            Assert.AreEqual(3, result.First().Errors.Count);
        }


        [TestMethod, TestCategory("CoffeeLint")]
        public async Task FileDontExist()
        {
            var result = await LinterFactory.LintAsync(Settings.Instance, "../../artifacts/coffeelint/doesntexist.coffee");
            Assert.IsTrue(result.First().HasErrors);
        }
    }
}
