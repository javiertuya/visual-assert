package giis.visualassert.portable;

/**
 * Determines the call stack trace from the position of the method where this instance has been created
 */
public class CallStack {
	private StackTraceElement[] stack;
	public CallStack() {
		Throwable tr=new Throwable();
		stack = tr.getStackTrace();
	}
	public int size() {
		return stack.length;
	}
	public String getClassName(int position) {
		return stack[position].getClassName();
	}
	public String getMethodName(int position) {
		return stack[position].getMethodName();
	}
	public String getFileName(int position) {
		return stack[position].getFileName();
	}
	public int getLineNumber(int position) {
		return stack[position].getLineNumber();
	}
	public String getString() {
		StringBuilder sb=new StringBuilder();
		for (int i=0; i<stack.length; i++)
			sb.append("\n        " + getClassName(i)+" "+getMethodName(i)+" "+getLineNumber(i)+" "+getFileName(i));
		return sb.toString();
	}

}
