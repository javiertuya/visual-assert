/////////////////////////////////////////////////////////////////////////////////////////////
/////// THIS FILE HAS BEEN AUTOMATICALLY CONVERTED FROM THE JAVA SOURCES. DO NOT EDIT ///////
/////////////////////////////////////////////////////////////////////////////////////////////
using System;


namespace Giis.Visualassert
{
	[System.Serializable]
	public class FailedTest : Exception
	{
		private const long serialVersionUID = 5657525735601303767L;

		public FailedTest()
			: base("Previous statement in this test should fail")
		{
		}
	}
}
