package giis.visualassert;

/**
 * Assertion methods that generate an html file with the differences
 * highlighting the additions and deletions; 
 * useful for comparing large strings.
 */
public class VisualAssert extends AbstractVisualAssert<VisualAssert> {

	@Override
	public void assertEquals(String expected, String actual, String message, String fileName) {
		if (!expected.equals(actual))
			platformAssert.assertEquals(expected, actual, getAssertionMessage(expected, actual, message, fileName));
	}

	@Override
	protected String getAssertionMessage(String expected, String actual, String message, String fileName) {
		return getAssertionMessage(expected, actual, message, fileName, "Strings are different.");
	}
}
