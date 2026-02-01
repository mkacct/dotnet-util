namespace MK.Util.Tests;

public static class DatesTest {

	public sealed class YearIsLeapTest {

		[Theory]
		[InlineData(1600)]
		[InlineData(2000)]
		[InlineData(2004)]
		[InlineData(2008)]
		public void LeapYears(int year) {
			Assert.True(Dates.YearIsLeap(year));
		}

		[Theory]
		[InlineData(1700)]
		[InlineData(1800)]
		[InlineData(1900)]
		[InlineData(2100)]
		[InlineData(2200)]
		[InlineData(2300)]
		[InlineData(2001)]
		[InlineData(2002)]
		[InlineData(2003)]
		[InlineData(2005)]
		[InlineData(2006)]
		[InlineData(2007)]
		public void NotLeapYears(int year) {
			Assert.False(Dates.YearIsLeap(year));
		}

	}

}
