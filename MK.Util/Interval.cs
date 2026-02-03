using System.Numerics;

namespace MK.Util;

/// <summary>
/// The open/closed status of an interval boundary.
/// </summary>
public enum IntervalBoundary {
	Open,
	Closed
}

/// <summary>
/// A numeric interval.
/// </summary>
public readonly struct Interval<N> : IEquatable<Interval<N>>, IFormattable where N : INumber<N> {

	readonly record struct Rep(N Min, N Max, IntervalBoundary MinType, IntervalBoundary MaxType);

	readonly Rep? _rep = null;

	/// <summary>
	/// The empty interval.
	/// </summary>
	public static Interval<N> Empty => new(IntervalBoundary.Open, N.Zero, IntervalBoundary.Open, N.Zero);

	/// <summary>
	/// True iff the interval is empty.
	/// </summary>
	public bool IsEmpty => this._rep == null;

	/// <summary>
	/// The minimum value of the interval.
	/// </summary>
	/// <exception cref="InvalidOperationException">The interval is empty.</exception>
	public N Min {
		get {
			if (this._rep is Rep rep) {return rep.Min;}
			throw new InvalidOperationException($"Interval is empty");
		}
	}

	/// <summary>
	/// The maximum value of the interval.
	/// </summary>
	/// <exception cref="InvalidOperationException">The interval is empty.</exception>
	public N Max {
		get {
			if (this._rep is Rep rep) {return rep.Max;}
			throw new InvalidOperationException($"Interval is empty");
		}
	}

	/// <summary>
	/// True iff the interval's minimum value is included.
	/// </summary>
	public bool IsLeftClosed {
		get {
			if (this._rep is Rep rep) {
				return rep.MinType == IntervalBoundary.Closed;
			}
			return false;
		}
	}

	/// <summary>
	/// True iff the interval's maximum value is included.
	/// </summary>
	public bool IsRightClosed {
		get {
			if (this._rep is Rep rep) {
				return rep.MaxType == IntervalBoundary.Closed;
			}
			return false;
		}
	}

	/// <summary>
	/// True iff the interval is open (neither of the bounds are included).
	/// </summary>
	public bool IsOpen {
		get {
			if (this._rep is Rep rep) {
				return (rep.MinType == IntervalBoundary.Open) && (rep.MaxType == IntervalBoundary.Open);
			}
			return true;
		}
	}

	/// <summary>
	/// True iff the interval is closed (both bounds are included).
	/// </summary>
	public bool IsClosed {
		get {
			if (this._rep is Rep rep) {
				return (rep.MinType == IntervalBoundary.Closed) && (rep.MaxType == IntervalBoundary.Closed);
			}
			return false;
		}
	}

	/// <summary>
	/// Creates a new interval.
	/// </summary>
	/// <param name="left">The type of the left boundary.</param>
	/// <param name="min">The minimum value.</param>
	/// <param name="right">The type of the right boundary.</param>
	/// <param name="max">The maximum value.</param>
	/// <exception cref="ArgumentException">The arguments do not represent a valid interval.</exception>
	public Interval(IntervalBoundary left, N min, IntervalBoundary right, N max) {
		if (!(N.IsRealNumber(min) && N.IsRealNumber(max))) {
			throw new ArgumentException($"min and max must be real numbers");
		}
		if (min > max) {throw new ArgumentException($"min must be less than or equal to max");}
		if (min == max) {
			if ((left == IntervalBoundary.Open) && (right == IntervalBoundary.Open)) {
				return;
			} else if ((left == IntervalBoundary.Open) || (right == IntervalBoundary.Open)) {
				throw new ArgumentException($"Interval definition is contradictory");
			}
		}
		if (
			(N.IsInfinity(min) && (left == IntervalBoundary.Closed))
			|| (N.IsInfinity(max) && (right == IntervalBoundary.Closed))
		) {
			throw new ArgumentException($"Interval cannot include infinite bounds");
		}
		this._rep = new Rep(min, max, left, right);
	}

	/// <summary>
	/// Creates an open interval.
	/// </summary>
	/// <param name="min">The minimum value.</param>
	/// <param name="max">The maximum value.</param>
	/// <returns>An open interval with the given bounds.</returns>
	public static Interval<N> Open(N min, N max) => new(IntervalBoundary.Open, min, IntervalBoundary.Open, max);

	/// <summary>
	/// Creates a closed interval.
	/// </summary>
	/// <param name="min">The minimum value.</param>
	/// <param name="max">The maximum value.</param>
	/// <returns>A closed interval with the given bounds.</returns>
	public static Interval<N> Closed(N min, N max) => new(IntervalBoundary.Closed, min, IntervalBoundary.Closed, max);

	/// <param name="n">A real (and not infinite) number.</param>
	/// <returns>true iff the interval contains n.</returns>
	public bool Contains(N n) {
		if (!(N.IsRealNumber(n) && N.IsFinite(n))) {return false;}
		if (this._rep is Rep rep) {
			if ((n < rep.Min) || (n > rep.Max)) {return false;}
			if ((rep.MinType == IntervalBoundary.Open) && (n == rep.Min)) {return false;}
			if ((rep.MaxType == IntervalBoundary.Open) && (n == rep.Max)) {return false;}
			return true;
		}
		return false;
	}

	public override bool Equals(object? obj) {
		if (obj is Interval<N> interval) {
			return this.Equals(interval);
		}
		return false;
	}

	public bool Equals(Interval<N> other) => this._rep == other._rep;

	public override int GetHashCode() => this._rep.GetHashCode();

	public static bool operator ==(Interval<N> left, Interval<N> right) => left.Equals(right);
	public static bool operator !=(Interval<N> left, Interval<N> right) => !left.Equals(right);

	/// <param name="other">An interval to compare with this interval.</param>
	/// <returns>true iff the two intervals intersect.</returns>
	public bool Intersects(Interval<N> other) {
		if ((this._rep is Rep rep) && (other._rep is Rep otherRep)) {
			if (rep.Max < otherRep.Min) {return false;}
			if (rep.Min > otherRep.Max) {return false;}
			if (
				(rep.Max == otherRep.Min)
				&& ((rep.MaxType == IntervalBoundary.Open) || (otherRep.MinType == IntervalBoundary.Open))
			) {return false;}
			if (
				(rep.Min == otherRep.Max)
				&& ((rep.MinType == IntervalBoundary.Open) || (otherRep.MaxType == IntervalBoundary.Open))
			) {return false;}
			return true;
		}
		return false;
	}

	/// <param name="other">An interval to compare with this interval.</param>
	/// <returns>true iff this interval entirely contains the other interval.</returns>
	public bool IsSubsetOrEqual(Interval<N> other) {
		if (other._rep is Rep otherRep) {
			if (this._rep is Rep rep) {
				if (rep.Min > otherRep.Min) {return false;}
				if (rep.Max < otherRep.Max) {return false;}
				if (
					(rep.Min == otherRep.Min)
					&& (rep.MinType == IntervalBoundary.Open) && (otherRep.MinType == IntervalBoundary.Closed)
				) {return false;}
				if (
					(rep.Max == otherRep.Max)
					&& (rep.MaxType == IntervalBoundary.Open) && (otherRep.MaxType == IntervalBoundary.Closed)
				) {return false;}
				return true;
			}
			return false;
		}
		return true;
	}

	/// <param name="other">An interval to compare with this interval.</param>
	/// <returns>true iff this interval entirely contains the other interval and is not equal to it.</returns>
	public bool IsStrictSubset(Interval<N> other) {
		return (this != other) && this.IsSubsetOrEqual(other);
	}

	/// <summary>
	/// Returns a string that represents the current value.
	/// </summary>
	/// <returns>A string that represents the current value.</returns>
	public override string ToString() => this.ToString(null, null);

	public string ToString(string? format, IFormatProvider? formatProvider) {
		if (this._rep is Rep rep) {
			char leftBracket = (rep.MinType == IntervalBoundary.Closed) ? '[' : '(';
			char rightBracket = (rep.MaxType == IntervalBoundary.Closed) ? ']' : ')';
			string minStr = rep.Min.ToString(format, formatProvider);
			rep.Min.ToString();
			string maxStr = rep.Max.ToString(format, formatProvider);
			return $"{leftBracket}{minStr}, {maxStr}{rightBracket}";
		}
		return "\u2205";
	}

}
