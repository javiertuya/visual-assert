using System;

namespace Giis.Visualassert
{
    public class FrameworkAssert
    {
        public FrameworkAssert(Framework framework) { }
        public Framework GetFramework() { return Framework.NONE; }
        public void FailNotEquals(string expected, string actual, string message) { throw new Exception(message); }
    }
}
