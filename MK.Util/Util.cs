using System.Numerics;
using System.Text.RegularExpressions;

namespace MK.Util;

/// <summary>
/// General utilities for .NET.
/// </summary>
public static partial class Util {

	/// <param name="val">A number.</param>
	/// <param name="low">The lower bound.</param>
	/// <param name="high">The upper bound.</param>
	/// <returns>low <= val <= high.</returns>
	public static bool Between<N>(this N val, N low, N high) where N : INumber<N> => (val >= low) && (val <= high);

	/// <param name="min">The minimum value.</param>
	/// <param name="max">The maximum value.</param>
	/// <returns>An enumerable over numbers from min to max inclusive, in steps of 1.</returns>
	public static IEnumerable<N> Range<N>(N min, N max) where N : INumber<N> => Range(min, max, N.One);

	/// <param name="min">The minimum value.</param>
	/// <param name="max">The maximum value.</param>
	/// <param name="step">The value by which to increment.</param>
	/// <returns>An enumerable over numbers from min to max inclusive, in steps of step.</returns>
	public static IEnumerable<N> Range<N>(N min, N max, N step) where N : INumber<N> {
		for (N i = min; i <= max; i += step) {
			yield return i;
		}
	}

	[GeneratedRegex(@"\s+")]
	static partial Regex WhitespaceRegex {get;}

	/// <param name="str">An input string.</param>
	/// <returns>The string with all whitespace collapsed to single spaces.</returns>
	public static string Collapse(this string str) => WhitespaceRegex.Replace(str, " ").Trim();

}
