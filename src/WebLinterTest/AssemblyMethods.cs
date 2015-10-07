using Microsoft.VisualStudio.TestTools.UnitTesting;
using WebLinter;

namespace WebLinterTest
{
    [TestClass]
    public class AssemblyMethods
    {
        [AssemblyCleanup]
        public static void Cleanup()
        {
            NodeServer.Down();
        }
    }
}
