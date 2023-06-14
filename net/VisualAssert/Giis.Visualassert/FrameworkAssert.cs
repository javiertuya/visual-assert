/////////////////////////////////////////////////////////////////////////////////////////////
/////// THIS FILE HAS BEEN AUTOMATICALLY CONVERTED FROM THE JAVA SOURCES. DO NOT EDIT ///////
/////////////////////////////////////////////////////////////////////////////////////////////
using System;


namespace Giis.Visualassert
{
	public class FrameworkAssert
	{
		public FrameworkAssert(Framework framework)
		{
		}

		public virtual Framework GetFramework()
		{
			return Framework.None;
		}

		public virtual void FailNotEquals(string expected, string actual, string message)
		{
			throw new Exception(message);
		}
	}
}
