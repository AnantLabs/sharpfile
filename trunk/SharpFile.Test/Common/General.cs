using NUnit.Framework;

namespace SharpFile.Test.Common {
	[TestFixture]
	public class General {
		[Test]
		public void GetHumanReadableSizeTest() {
			// d:\\ <2147479552> == 2 GB
			// u:\\ <81603786240>
		}
	}
}