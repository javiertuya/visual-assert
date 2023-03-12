package giis.visualassert;

/**
 * Performs the actual assert action for the specified platform.
 * If a test framework is specified at instantiation, it executes the corresponding assert
 * If not, it throws a generic assertion exception
 */
public class FrameworkAssert {
	
	private Framework framework;
	
	public FrameworkAssert(Framework framework) {
		this.framework = framework==null ? Framework.NONE : framework;
	}
	
	public Framework getFramework() {
		return framework;
	}

	@SuppressWarnings("deprecation")
	public void assertEquals(String expected, String actual, String message) {
		if (!expected.equals(actual)) {
			if (framework==Framework.JUNIT3) {
				junit.framework.Assert.assertEquals(message, expected, actual);
			} else if (framework==Framework.JUNIT4) {
				org.junit.Assert.assertEquals(message, expected, actual);
			} else if (framework==Framework.JUNIT5) {
				org.junit.jupiter.api.Assertions.assertEquals(expected, actual, message);
			} else
				throw new AssertionError(message);
		}
	}
	
}
