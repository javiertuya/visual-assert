/////////////////////////////////////////////////////////////////////////////////////////////
/////// THIS FILE HAS BEEN AUTOMATICALLY CONVERTED FROM THE JAVA SOURCES. DO NOT EDIT ///////
/////////////////////////////////////////////////////////////////////////////////////////////


namespace Giis.Visualassert
{
	/// <summary>
	/// Assertion methods that generate an html file with the differences
	/// highlighting the additions and deletions;
	/// useful for comparing large strings.
	/// </summary>
	public class VisualAssert : AbstractVisualAssert<VisualAssert>
	{
		public override void AssertEquals(string expected, string actual, string message, string fileName)
		{
			expected = Normalize(expected);
			actual = Normalize(actual);
			if (!StringsAreEqual(expected, actual))
			{
				platformAssert.FailNotEquals(expected, actual, GetAssertionMessage(expected, actual, message, fileName));
			}
		}

		protected internal override string GetAssertionMessage(string expected, string actual, string message, string fileName)
		{
			return GetAssertionMessage(expected, actual, message, fileName, "Strings are different.");
		}
	}
}
