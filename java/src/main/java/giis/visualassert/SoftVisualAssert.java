package giis.visualassert;

import java.util.ArrayList;
import java.util.List;

import giis.visualassert.portable.CallStack;
import giis.visualassert.portable.JavaCs;

/**
 * Records all VisualAssert assertion message that fail and the diff files instead of throwing and exception;
 * Calling assertAll() will cause an exception to be thrown including all assertion messages 
 * (if one or more assertions failed).
 */
public class SoftVisualAssert extends AbstractVisualAssert<SoftVisualAssert> {

	private List<String> assertionMessages;
	private List<String> userMessages; // additional message set in the assert
	//For each failure message, stores expected and actual to generate aggregated diffs
	private List<String> failureExpected;
	private List<String> failureActual;
	
	private int callStackLength = 1; //by default shows the single line where assert failed
	
	public SoftVisualAssert() {
		super();
		assertClear();
	}
	
	/**
	 * Sets the number of the call stack items that are shown when a soft assertion fails (default 1)
	 * @param length number of call stack items
	 * @return this object to allow fluent style
	 */
	public SoftVisualAssert setCallStackLength(int length) {
		this.callStackLength = length;
		return this;
	}
	
	@Override
	public void assertEquals(String expected, String actual, String message, String fileName) {
		expected = normalize(expected);
		actual = normalize(actual);
		if (!stringsAreEqual(expected, actual))
			throwAssertionError(getAssertionMessage(expected, actual, message, fileName), message, expected, actual);
	}

	protected void throwAssertionError(String message, String userMessage, String expected, String actual) {
		// instead of throwing the exception, stores the message
		if (callStackLength>0) { 
			//firstly adds the call stack traces when appropriate
			CallStack stack=new CallStack();
			message += (message.endsWith("\n") ? "" : "\n") + getStackTraceMessage(stack);
		}
		assertionMessages.add(message);
		userMessages.add(userMessage);
		failureExpected.add(expected);
		failureActual.add(actual);
	}
	private String getStackTraceMessage(CallStack stack) {
		StringBuilder sb=new StringBuilder();
		boolean skip=true;
		int countTraces=0;
		for (int i=0; i<stack.size(); i++) {
			//Skip traces corresponding to the classes of this package that are at the top of stack
			//case insensitive for net compatibility
			String stackClass = stack.getClassName(i).toLowerCase();
			if (skip 
					&& !stackClass.startsWith("giis.visualassert.portable.callstack")
					&& !stackClass.startsWith("giis.visualassert.abstractvisualassert") 
					&& !stackClass.startsWith("giis.visualassert.softvisualassert") 
					&& !stackClass.startsWith("giis.visualassert.visualassert")
					&& !stackClass.startsWith("system.environment") //only net
					) {
				skip=false;
			}
			//Collect the desired number of traces
			if (!skip) {
				sb.append("\n    at ").append(stack.getClassName(i) + "." + stack.getMethodName(i) 
						+ "(" + stack.getFileName(i) + ":" +stack.getLineNumber(i) +")");
				countTraces++;
				if (countTraces>=callStackLength)
					return "- Call Stack:" + sb.toString();
			}
		}
		return "";
	}
	
	@Override
	protected String getAssertionMessage(String expected, String actual, String message, String fileName) {
		return super.getAssertionMessage(expected, actual, message, fileName, getMessagePrefix() +" Strings are different.");
	}
	private String getMessagePrefix() {
		return "Failure " + (assertionMessages.size()+1) + ":";
	}
	
	/**
	 * Throws and exception if at least one assertion failed including all assertion messages
	 */
	public void assertAll(String aggregateFileName) {
		if (assertionMessages.size() == 0)
			return;
		if (aggregateFileName == null)
			aggregateFileName = "";
		//Composes a single message for all assertions and throws the exception
		StringBuilder allMsg = new StringBuilder();
		allMsg.append("There are " + assertionMessages.size() + " failed assertion(s)");
		if (!"".equals(aggregateFileName))
			allMsg.append("\nAggregated visual diffs at: " + getFileUrl(aggregateFileName));
		
		for (String msg : assertionMessages)
			allMsg.append("\n" + msg);
		
		//Also collects expected and actual values for each failure
		String expected=composeOutputInfo(failureExpected);
		String actual=composeOutputInfo(failureActual);
		if (!"".equals(aggregateFileName)) {
			String htmlDiff = getHtmlDiffs(expected, actual);
			writeDiffFile(htmlDiff, aggregateFileName);
		}
		assertClear(); //reset
		if (!stringsAreEqual(expected, actual))
			platformAssert.failNotEquals(expected, actual, allMsg.toString());
	}
	/**
	 * Throws and exception if at least one assertion failed including all assertion messages
	 */
	public void assertAll() {
		assertAll("");
	}
	
	private String composeOutputInfo(List<String> expectedOrActual) {
		StringBuilder sb=new StringBuilder();
		sb.append("Aggregated failures:");
		for (int i = 0; i<expectedOrActual.size(); i++) {
			sb.append("\n").append(getAggregateFailureHeader(i, userMessages.get(i)))
				.append("\n").append(expectedOrActual.get(i) == null ? "" : expectedOrActual.get(i));
		}
		return sb.toString();
	}
	public String getAggregateFailureHeader(int failureNumber, String userMessage) {
		return "\n---------------------------"
			+ "\n-------- Failure " + (failureNumber+1) + " --------"
			+ getUserMessage(userMessage)
			+ "\n---------------------------";
	}
	private String getUserMessage(String userMessage) {
		if (JavaCs.isEmpty(userMessage))
			return "";
		return "\n-------- " + userMessage;
	}
	/**
	 * Resets the current failure messages that are stored
	 */
	public void assertClear() {
		assertionMessages=new ArrayList<String>();
		userMessages=new ArrayList<String>();
		failureExpected=new ArrayList<String>();
		failureActual=new ArrayList<String>();
	}
	/**
	 * Returns the current number of failure messages
	 */
	public int getFailureCount() {
		return assertionMessages.size();
	}
	
	/**
	 * Fails a test with no message
	 */
	public void fail() {
		this.fail("");
	}
	/**
	 * Fails a test with the given message
	 */
	public void fail(String message) {
		String messageInfo = "Fail assertion raised." + ("".equals(message) ? "" : "\n"+message);
		// sets the message info as the actual value to show in the comparison file
		throwAssertionError(getMessagePrefix() + " " + messageInfo, message, "", messageInfo);
	}
	

}
