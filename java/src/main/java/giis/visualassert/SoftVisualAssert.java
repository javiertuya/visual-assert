package giis.visualassert;

import java.util.ArrayList;
import java.util.List;

/**
 * Records all VisualAssert assertion message that fail and the diff files instead of throwing and exception;
 * Calling assertAll() will cause an exception to be thrown including all assertion messages 
 * (if one or more assertions failed).
 */
public class SoftVisualAssert extends VisualAssert {

	private List<String> assertionMessages;
	
	public SoftVisualAssert() {
		super();
		assertClear();
	}
	
	@Override
	protected void throwAssertionError(String message) {
		// instead of throwing the exception, stores the message
		assertionMessages.add(message);
	}
	@Override
	protected String getAssertionMessage(String expected, String actual, String message, String fileName) {
		return super.getAssertionMessage(expected, actual, message, fileName, getMessagePrefix()+" Strings are different.");
	}
	private String getMessagePrefix() {
		return "Failure " + (assertionMessages.size()+1) + ":";
	}
	
	/**
	 * Throws and exception if at least one assertion failed including all assertion messages
	 */
	public void assertAll() {
		if (assertionMessages.isEmpty())
			return;
		//Composes a single message for all assertions and throws the exception (by calling the parent method)
		StringBuilder allMsg = new StringBuilder();
		allMsg.append("There are " + assertionMessages.size() + " failed assertion(s)");
		for (String msg : assertionMessages)
			allMsg.append("\n" + msg);
		assertClear(); //reset
		super.throwAssertionError(allMsg.toString());
	}
	
	/**
	 * Resets the current failure messages that are stored
	 */
	public void assertClear() {
		assertionMessages=new ArrayList<String>();
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
		throwAssertionError("");
	}
	/**
	 * Fails a test with the given message
	 */
	public void fail(String message) {
		throwAssertionError(getMessagePrefix() + " Fail assertion raised." + ("".equals(message) ? "" : "\n"+message));
	}
	

}
