package giis.visualassert;

import org.junit.Ignore;
import org.junit.Test;

/**
 * When specifying a framework, the message that includes expected and actual 
 * values and the raised exception are provided by the framework.
 * This test class reuses the assertions from the framework independent tests.
 */
public class TestWithFrameworks {
	
	private String junit4AdditionalMessage="\n expected:<[abc def ghi\n"
			+ "mno pqr s ]tu> but was:<[ abc DEF ghi\n"
			+ "other line\n"
			+ "mno pqr s]tu>";
	private String junit5AdditionalMessage="\n ==> expected: <abc def ghi\n"
			+ "mno pqr s tu> but was: < abc DEF ghi\n"
			+ "other line\n"
			+ "mno pqr stu>";

	@Test
	public void testJunit4Pass() {
		VisualAssert va = new VisualAssert().setFramework(Framework.JUNIT4);
		va.assertEquals("ab cd\nxy zt", "ab cd\nxy zt");
	}

	@Test
	public void testJunit4Failure() {
		VisualAssert va = new VisualAssert()
				.setShowExpectedAndActual(false)
				.setFramework(Framework.JUNIT4);
		TestVisualAssert.doFailAllConditionsFalse(va, "org.junit.ComparisonFailure", "", junit4AdditionalMessage);
	}

	@Test
	public void testJunit4FailureWithMessage() {
		VisualAssert va = new VisualAssert()
				.setShowExpectedAndActual(false)
				.setFramework(Framework.JUNIT4);
		TestVisualAssert.doFailAllConditionsFalse(va, "org.junit.ComparisonFailure", "XXX", junit4AdditionalMessage);
	}

	@Test
	public void testJunit4FailureWithShowExpectedAndActual() {
		VisualAssert va = new VisualAssert()
				.setShowExpectedAndActual(true) //this should be ignored (replaced by native diff message)
				.setFramework(Framework.JUNIT4);
		TestVisualAssert.doFailAllConditionsFalse(va, "org.junit.ComparisonFailure", "", junit4AdditionalMessage);
	}

	@Test
	public void testJunit3Failure() {
		VisualAssert va = new VisualAssert()
				.setShowExpectedAndActual(true) //this should be ignored (replaced by native diff message)
				.setFramework(Framework.JUNIT3);
		TestVisualAssert.doFailAllConditionsFalse(va, "junit.framework.ComparisonFailure", "", junit4AdditionalMessage);
	}

	@Test
	public void testJunit5Failure() {
		VisualAssert va = new VisualAssert()
				.setShowExpectedAndActual(true) //this should be ignored (replaced by native diff message)
				.setFramework(Framework.JUNIT5);
		TestVisualAssert.doFailAllConditionsFalse(va, "org.opentest4j.AssertionFailedError", "", junit5AdditionalMessage);
	}

	//Remove the Ignore annotation to check that we can see 
	//the diffs from the development environment
	
	@Ignore
	@Test
	public void testJUnit4Interactive() {
		VisualAssert va=new VisualAssert().setFramework(Framework.JUNIT4);
		va.assertEquals("ab c\n d", "ab x\n d");
	}
	
	@Ignore
	@Test
	public void testJUnit3Interactive() {
		VisualAssert va=new VisualAssert().setFramework(Framework.JUNIT3);
		va.assertEquals("ab c\n d", "ab x\n d");
	}

	@Ignore
	@Test
	public void testJUnit5Interactive() {
		VisualAssert va=new VisualAssert().setFramework(Framework.JUNIT5);
		va.assertEquals("ab c\n d", "ab x\n d");
	}
	
}
