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

		public SoftVisualAssert() : base()
		{
			AssertClear();
		}

		protected override void ThrowAssertionError(string message)
		{
			// instead of throwing the exception, stores the message
			assertionMessages.Add(message);
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
