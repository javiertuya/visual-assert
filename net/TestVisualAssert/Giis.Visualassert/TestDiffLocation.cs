/////////////////////////////////////////////////////////////////////////////////////////////
/////// THIS FILE HAS BEEN AUTOMATICALLY CONVERTED FROM THE JAVA SOURCES. DO NOT EDIT ///////
/////////////////////////////////////////////////////////////////////////////////////////////
using NUnit.Framework;


namespace Giis.Visualassert
{
	public class TestDiffLocation
	{
		[Test]
		public virtual void TestEqualSizes()
		{
			VisualAssert va = new VisualAssert();
			NUnit.Framework.Assert.AreEqual("No diffs", va.GetDiffLocation("a b\nc d", "a b\nc d"));
			NUnit.Framework.Assert.AreEqual("First diff at line 1 column 1", va.GetDiffLocation("X b\nc d", "a b\nc d"));
			NUnit.Framework.Assert.AreEqual("First diff at line 1 column 3", va.GetDiffLocation("a X\nc d", "a b\nc d"));
			NUnit.Framework.Assert.AreEqual("First diff at line 2 column 1", va.GetDiffLocation("a b\nX d", "a b\nc d"));
			NUnit.Framework.Assert.AreEqual("First diff at line 2 column 3", va.GetDiffLocation("a b\nc X", "a b\nc d"));
			// More than one diff (report first one)
			NUnit.Framework.Assert.AreEqual("First diff at line 1 column 3", va.GetDiffLocation("a X\nX d", "a b\nc d"));
		}

		[Test]
		public virtual void TestDifferentSizesNoFail()
		{
			VisualAssert va = new VisualAssert();
			NUnit.Framework.Assert.AreEqual("Expected is contained in actual", va.GetDiffLocation("a b\nc d", "a b\nc d+"));
			NUnit.Framework.Assert.AreEqual("Expected is contained in actual", va.GetDiffLocation("a b\nc d", "a b\nc d\n"));
			NUnit.Framework.Assert.AreEqual("Actual is contained in expected", va.GetDiffLocation("a b\nc d+", "a b\nc d"));
			NUnit.Framework.Assert.AreEqual("Actual is contained in expected", va.GetDiffLocation("a b\nc d\n", "a b\nc d"));
		}

		[Test]
		public virtual void TestDifferentSizesWithFail()
		{
			VisualAssert va = new VisualAssert();
			NUnit.Framework.Assert.AreEqual("First diff at line 2 column 3", va.GetDiffLocation("a b\nc X", "a b\nc d+"));
			NUnit.Framework.Assert.AreEqual("First diff at line 2 column 3", va.GetDiffLocation("a b\nc d+", "a b\nc X"));
		}

		[Test]
		public virtual void TestEmptyFiles()
		{
			VisualAssert va = new VisualAssert();
			NUnit.Framework.Assert.AreEqual("Expected is empty", va.GetDiffLocation(string.Empty, "a b\nc d"));
			NUnit.Framework.Assert.AreEqual("Actual is empty", va.GetDiffLocation("a b\nc d", string.Empty));
			NUnit.Framework.Assert.AreEqual("No diffs", va.GetDiffLocation(string.Empty, string.Empty));
		}
	}
}
