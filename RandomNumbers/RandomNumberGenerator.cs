namespace RandomNumbers;

/// <summary>
/// Generates random numbers using a noise generator and maintains a state.
/// </summary>
public class RandomNumberGenerator
{
  /// <summary>
  /// Gets or sets the state of the random number generator.
  /// </summary>
  /// <remarks>
  /// This property represents the current coordinate in the noise field used to generate random numbers. You can use it
  /// to jump in the list of generated values. 
  /// </remarks>
  public uint State { get; set; }
 
  private readonly Noise noise;
  
  /// <param name="seed">The seed value for the noise generator.</param>
  /// <param name="state">The initial state of the random number generator.</param>
  public RandomNumberGenerator(uint seed = 0, uint state = 0)
  {
    noise = new Noise(seed);
    State = state;
  }

  /// <summary>
  /// Generates the next random byte.
  /// </summary>
  public byte NextByte() => unchecked((byte)noise.Raw(State++));
  
  /// <summary>
  /// Generates the next sequence of random bytes.
  /// </summary>
  /// <returns>An array of 4 random byte values.</returns>
  public byte[] NextBytes() => BitConverter.GetBytes(noise.Raw(State++));

  /// <summary>
  /// Generates the next random integer.
  /// </summary>
  /// <param name="max">The upper bound (exclusive) of the random number.</param>
  public int NextInt(int max = int.MaxValue) => unchecked((int)(noise[State++] * max));
  
  /// <summary>
  /// Generates the next random integer within a specified range.
  /// </summary>
  /// <param name="min">The lower bound (inclusive) of the random number.</param>
  /// <param name="max">The upper bound (exclusive) of the random number.</param>
  public int NextInt(int min, int max) => (int)(noise[State++] * (max - min)) + min;
  
  /// <summary>
  /// Generates the next random unsigned integer.
  /// </summary>
  public uint NextUInt() => noise.Raw(State++);
  
  /// <summary>
  /// Generates the next random unsigned integer within a specified range.
  /// </summary>
  /// <param name="max">The upper bound (exclusive) of the random number.</param>
  public uint NextUInt(uint max) => (uint)(noise[State++] * max);
  
  /// <summary>
  /// Generates the next random unsigned integer within a specified range.
  /// </summary>
  /// <param name="min">The lower bound (inclusive) of the random number.</param>
  /// <param name="max">The upper bound (exclusive) of the random number.</param>
  public uint NextUInt(uint min, uint max) => (uint)(noise[State++] * (max - min)) + min;
 
  /// <summary>
  /// Generates the next random float.
  /// </summary>
  public float NextFloat() => noise[State++];
  
  /// <summary>
  /// Generates the next random float within a specified range.
  /// </summary>
  /// <param name="max">The upper bound (exclusive) of the random number.</param>
  public float NextFloat(float max) => noise[State++] * max;
  
  /// <summary>
  /// Generates the next random float within a specified range.
  /// </summary>
  /// <param name="min">The lower bound (inclusive) of the random number.</param>
  /// <param name="max">The upper bound (exclusive) of the random number.</param>
  public float NextFloat(float min, float max) => noise[State++] * (max - min) + min;
  
  /// <summary>
  /// Generates the next random double.
  /// </summary>
  public double NextDouble() => noise[State++];
  
  /// <summary>
  /// Generates the next random double within a specified range.
  /// </summary>
  /// <param name="max">The upper bound (exclusive) of the random number.</param>
  public double NextDouble(double max) => noise[State++] * max;
  
  /// <summary>
  /// Generates the next random double within a specified range.
  /// </summary>
  /// <param name="min">The lower bound (inclusive) of the random number.</param>
  /// <param name="max">The upper bound (exclusive) of the random number.</param>
  public double NextDouble(double min, double max) => noise[State++] * (max - min) + min;
  
  /// <summary>
  /// Generates the next random boolean value.
  /// </summary>
  public bool NextBool() => (noise.Raw(State++) & 1) == 1;
  
  /// <summary>
  /// Determines if the next random value is less than the specified probability (between 0 and 1).
  /// </summary>
  /// <param name="probability">The probability threshold for comparison. Should be between 0 and 1.</param>
  /// <returns>True if the random value is less than the probability, otherwise false.</returns>
  public bool NextChance(float probability) => noise[State++] < probability;
  
  /// <inheritdoc cref="NextChance(float)"/>
  public bool NextChance(double probability) => noise[State++] < probability;
}