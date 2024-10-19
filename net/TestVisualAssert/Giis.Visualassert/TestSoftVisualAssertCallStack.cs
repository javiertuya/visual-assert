using NUnit.Framework;
using Giis.Visualassert.Portable;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
/////// THIS FILE HAS BEEN AUTOMATICALLY CONVERTED FROM THE JAVA SOURCES. DO NOT EDIT ///////

namespace Giis.Visualassert
{
    /// <summary>
    /// Checks the stack trace items shown in messages.
    /// WARNING: Expected messages (at the end of this class) are sensitive to
    /// the line numbers where the asserts are thrown.
    /// Adjust the numbers if new lines are added to the code
    /// </summary>
    public class TestSoftVisualAssertCallStack
    {
        private string innerClassSeparator = JavaCs.IsJava() ? "$" : ".";
        //Deep stack assert invocations
        public class OtherClass
        {
            public virtual void DoAssert(SoftVisualAssert va)
            {
                va.AssertEquals("xy vw", "xy zz vw", "", "fstack12.html");
            }
        }

        private void CallDoAssert(SoftVisualAssert va)
        {
            OtherClass cls = new OtherClass();
            cls.DoAssert(va);
        }

        /*
         * Conditions:
         * -default(1)/more traces (0 already tested)
         * -single/multiple fault
         * -assert in the same test case/deeper in call stack (test class/other class)
         */
        [Test]
        public virtual void TestSingleStackItem()
        {
            SoftVisualAssert va = new SoftVisualAssert(); //default stack 1
            va.AssertEquals("ab zz cd", "ab cd", "", "fstack11.html");
            CallDoAssert(va);
            bool success = false;
            try
            {
                va.AssertAll();
            }
            catch (Exception e)
            {
                NUnit.Framework.Legacy.ClassicAssert.AreEqual(CallStack.Normalize(GetExpectedSingleStackItem()).ToLower(), CallStack.Normalize(e.Message).ToLower());
                success = true;
            }

            NUnit.Framework.Legacy.ClassicAssert.IsTrue(success);
        }

        [Test]
        public virtual void TestMultipleStackItem()
        {
            SoftVisualAssert va = new SoftVisualAssert().SetCallStackLength(3);
            CallDoAssert(va);
            bool success = false;
            try
            {
                va.AssertAll();
            }
            catch (Exception e)
            {
                NUnit.Framework.Legacy.ClassicAssert.AreEqual(CallStack.Normalize(GetExpectedMultipleStackItem()).ToLower(), CallStack.Normalize(e.Message).ToLower());
                success = true;
            }

            NUnit.Framework.Legacy.ClassicAssert.IsTrue(success);
        }

        [Test]
        public virtual void TestZeroStackItem()
        {
            SoftVisualAssert va = new SoftVisualAssert().SetCallStackLength(0);
            va.AssertEquals("ab zz cd", "ab cd", "", "fstack11.html");
            CallDoAssert(va);
            bool success = false;
            try
            {
                va.AssertAll();
            }
            catch (Exception e)
            {
                NUnit.Framework.Legacy.ClassicAssert.AreEqual(CallStack.Normalize(GetExpectedZeroStackItem()), CallStack.Normalize(e.Message));
                success = true;
            }

            NUnit.Framework.Legacy.ClassicAssert.IsTrue(success);
        }

        private string GetExpectedSingleStackItem()
        {
            return "There are 2 failed assertion(s)\n" + "Failure 1: Strings are different. First diff at line 1 column 4.\n" + "- Visual diffs at: ../../../../reports/fstack11.html\n" + "- Call Stack:\n" + "    at giis.visualassert.TestSoftVisualAssertCallStack.testSingleStackItem(TestSoftVisualAssertCallStack.java:37)\n" + "Failure 2: Strings are different. First diff at line 1 column 4.\n" + "- Visual diffs at: ../../../../reports/fstack12.html\n" + "- Call Stack:\n" + "    at giis.visualassert.TestSoftVisualAssertCallStack" + innerClassSeparator + "OtherClass.doAssert(TestSoftVisualAssertCallStack.java:19)";
        }

        private string GetExpectedMultipleStackItem()
        {
            return "There are 1 failed assertion(s)\n" + "Failure 1: Strings are different. First diff at line 1 column 4.\n" + "- Visual diffs at: ../../../../reports/fstack12.html\n" + "- Call Stack:\n" + "    at giis.visualassert.TestSoftVisualAssertCallStack" + innerClassSeparator + "OtherClass.doAssert(TestSoftVisualAssertCallStack.java:19)\n" + "    at giis.visualassert.TestSoftVisualAssertCallStack.callDoAssert(TestSoftVisualAssertCallStack.java:25)\n" + "    at giis.visualassert.TestSoftVisualAssertCallStack.testMultipleStackItem(TestSoftVisualAssertCallStack.java:49)";
        }

        private string GetExpectedZeroStackItem()
        {
            return "There are 2 failed assertion(s)\n" + "Failure 1: Strings are different. First diff at line 1 column 4.\n" + "- Visual diffs at: ../../../../reports/fstack11.html\n" + "Failure 2: Strings are different. First diff at line 1 column 4.\n" + "- Visual diffs at: ../../../../reports/fstack12.html";
        }
    }
}