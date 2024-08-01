namespace Ironclad.RandomNumbers;

public static class RandomNumberGeneratorExtensions
{
  /// <summary>
  /// Generates the next random byte.
  /// </summary>
  /// <param name="rng">The random number generator instance.</param>
  public static byte NextByte(this IRandomNumberGenerator rng) => unchecked((byte)rng.NextUInt());
  
  /// <summary>
  /// Generates the next sequence of random bytes.
  /// </summary>
  /// <param name="rng">The random number generator instance.</param>
  /// <returns>An array of 4 random byte values.</returns>
  public static byte[] NextBytes(this IRandomNumberGenerator rng) => BitConverter.GetBytes(rng.NextUInt());

  /// <summary>
  /// Generates the next random integer.
  /// </summary>
  /// <param name="rng">The random number generator instance.</param>
  /// <param name="max">The upper bound (exclusive) of the random number.</param>
  public static int NextInt(this IRandomNumberGenerator rng, int max = int.MaxValue) => unchecked((int)(rng.NextFloat() * max));
  
  /// <summary>
  /// Generates the next random integer within a specified range.
  /// </summary>
  /// <param name="rng">The random number generator instance.</param>
  /// <param name="min">The lower bound (inclusive) of the random number.</param>
  /// <param name="max">The upper bound (exclusive) of the random number.</param>
  public static int NextInt(this IRandomNumberGenerator rng, int min, int max) => (int)(rng.NextFloat() * (max - min)) + min;
  
  /// <summary>
  /// Generates the next random unsigned integer within a specified range.
  /// </summary>
  /// <param name="rng">The random number generator instance.</param>
  /// <param name="max">The upper bound (exclusive) of the random number.</param>
  public static uint NextUInt(this IRandomNumberGenerator rng, uint max) => (uint)(rng.NextFloat() * max);
  
  /// <summary>
  /// Generates the next random unsigned integer within a specified range.
  /// </summary>
  /// <param name="rng">The random number generator instance.</param>
  /// <param name="min">The lower bound (inclusive) of the random number.</param>
  /// <param name="max">The upper bound (exclusive) of the random number.</param>
  public static uint NextUInt(this IRandomNumberGenerator rng, uint min, uint max) => (uint)(rng.NextFloat() * (max - min)) + min;
  
  /// <summary>
  /// Generates the next random float within a specified range.
  /// </summary>
  /// <param name="rng">The random number generator instance.</param>
  /// <param name="max">The upper bound (exclusive) of the random number.</param>
  public static float NextFloat(this IRandomNumberGenerator rng, float max) => rng.NextFloat() * max;
  
  /// <summary>
  /// Generates the next random float within a specified range.
  /// </summary>
  /// <param name="rng">The random number generator instance.</param>
  /// <param name="min">The lower bound (inclusive) of the random number.</param>
  /// <param name="max">The upper bound (exclusive) of the random number.</param>
  public static float NextFloat(this IRandomNumberGenerator rng, float min, float max) => rng.NextFloat() * (max - min) + min;

  
  /// <summary>
  /// Generates the next random double within a specified range.
  /// </summary>
  /// <param name="rng">The random number generator instance.</param>
  /// <param name="max">The upper bound (exclusive) of the random number.</param>
  public static double NextDouble(this IRandomNumberGenerator rng, double max) => rng.NextFloat() * max;
  
  /// <summary>
  /// Generates the next random double within a specified range.
  /// </summary>
  /// <param name="rng">The random number generator instance.</param>
  /// <param name="min">The lower bound (inclusive) of the random number.</param>
  /// <param name="max">The upper bound (exclusive) of the random number.</param>
  public static double NextDouble(this IRandomNumberGenerator rng, double min, double max) => rng.NextFloat() * (max - min) + min;
  
  /// <summary>
  /// Generates the next random boolean value.
  /// </summary>
  /// <param name="rng">The random number generator instance.</param>
  public static bool NextBool(this IRandomNumberGenerator rng) => (rng.NextUInt() & 1) == 1;
  
  /// <summary>
  /// Determines if the next random value is less than the specified probability (between 0 and 1).
  /// </summary>
  /// <param name="rng">The random number generator instance.</param>
  /// <param name="probability">The probability threshold for comparison. Should be between 0 and 1.</param>
  /// <returns>True if the random value is less than the probability, otherwise false.</returns>
  public static bool NextChance(this IRandomNumberGenerator rng, float probability) => rng.NextFloat() < probability;

  /// <inheritdoc cref="NextChance(Ironclad.RandomNumbers.IRandomNumberGenerator,float)" />
  public static bool NextChance(this IRandomNumberGenerator rng, double probability) => rng.NextFloat() < probability;
}