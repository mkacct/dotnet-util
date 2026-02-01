using System.Numerics;

namespace MK.Util.Tests;

public static class UtilTest {

	public sealed class BetweenTest {

		[Theory]
		[InlineData(4, 3, 5)]
		[InlineData(3, 3, 3)]
		[InlineData(0, int.MinValue, int.MaxValue)]
		public void Integers_True(int val, int low, int high) {
			Assert.True(val.Between(low, high));
		}

		[Theory]
		[InlineData(2, 3, 5)]
		[InlineData(6, 3, 5)]
		public void Integers_False(int val, int low, int high) {
			Assert.False(val.Between(low, high));
		}

		[Theory]
		[InlineData(3.1, 3, 3.2)]
		[InlineData(3.1, 3.1, 3.1)]
		[InlineData(0, double.MinValue, double.MaxValue)]
		public void Doubles_True(double val, double low, double high) {
			Assert.True(val.Between(low, high));
		}

		[Theory]
		[InlineData(2.9, 3, 3.2)]
		[InlineData(3.3, 3, 3.2)]
		public void Doubles_False(double val, double low, double high) {
			Assert.False(val.Between(low, high));
		}

	}

	public sealed class RangeTest {

		static void TestNoStep<N>(N[] expected, N min, N max) where N : INumber<N> {
			int i = 0;
			foreach (N val in Util.Range(min, max)) {
				Assert.Equal(expected[i], val);
				i++;
			}
			Assert.Equal(expected.Length, i);
		}

		static void TestWithStep<N>(N[] expected, N min, N max, N step) where N : INumber<N> {
			int i = 0;
			foreach (N val in Util.Range(min, max, step)) {
				Assert.Equal(expected[i], val);
				i++;
			}
			Assert.Equal(expected.Length, i);
		}

		[Theory]
		[InlineData(new int[] {0, 1, 2, 3, 4, 5}, 0, 5)]
		[InlineData(new int[] {-3, -2, -1, 0, 1, 2, 3}, -3, 3)]
		public void Integers(int[] expected, int min, int max) {
			TestNoStep(expected, min, max);
		}

		[Theory]
		[InlineData(new double[] {3, 4, 5, 6}, 3, 6.6)]
		[InlineData(new double[] {3.2, 4.2, 5.2, 6.2}, 3.2, 6.2)]
		[InlineData(new double[] {3.2, 4.2, 5.2}, 3.2, 6.1)]
		public void Doubles(double[] expected, double min, double max) {
			TestNoStep(expected, min, max);
		}

		[Theory]
		[InlineData(new double[] {3.2, 3.7, 4.2, 4.7, 5.2, 5.7, 6.2, 6.7}, 3.2, 6.7, 0.5)]
		[InlineData(new double[] {3.2, 3.7, 4.2, 4.7, 5.2, 5.7, 6.2}, 3.2, 6.6, 0.5)]
		public void CustomStep(double[] expected, double min, double max, double step) {
			TestWithStep(expected, min, max, step);
		}

	}

	public sealed class CollapseTest {

		[Theory]
		[InlineData("foo", "foo")]
		[InlineData("foo", "\n  foo  \t ")]
		[InlineData("foo bar", "\n  foo\f\t\nbar  \t ")]
		public void Simple(string expected, string str) {
			Assert.Equal(expected, str.Collapse());
		}

	}

}
