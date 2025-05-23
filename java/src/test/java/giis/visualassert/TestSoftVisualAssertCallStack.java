package giis.visualassert;

import static org.junit.Assert.assertEquals;
import static org.junit.Assert.assertThrows;

import org.junit.Test;

import giis.visualassert.portable.CallStack;
import giis.visualassert.portable.JavaCs;

/**
 * Checks the stack trace items shown in messages.
 * WARNING: Expected messages (at the end of this class) are sensitive to 
 * the line numbers where the asserts are thrown.
 * Adjust the numbers if new lines are added to the code
 */
public class TestSoftVisualAssertCallStack {
	
	private String innerClassSeparator = JavaCs.isJava() ? "$" : ".";

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
		AssertionError e = assertThrows(AssertionError.class, () -> {
			va.assertAll();
		});
		assertEquals(CallStack.normalize(getExpectedSingleStackItem()).toLowerCase(), CallStack.normalize(e.getMessage()).toLowerCase());
	}
	@Test
	public void testMultipleStackItem() {
		SoftVisualAssert va = new SoftVisualAssert().setCallStackLength(3);
		callDoAssert(va);
		AssertionError e = assertThrows(AssertionError.class, () -> {
			va.assertAll();
		});
		assertEquals(CallStack.normalize(getExpectedMultipleStackItem()).toLowerCase(), CallStack.normalize(e.getMessage()).toLowerCase());
	}
	@Test
	public void testZeroStackItem() {
		SoftVisualAssert va = new SoftVisualAssert().setCallStackLength(0);
		va.assertEquals("ab zz cd", "ab cd", "", "fstack11.html");
		callDoAssert(va);
		AssertionError e = assertThrows(AssertionError.class, () -> {
			va.assertAll();
		});
		assertEquals(CallStack.normalize(getExpectedZeroStackItem()), CallStack.normalize(e.getMessage()));
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
				+ "    at giis.visualassert.TestSoftVisualAssertCallStack" + innerClassSeparator + "OtherClass.doAssert(TestSoftVisualAssertCallStack.java:19)";
	}

	private String getExpectedMultipleStackItem() {
		return "There are 1 failed assertion(s)\n"
				+ "Failure 1: Strings are different. First diff at line 1 column 4.\n"
				+ "- Visual diffs at: target/fstack12.html\n"
				+ "- Call Stack:\n"
				+ "    at giis.visualassert.TestSoftVisualAssertCallStack" + innerClassSeparator + "OtherClass.doAssert(TestSoftVisualAssertCallStack.java:19)\n"
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
