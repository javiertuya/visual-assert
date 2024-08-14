package giis.visualassert;

import static org.junit.Assert.assertEquals;
import static org.junit.Assert.assertTrue;

import org.junit.Test;

import giis.visualassert.portable.CallStack;

/**
 * Checks the stack trace items shown in messages.
 * WARNING: Expected messages (at the end of this class) are sensitive to 
 * the line numbers where the asserts are thrown.
 * Adjust the numbers if new lines are added to the code
 */
public class TestSoftVisualAssertCallStack {

	//Deep stack assert invocations
	public class OtherClass {
		public void doAssert(SoftVisualAssert va) {
			va.assertEquals("xy vw", "xy zz vw", "", "fstack12.html");			
		}
	}

	private void callDoAssert(SoftVisualAssert va) {
		OtherClass cls=new OtherClass();
		cls.doAssert(va);
	}
	
	/*
	 * Conditions:
	 * -default(1)/more traces (0 already tested)
	 * -single/multiple fault
	 * -assert in the same test case/deeper in call stack (test class/other class)
	 */
	@Test
	public void testSingleStackItem() {
		SoftVisualAssert va = new SoftVisualAssert(); //default stack 1
		va.assertEquals("ab zz cd", "ab cd", "", "fstack11.html");
		callDoAssert(va);
		boolean success = false;
		try {
			va.assertAll();
		} catch (AssertionError e) {
			assertEquals(CallStack.normalize(getExpectedSingleStackItem()), CallStack.normalize(e.getMessage()));
			success=true;
		}
		assertTrue(success);
	}
	@Test
	public void testMultipleStackItem() {
		SoftVisualAssert va = new SoftVisualAssert().setCallStackLength(3);
		callDoAssert(va);
		boolean success = false;
		try {
			va.assertAll();
		} catch (AssertionError e) {
			assertEquals(CallStack.normalize(getExpectedMultipleStackItem()), CallStack.normalize(e.getMessage()));
			success=true;
		}
		assertTrue(success);
	}
	@Test
	public void testZeroStackItem() {
		SoftVisualAssert va = new SoftVisualAssert().setCallStackLength(0);
		va.assertEquals("ab zz cd", "ab cd", "", "fstack11.html");
		callDoAssert(va);
		boolean success = false;
		try {
			va.assertAll();
		} catch (AssertionError e) {
			assertEquals(CallStack.normalize(getExpectedZeroStackItem()), CallStack.normalize(e.getMessage()));
			success=true;
		}
		assertTrue(success);
	}
	
	private String getExpectedSingleStackItem() {
		return "There are 2 failed assertion(s)\n"
				+ "Failure 1: Strings are different. First diff at line 1 column 4.\n"
				+ "- Visual diffs at: target/fstack11.html\n"
				+ "- Call Stack:\n"
				+ "    at giis.visualassert.TestSoftVisualAssertCallStack.testSingleStackItem(TestSoftVisualAssertCallStack.java:37)\n"
				+ "Failure 2: Strings are different. First diff at line 1 column 4.\n"
				+ "- Visual diffs at: target/fstack12.html\n"
				+ "- Call Stack:\n"
				+ "    at giis.visualassert.TestSoftVisualAssertCallStack$OtherClass.doAssert(TestSoftVisualAssertCallStack.java:19)";
	}

	private String getExpectedMultipleStackItem() {
		return "There are 1 failed assertion(s)\n"
				+ "Failure 1: Strings are different. First diff at line 1 column 4.\n"
				+ "- Visual diffs at: target/fstack12.html\n"
				+ "- Call Stack:\n"
				+ "    at giis.visualassert.TestSoftVisualAssertCallStack$OtherClass.doAssert(TestSoftVisualAssertCallStack.java:19)\n"
				+ "    at giis.visualassert.TestSoftVisualAssertCallStack.callDoAssert(TestSoftVisualAssertCallStack.java:25)\n"
				+ "    at giis.visualassert.TestSoftVisualAssertCallStack.testMultipleStackItem(TestSoftVisualAssertCallStack.java:49)";
	}

	private String getExpectedZeroStackItem() {
		return "There are 2 failed assertion(s)\n"
				+ "Failure 1: Strings are different. First diff at line 1 column 4.\n"
				+ "- Visual diffs at: target/fstack11.html\n"
				+ "Failure 2: Strings are different. First diff at line 1 column 4.\n"
				+ "- Visual diffs at: target/fstack12.html";
	}
}
