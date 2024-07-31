namespace Ironclad.RandomNumbers;

public static class RandomNumberGenerator8ByteExtensions
{
  /// <summary>
  /// Generates the next random byte.
  /// </summary>
  /// <param name="rng">The random number generator instance.</param>
  public static byte NextByte(this IRandomNumberGenerator8Byte rng) => unchecked((byte)rng.NextULong());
  
  /// <summary>
  /// Generates the next sequence of random bytes.
  /// </summary>
  /// <param name="rng">The random number generator instance.</param>
  /// <returns>An array of 4 random byte values.</returns>
  public static byte[] NextBytes(this IRandomNumberGenerator8Byte rng) => BitConverter.GetBytes(rng.NextULong());

  /// <summary>
  /// Generates the next random integer.
  /// </summary>
  /// <param name="rng">The random number generator instance.</param>
  /// <param name="max">The upper bound (exclusive) of the random number.</param>
  public static long NextLong(this IRandomNumberGenerator8Byte rng, long max = long.MaxValue) => unchecked((long)(rng.NextDouble() * max));
  
  /// <summary>
  /// Generates the next random integer within a specified range.
  /// </summary>
  /// <param name="rng">The random number generator instance.</param>
  /// <param name="min">The lower bound (inclusive) of the random number.</param>
  /// <param name="max">The upper bound (exclusive) of the random number.</param>
  public static long NextLong(this IRandomNumberGenerator8Byte rng, long min, long max) => (long)(rng.NextDouble() * (max - min)) + min;
  
  /// <summary>
  /// Generates the next random unsigned integer within a specified range.
  /// </summary>
  /// <param name="rng">The random number generator instance.</param>
  /// <param name="max">The upper bound (exclusive) of the random number.</param>
  public static ulong NextULong(this IRandomNumberGenerator8Byte rng, ulong max) => (ulong)(rng.NextDouble() * max);
  
  /// <summary>
  /// Generates the next random unsigned integer within a specified range.
  /// </summary>
  /// <param name="rng">The random number generator instance.</param>
  /// <param name="min">The lower bound (inclusive) of the random number.</param>
  /// <param name="max">The upper bound (exclusive) of the random number.</param>
  public static ulong NextULong(this IRandomNumberGenerator8Byte rng, ulong min, ulong max) => (ulong)(rng.NextDouble() * (max - min)) + min;
  
  /// <summary>
  /// Generates the next random float within a specified range.
  /// </summary>
  /// <param name="rng">The random number generator instance.</param>
  /// <param name="max">The upper bound (exclusive) of the random number.</param>
  public static float NextFloat(this IRandomNumberGenerator8Byte rng, float max) => (float)rng.NextDouble() * max;
  
  /// <summary>
  /// Generates the next random float within a specified range.
  /// </summary>
  /// <param name="rng">The random number generator instance.</param>
  /// <param name="min">The lower bound (inclusive) of the random number.</param>
  /// <param name="max">The upper bound (exclusive) of the random number.</param>
  public static float NextFloat(this IRandomNumberGenerator8Byte rng, float min, float max) => (float)rng.NextDouble() * (max - min) + min;
  
  /// <summary>
  /// Generates the next random double within a specified range.
  /// </summary>
  /// <param name="rng">The random number generator instance.</param>
  /// <param name="max">The upper bound (exclusive) of the random number.</param>
  public static double NextDouble(this IRandomNumberGenerator8Byte rng, double max) => rng.NextDouble() * max;
  
  /// <summary>
  /// Generates the next random double within a specified range.
  /// </summary>
  /// <param name="rng">The random number generator instance.</param>
  /// <param name="min">The lower bound (inclusive) of the random number.</param>
  /// <param name="max">The upper bound (exclusive) of the random number.</param>
  public static double NextDouble(this IRandomNumberGenerator8Byte rng, double min, double max) => rng.NextDouble() * (max - min) + min;
  
  /// <summary>
  /// Generates the next random boolean value.
  /// </summary>
  /// <param name="rng">The random number generator instance.</param>
  public static bool NextBool(this IRandomNumberGenerator8Byte rng) => (rng.NextULong() & 1) == 1;
  
  /// <summary>
  /// Determines if the next random value is less than the specified probability (between 0 and 1).
  /// </summary>
  /// <param name="rng">The random number generator instance.</param>
  /// <param name="probability">The probability threshold for comparison. Should be between 0 and 1.</param>
  /// <returns>True if the random value is less than the probability, otherwise false.</returns>
  public static bool NextChance(this IRandomNumberGenerator8Byte rng, float probability) => rng.NextDouble() < probability;

  /// <inheritdoc cref="NextChance(Ironclad.RandomNumbers.IRandomNumberGenerator8Byte,float)" />
  public static bool NextChance(this IRandomNumberGenerator8Byte rng, double probability) => rng.NextDouble() < probability;
}