using Giis.Visualassert.Portable;
using System.Collections.Generic;
using System.Text;

namespace Giis.Visualassert
{
	/// <summary>
	/// Records all VisualAssert assertion message that fail and the diff files instead of throwing and exception;
	/// Calling assertAll() will cause an exception to be thrown including all assertion messages 
	/// (if one or more assertions failed).
	/// </summary>
	public class SoftVisualAssert : VisualAssert
	{
		private List<string> assertionMessages;
		private int callStackLength = 1; //by default shows the single line where assert failed

		public SoftVisualAssert() : base()
		{
			AssertClear();
		}

		/// <summary>
		/// Sets the number of the call stack items that are shown when a soft assertion fails (default 1)
		/// </summary>
		public SoftVisualAssert SetCallStackLength(int length)
		{
			this.callStackLength = length;
			return this;
		}

		protected override void ThrowAssertionError(string message)
		{
			// instead of throwing the exception, stores the message
			if (callStackLength > 0)
			{
				//firstly adds the call stack traces when appropriate
				CallStack stack = new CallStack();
				message += GetStackTraceMessage(stack);
			}
			assertionMessages.Add(message);
		}
		private string GetStackTraceMessage(CallStack stack)
		{
			StringBuilder sb = new StringBuilder();
			bool skip = true;
			int countTraces = 0;
			for (int i = 0; i < stack.Size(); i++)
			{
				//Skip traces corresponding to the classes of this package that are at the top of stack
				if (skip && !stack.GetClassName(i).StartsWith("System.")
						&& !stack.GetClassName(i).StartsWith("Giis.Visualassert.Portable.CallStack")
						&& !stack.GetClassName(i).StartsWith("Giis.Visualassert.SoftVisualAssert")
						&& !stack.GetClassName(i).StartsWith("Giis.Visualassert.VisualAssert"))
				{
					skip = false;
				}
				//Collect the desired number of traces
				if (!skip)
				{
					sb.Append("\n    at ").Append(stack.GetClassName(i) + "." + stack.GetMethodName(i)
							+ "(" + stack.GetFileName(i) + ":" + stack.GetLineNumber(i) + ")");
					countTraces++;
					if (countTraces >= callStackLength)
						return "\n- Call Stack:" + sb.ToString();
				}
			}
			return "";
		}


		protected override string GetAssertionMessage(string expected, string actual, string message, string fileName)
		{
			return base.GetAssertionMessage(expected, actual, message, fileName, GetMessagePrefix() + " Strings are different.");
		}
		private string GetMessagePrefix()
		{
			return "Failure " + (assertionMessages.Count + 1) + ":";
		}

		/// <summary>
		/// Throws and exception if at least one assertion failed including all assertion messages
		/// </summary>
		public void AssertAll()
		{
			if (assertionMessages.Count==0)
				return;
			//Composes a single message for all assertions and throws the exception (by calling the parent method)
			StringBuilder allMsg = new StringBuilder();
			allMsg.Append("There are " + assertionMessages.Count + " failed assertion(s)");
			foreach (string msg in assertionMessages)
				allMsg.Append("\n" + msg);
			AssertClear(); //reset
			base.ThrowAssertionError(allMsg.ToString());
		}

		/// <summary>
		/// Resets the current failure messages that are stored
		/// </summary>
		public void AssertClear()
		{
			assertionMessages = new List<string>();
		}
		/// <summary>
		/// Returns the current number of failure messages
		/// </summary>
		public int GetFailureCount()
		{
			return assertionMessages.Count;
		}

		/// <summary>
		/// Fails a test with no message
		/// </summary>
		public void Fail()
		{
			ThrowAssertionError("");
		}
		/// <summary>
		/// Fails a test with the given message
		/// </summary>
		public void Fail(string message)
		{
			ThrowAssertionError(GetMessagePrefix() + " Fail assertion raised." + (string.IsNullOrEmpty(message) ? "" : "\n" + message));
		}

	}
}
