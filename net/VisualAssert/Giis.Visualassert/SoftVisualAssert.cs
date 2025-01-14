using Giis.Visualassert.Portable;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
/////// THIS FILE HAS BEEN AUTOMATICALLY CONVERTED FROM THE JAVA SOURCES. DO NOT EDIT ///////
using static Giis.Visualassert.Framework;

namespace Giis.Visualassert
{
    /// <summary>
    /// Records all VisualAssert assertion message that fail and the diff files instead of throwing and exception;
    /// Calling assertAll() will cause an exception to be thrown including all assertion messages
    /// (if one or more assertions failed).
    /// </summary>
    public class SoftVisualAssert : AbstractVisualAssert<SoftVisualAssert>
    {
        private IList<string> assertionMessages;
        private IList<string> userMessages; // additional message set in the assert
        //For each failure message, stores expected and actual to generate aggregated diffs
        private IList<string> failureExpected;
        private IList<string> failureActual;
        private int callStackLength = 1; //by default shows the single line where assert failed
        public SoftVisualAssert() : base()
        {
            AssertClear();
        }

        /// <summary>
        /// Sets the number of the call stack items that are shown when a soft assertion fails (default 1)
        /// </summary>
        /// <param name="length">number of call stack items</param>
        /// <returns>this object to allow fluent style</returns>
        public virtual SoftVisualAssert SetCallStackLength(int length)
        {
            this.callStackLength = length;
            return this;
        }

        public override void AssertEquals(string expected, string actual, string message, string fileName)
        {
            expected = Normalize(expected);
            actual = Normalize(actual);
            if (!StringsAreEqual(expected, actual))
                ThrowAssertionError(GetAssertionMessage(expected, actual, message, fileName), message, expected, actual);
        }

        protected virtual void ThrowAssertionError(string message, string userMessage, string expected, string actual)
        {

            // instead of throwing the exception, stores the message
            if (callStackLength > 0)
            {

                //firstly adds the call stack traces when appropriate
                CallStack stack = new CallStack();
                message += (message.EndsWith("\n") ? "" : "\n") + GetStackTraceMessage(stack);
            }

            assertionMessages.Add(message);
            userMessages.Add(userMessage);
            failureExpected.Add(expected);
            failureActual.Add(actual);
        }

        private string GetStackTraceMessage(CallStack stack)
        {
            StringBuilder sb = new StringBuilder();
            bool skip = true;
            int countTraces = 0;
            for (int i = 0; i < stack.Count; i++)
            {

                //Skip traces corresponding to the classes of this package that are at the top of stack
                //case insensitive for net compatibility
                string stackClass = stack.GetClassName(i).ToLower();
                if (skip && !stackClass.StartsWith("giis.visualassert.portable.callstack") && !stackClass.StartsWith("giis.visualassert.abstractvisualassert") && !stackClass.StartsWith("giis.visualassert.softvisualassert") && !stackClass.StartsWith("giis.visualassert.visualassert") && !stackClass.StartsWith("system.environment"))
                {
                    skip = false;
                }


                //Collect the desired number of traces
                if (!skip)
                {
                    sb.Append("\n    at ").Append(stack.GetClassName(i) + "." + stack.GetMethodName(i) + "(" + stack.GetFileName(i) + ":" + stack.GetLineNumber(i) + ")");
                    countTraces++;
                    if (countTraces >= callStackLength)
                        return "- Call Stack:" + sb.ToString();
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
        public virtual void AssertAll(string aggregateFileName)
        {
            if (assertionMessages.Count == 0)
                return;
            if (aggregateFileName == null)
                aggregateFileName = "";

            //Composes a single message for all assertions and throws the exception
            StringBuilder allMsg = new StringBuilder();
            allMsg.Append("There are " + assertionMessages.Count + " failed assertion(s)");
            if (!"".Equals(aggregateFileName))
                allMsg.Append("\nAggregated visual diffs at: " + GetFileUrl(aggregateFileName));
            foreach (string msg in assertionMessages)
                allMsg.Append("\n" + msg);

            //Also collects expected and actual values for each failure
            string expected = ComposeOutputInfo(failureExpected);
            string actual = ComposeOutputInfo(failureActual);
            if (!"".Equals(aggregateFileName))
            {
                string htmlDiff = GetHtmlDiffs(expected, actual);
                WriteDiffFile(htmlDiff, aggregateFileName);
            }

            AssertClear(); //reset
            if (!StringsAreEqual(expected, actual))
                platformAssert.FailNotEquals(expected, actual, allMsg.ToString());
        }

        /// <summary>
        /// Throws and exception if at least one assertion failed including all assertion messages
        /// </summary>
        public virtual void AssertAll()
        {
            AssertAll("");
        }

        private string ComposeOutputInfo(IList<string> expectedOrActual)
        {
            StringBuilder sb = new StringBuilder();
            sb.Append("Aggregated failures:");
            for (int i = 0; i < expectedOrActual.Count; i++)
            {
                sb.Append("\n").Append(GetAggregateFailureHeader(i, userMessages[i])).Append("\n").Append(expectedOrActual[i] == null ? "" : expectedOrActual[i]);
            }

            return sb.ToString();
        }

        public virtual string GetAggregateFailureHeader(int failureNumber, string userMessage)
        {
            return "\n---------------------------" + "\n-------- Failure " + (failureNumber + 1) + " --------" + GetUserMessage(userMessage) + "\n---------------------------";
        }

        private string GetUserMessage(string userMessage)
        {
            if (JavaCs.IsEmpty(userMessage))
                return "";
            return "\n-------- " + userMessage;
        }

        /// <summary>
        /// Resets the current failure messages that are stored
        /// </summary>
        public virtual void AssertClear()
        {
            assertionMessages = new List<string>();
            userMessages = new List<string>();
            failureExpected = new List<string>();
            failureActual = new List<string>();
        }

        /// <summary>
        /// Returns the current number of failure messages
        /// </summary>
        public virtual int GetFailureCount()
        {
            return assertionMessages.Count;
        }

        /// <summary>
        /// Fails a test with no message
        /// </summary>
        public virtual void Fail()
        {
            this.Fail("");
        }

        /// <summary>
        /// Fails a test with the given message
        /// </summary>
        public virtual void Fail(string message)
        {
            string messageInfo = "Fail assertion raised." + ("".Equals(message) ? "" : "\n" + message);

            // sets the message info as the actual value to show in the comparison file
            ThrowAssertionError(GetMessagePrefix() + " " + messageInfo, message, "", messageInfo);
        }
    }
}