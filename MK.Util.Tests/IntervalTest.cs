using System.Globalization;
using System.Numerics;

namespace MK.Util.Tests;

public sealed class IntervalTest {

	const string EmptySet = "\u2205";
	const IntervalBoundary Open = IntervalBoundary.Open;
	const IntervalBoundary Closed = IntervalBoundary.Closed;

	[Fact]
	public void Empty_Integers() {
		AssertInterval(EmptySet, Interval<int>.Empty);
	}

	[Fact]
	public void Empty_Doubles() {
		AssertInterval(EmptySet, Interval<double>.Empty);
	}

	[Theory]
	[InlineData("[3, 5]", Closed, 3, Closed, 5)]
	[InlineData("(3, 5)", Open, 3, Open, 5)]
	[InlineData("[3, 5)", Closed, 3, Open, 5)]
	[InlineData("(3, 5]", Open, 3, Closed, 5)]
	[InlineData("[3, 3]", Closed, 3, Closed, 3)]
	[InlineData(EmptySet, Open, 3, Open, 3)]
	public void Constructor_Integers_Valid(
		string expectedStr,
		IntervalBoundary left, int min, IntervalBoundary right, int max
	) {
		AssertInterval(expectedStr, new Interval<int>(left, min, right, max));
	}

	[Theory]
	[InlineData("[3, 5]", Closed, 3, Closed, 5)]
	[InlineData("(3, 5)", Open, 3, Open, 5)]
	[InlineData("[3, 5)", Closed, 3, Open, 5)]
	[InlineData("(3, 5]", Open, 3, Closed, 5)]
	[InlineData("(-Infinity, 5]", Open, double.NegativeInfinity, Closed, 5)]
	[InlineData("[3, Infinity)", Closed, 3, Open, double.PositiveInfinity)]
	[InlineData("(-Infinity, Infinity)", Open, double.NegativeInfinity, Open, double.PositiveInfinity)]
	[InlineData("[3, 3]", Closed, 3, Closed, 3)]
	[InlineData(EmptySet, Open, 3, Open, 3)]
	[InlineData(EmptySet, Open, double.PositiveInfinity, Open, double.PositiveInfinity)]
	public void Constructor_Doubles_Valid(
		string expectedStr,
		IntervalBoundary left, double min, IntervalBoundary right, double max
	) {
		AssertInterval(expectedStr, new Interval<double>(left, min, right, max));
	}

	[Theory]
	[InlineData(Closed, 5, Closed, 3)]
	[InlineData(Open, 3, Closed, 3)]
	[InlineData(Closed, 3, Open, 3)]
	public void Constructor_Integers_Invalid (
		IntervalBoundary left, int min, IntervalBoundary right, int max
	) {
		Assert.Throws<ArgumentException>(() => new Interval<int>(left, min, right, max));
	}

	[Theory]
	[InlineData(Closed, 5, Closed, 3)]
	[InlineData(Closed, double.NaN, Closed, 3)]
	[InlineData(Closed, 3, Closed, double.NaN)]
	[InlineData(Closed, double.NegativeInfinity, Closed, 3)]
	[InlineData(Closed, 3, Closed, double.PositiveInfinity)]
	[InlineData(Open, double.PositiveInfinity, Open, double.NegativeInfinity)]
	[InlineData(Open, 3, Closed, 3)]
	[InlineData(Closed, 3, Open, 3)]
	public void Constructor_Doubles_Invalid (
		IntervalBoundary left, double min, IntervalBoundary right, double max
	) {
		Assert.Throws<ArgumentException>(() => new Interval<double>(left, min, right, max));
	}

	[Theory]
	[InlineData("(3, 5)", 3, 5)]
	[InlineData(EmptySet, 3, 3)]
	public void Open_Integers(
		string expectedStr,
		int min, int max
	) {
		AssertInterval(expectedStr, Interval<int>.Open(min, max));
	}

