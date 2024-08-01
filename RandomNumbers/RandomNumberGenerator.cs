using Ironclad.Noise;

namespace Ironclad.RandomNumbers;

/// <summary>
/// Generates random numbers using a noise generator and maintains a state.
/// </summary>
public class RandomNumberGenerator : IRandomNumberGenerator
{
  /// <summary>
  /// Gets the seed value used by the underlying noise generator.
  /// </summary>
  public uint Seed => noise.Seed;
  
  /// <inheritdoc />
  /// <remarks>
  /// This property represents the current coordinate in the noise field used to generate random numbers. You can use it
  /// to jump in the list of generated values. 
  /// </remarks>
  public uint State { get; set; }
 
  private readonly Noise4Byte noise;
  
  /// <summary>
  /// Initializes a random number generator with seed = 0 and state = 0.
  /// </summary>
  public RandomNumberGenerator()
  {
    noise = new Noise4Byte();
  }
  
  /// <param name="seed">The seed value for the noise generator.</param>
  /// <param name="state">The initial state of the random number generator.</param>
  public RandomNumberGenerator(uint seed, uint state = 0)
  {
    noise = new Noise4Byte(seed);
    State = state;
  }
  
  /// <param name="seed">The seed value for the noise generator.</param>
  /// <param name="state">The initial state of the random number generator.</param>
  public RandomNumberGenerator(int seed, uint state = 0)
  {
    noise = new Noise4Byte(seed);
    State = state;
  }
  
  /// <param name="seed">The seed value for the noise generator.</param>
  /// <param name="state">The initial state of the random number generator.</param>
  public RandomNumberGenerator(object? seed, uint state = 0)
  {
    noise = new Noise4Byte(seed);
    State = state;
  }
  
  /// <inheritdoc />
  public uint NextUInt() => noise.Raw(State++);
 
  /// <inheritdoc />
  public float NextFloat() => noise[State++];
  
  /// <inheritdoc />
  public double NextDouble() => noise[State++];
}