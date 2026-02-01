namespace MK.Util;

/// <summary>
/// Utility methods for date/time.
/// </summary>
public static class Dates {

	/// <param name="year">A year in the Gregorian calendar.</param>
	/// <returns>true iff the year is a leap year.</returns>
	public static bool YearIsLeap(int year) => (year % 100 == 0) ? (year % 400 == 0) : (year % 4 == 0);

}