	[Theory]
	[InlineData("(3, 5)", 3, 5)]
	[InlineData(EmptySet, 3, 3)]
	[InlineData("(-Infinity, 5)", double.NegativeInfinity, 5)]
	[InlineData("(3, Infinity)", 3, double.PositiveInfinity)]
	public void Open_Doubles(
		string expectedStr,
		double min, double max
	) {
		AssertInterval(expectedStr, Interval<double>.Open(min, max));
	}

	[Theory]
	[InlineData("[3, 5]", 3, 5)]
	[InlineData("[3, 3]", 3, 3)]
	public void Closed_Integers(
		string expectedStr,
		int min, int max
	) {
		AssertInterval(expectedStr, Interval<int>.Closed(min, max));
	}

	[Theory]
	[InlineData("[3, 5]", 3, 5)]
	[InlineData("[3, 3]", 3, 3)]
	public void Closed_Doubles(
		string expectedStr,
		double min, double max
	) {
		AssertInterval(expectedStr, Interval<double>.Closed(min, max));
	}

	[Fact]
	public void IsEmpty_Integers_True() {
		Assert.True(Interval<int>.Empty.IsEmpty);
	}

	[Fact]
	public void IsEmpty_Doubles_True() {
		Assert.True(Interval<double>.Empty.IsEmpty);
	}

	[Theory]
	[InlineData(Open, 3, Open, 5)]
	[InlineData(Closed, 3, Closed, 5)]
	[InlineData(Closed, 3, Open, 5)]
	[InlineData(Open, 3, Closed, 5)]
	public void IsEmpty_Integers_False(
		IntervalBoundary left, int min, IntervalBoundary right, int max
	) {
		Assert.False(new Interval<int>(left, min, right, max).IsEmpty);
	}

	[Theory]
	[InlineData(Open, 3, Open, 5)]
	[InlineData(Closed, 3, Closed, 5)]
	[InlineData(Open, double.NegativeInfinity, Open, double.PositiveInfinity)]
	[InlineData(Closed, 3, Open, 5)]
	[InlineData(Open, 3, Closed, 5)]
	[InlineData(Open, double.NegativeInfinity, Closed, 5)]
	[InlineData(Closed, 3, Open, double.PositiveInfinity)]
	public void IsEmpty_Doubles_False(
		IntervalBoundary left, double min, IntervalBoundary right, double max
	) {
		Assert.False(new Interval<double>(left, min, right, max).IsEmpty);
	}

	[Theory]
	[InlineData(3, Open, 3, Open, 5)]
	[InlineData(3, Closed, 3, Closed, 5)]
	[InlineData(3, Closed, 3, Open, 5)]
	[InlineData(3, Open, 3, Closed, 5)]
	public void Min_Integers(
		int expected,
		IntervalBoundary left, int min, IntervalBoundary right, int max
	) {
		Assert.Equal(expected, new Interval<int>(left, min, right, max).Min);
	}

	[Theory]
	[InlineData(3, Open, 3, Open, 5)]
	[InlineData(3, Closed, 3, Closed, 5)]
	[InlineData(double.NegativeInfinity, Open, double.NegativeInfinity, Open, double.PositiveInfinity)]
	[InlineData(3, Closed, 3, Open, 5)]
	[InlineData(3, Open, 3, Closed, 5)]
	[InlineData(double.NegativeInfinity, Open, double.NegativeInfinity, Closed, 5)]
	[InlineData(3, Closed, 3, Open, double.PositiveInfinity)]
	public void Min_Doubles(
		double expected,
		IntervalBoundary left, double min, IntervalBoundary right, double max
	) {
		Assert.Equal(expected, new Interval<double>(left, min, right, max).Min);
	}

	[Fact]
	public void Min_Integers_Invalid() {
		Assert.Throws<InvalidOperationException>(() => Interval<int>.Empty.Min);
	}

	[Fact]
	public void Min_Doubles_Invalid() {
		Assert.Throws<InvalidOperationException>(() => Interval<double>.Empty.Min);
	}

	[Theory]
	[InlineData(5, Open, 3, Open, 5)]
	[InlineData(5, Closed, 3, Closed, 5)]
	[InlineData(5, Closed, 3, Open, 5)]
	[InlineData(5, Open, 3, Closed, 5)]
	public void Max_Integers(
		int expected,
		IntervalBoundary left, int min, IntervalBoundary right, int max
	) {
		Assert.Equal(expected, new Interval<int>(left, min, right, max).Max);
	}

	[Theory]
	[InlineData(5, Open, 3, Open, 5)]
	[InlineData(5, Closed, 3, Closed, 5)]
	[InlineData(double.PositiveInfinity, Open, double.NegativeInfinity, Open, double.PositiveInfinity)]
	[InlineData(5, Closed, 3, Open, 5)]
	[InlineData(5, Open, 3, Closed, 5)]
	[InlineData(5, Open, double.NegativeInfinity, Closed, 5)]
	[InlineData(double.PositiveInfinity, Closed, 3, Open, double.PositiveInfinity)]
	public void Max_Doubles(
		double expected,
		IntervalBoundary left, double min, IntervalBoundary right, double max
	) {
		Assert.Equal(expected, new Interval<double>(left, min, right, max).Max);
	}

	[Fact]
	public void Max_Integers_Invalid() {
		Assert.Throws<InvalidOperationException>(() => Interval<int>.Empty.Max);
	}

	[Fact]
	public void Max_Doubles_Invalid() {
		Assert.Throws<InvalidOperationException>(() => Interval<double>.Empty.Max);
	}

	[Theory]
	[InlineData(Closed, 3, Closed, 5)]
	[InlineData(Closed, 3, Open, 5)]
	public void IsLeftClosed_Integers_True(
		IntervalBoundary left, int min, IntervalBoundary right, int max
	) {
		Assert.True(new Interval<int>(left, min, right, max).IsLeftClosed);
	}

	[Theory]
	[InlineData(Closed, 3, Closed, 5)]
	[InlineData(Closed, 3, Open, 5)]
	[InlineData(Closed, 3, Open, double.PositiveInfinity)]
	public void IsLeftClosed_Doubles_True(
		IntervalBoundary left, double min, IntervalBoundary right, double max
	) {
		Assert.True(new Interval<double>(left, min, right, max).IsLeftClosed);
	}

	[Theory]
	[InlineData(Open, 0, Open, 0)]
	[InlineData(Open, 3, Open, 5)]
	[InlineData(Open, 3, Closed, 5)]
	public void IsLeftClosed_Integers_False(
		IntervalBoundary left, int min, IntervalBoundary right, int max
	) {
		Assert.False(new Interval<int>(left, min, right, max).IsLeftClosed);
	}

	[Theory]
	[InlineData(Open, 0, Open, 0)]
	[InlineData(Open, 3, Open, 5)]
	[InlineData(Open, double.NegativeInfinity, Open, double.PositiveInfinity)]
	[InlineData(Open, 3, Closed, 5)]
	[InlineData(Open, double.NegativeInfinity, Closed, 5)]
	public void IsLeftClosed_Doubles_False(
		IntervalBoundary left, double min, IntervalBoundary right, double max
	) {
		Assert.False(new Interval<double>(left, min, right, max).IsLeftClosed);
	}

	[Theory]
	[InlineData(Closed, 3, Closed, 5)]
	[InlineData(Open, 3, Closed, 5)]
	public void IsRightClosed_Integers_True(
		IntervalBoundary left, int min, IntervalBoundary right, int max
	) {
		Assert.True(new Interval<int>(left, min, right, max).IsRightClosed);
	}

	[Theory]
	[InlineData(Closed, 3, Closed, 5)]
	[InlineData(Open, 3, Closed, 5)]
	[InlineData(Open, double.NegativeInfinity, Closed, 5)]
	public void IsRightClosed_Doubles_True(
		IntervalBoundary left, double min, IntervalBoundary right, double max
	) {
		Assert.True(new Interval<double>(left, min, right, max).IsRightClosed);
	}

	[Theory]
	[InlineData(Open, 0, Open, 0)]
	[InlineData(Open, 3, Open, 5)]
	[InlineData(Closed, 3, Open, 5)]
	public void IsRightClosed_Integers_False(
		IntervalBoundary left, int min, IntervalBoundary right, int max
	) {
		Assert.False(new Interval<int>(left, min, right, max).IsRightClosed);
	}

	[Theory]
	[InlineData(Open, 0, Open, 0)]
	[InlineData(Open, 3, Open, 5)]
	[InlineData(Open, double.NegativeInfinity, Open, double.PositiveInfinity)]
	[InlineData(Closed, 3, Open, 5)]
	[InlineData(Closed, 3, Open, double.PositiveInfinity)]
	public void IsRightClosed_Doubles_False(
		IntervalBoundary left, double min, IntervalBoundary right, double max
	) {
		Assert.False(new Interval<double>(left, min, right, max).IsRightClosed);
	}

	[Theory]
	[InlineData(Open, 3, Open, 5)]
	public void IsOpen_Integers_True(
		IntervalBoundary left, int min, IntervalBoundary right, int max
	) {
		Assert.True(new Interval<int>(left, min, right, max).IsOpen);
	}

	[Theory]
	[InlineData(Open, 3, Open, 5)]
	[InlineData(Open, double.NegativeInfinity, Open, double.PositiveInfinity)]
	public void IsOpen_Doubles_True(
		IntervalBoundary left, double min, IntervalBoundary right, double max
	) {
		Assert.True(new Interval<double>(left, min, right, max).IsOpen);
	}

	[Theory]
	[InlineData(Closed, 3, Closed, 5)]
	[InlineData(Closed, 3, Open, 5)]
	[InlineData(Open, 3, Closed, 5)]
	public void IsOpen_Integers_False(
		IntervalBoundary left, int min, IntervalBoundary right, int max
	) {
		Assert.False(new Interval<int>(left, min, right, max).IsOpen);
	}

	[Theory]
	[InlineData(Closed, 3, Closed, 5)]
	[InlineData(Closed, 3, Open, 5)]
	[InlineData(Open, 3, Closed, 5)]
	[InlineData(Open, double.NegativeInfinity, Closed, 5)]
	[InlineData(Closed, 3, Open, double.PositiveInfinity)]
	public void IsOpenDoubles_False(
		IntervalBoundary left, double min, IntervalBoundary right, double max
	) {
		Assert.False(new Interval<double>(left, min, right, max).IsOpen);
	}

	[Theory]
	[InlineData(Closed, 3, Closed, 5)]
	public void IsClosed_Integers_True(
		IntervalBoundary left, int min, IntervalBoundary right, int max
	) {
		Assert.True(new Interval<int>(left, min, right, max).IsClosed);
	}

	[Theory]
	[InlineData(Closed, 3, Closed, 5)]
	public void IsClosed_Doubles_True(
		IntervalBoundary left, double min, IntervalBoundary right, double max
	) {
		Assert.True(new Interval<double>(left, min, right, max).IsClosed);
	}

	[Theory]
	[InlineData(Open, 0, Open, 0)]
	[InlineData(Open, 3, Open, 5)]
	[InlineData(Closed, 3, Open, 5)]
	[InlineData(Open, 3, Closed, 5)]
	public void IsClosed_Integers_False(
		IntervalBoundary left, int min, IntervalBoundary right, int max
	) {
		Assert.False(new Interval<int>(left, min, right, max).IsClosed);
	}

	[Theory]
	[InlineData(Open, 0, Open, 0)]
	[InlineData(Open, 3, Open, 5)]
	[InlineData(Open, double.NegativeInfinity, Open, double.PositiveInfinity)]
	[InlineData(Closed, 3, Open, 5)]
	[InlineData(Open, 3, Closed, 5)]
	[InlineData(Open, double.NegativeInfinity, Closed, 5)]
	[InlineData(Closed, 3, Open, double.PositiveInfinity)]
	public void IsClosed_Doubles_False(
		IntervalBoundary left, double min, IntervalBoundary right, double max
	) {
		Assert.False(new Interval<double>(left, min, right, max).IsClosed);
	}

	// TODO: test instance methods

	static void AssertInterval<N>(string expectedStr, Interval<N> actual) where N : INumber<N> {
		Assert.Equal(expectedStr, actual.ToString(null, CultureInfo.InvariantCulture));
	}

}
