using Ironclad.Noise;

namespace Ironclad.RandomNumbers;

/// <summary>
/// Generates random numbers using a noise generator and malongains a state.
/// </summary>
public class RandomNumberGenerator8Byte : IRandomNumberGenerator8Byte
{
  /// <summary>
  /// Gets the seed value used by the underlying noise generator.
  /// </summary>
  public ulong Seed => noise.Seed;
  
  /// <summary>
  /// Gets or sets the state of the random number generator.
  /// </summary>
  /// <remarks>
  /// This property represents the current coordinate in the noise field used to generate random numbers. You can use it
  /// to jump in the list of generated values. 
  /// </remarks>
  public ulong State { get; set; }
 
  private readonly Noise8Byte noise;
  
  /// <summary>
  /// Initializes a random number generator with seed = 0 and state = 0.
  /// </summary>
  public RandomNumberGenerator8Byte()
  {
    noise = new Noise8Byte();
  }
  
  /// <param name="seed">The seed value for the noise generator.</param>
  /// <param name="state">The initial state of the random number generator.</param>
  public RandomNumberGenerator8Byte(ulong seed, ulong state = 0)
  {
    noise = new Noise8Byte(seed);
    State = state;
  }
  
  /// <param name="seed">The seed value for the noise generator.</param>
  /// <param name="state">The initial state of the random number generator.</param>
  public RandomNumberGenerator8Byte(long seed, ulong state = 0)
  {
    noise = new Noise8Byte(seed);
    State = state;
  }
  
  /// <param name="seed">The seed value for the noise generator.</param>
  /// <param name="state">The initial state of the random number generator.</param>
  public RandomNumberGenerator8Byte(object? seed, ulong state = 0)
  {
    noise = new Noise8Byte(seed);
    State = state;
  }
  
  /// <inheritdoc />
  public ulong NextULong() => noise.Raw(State++);
 
  /// <inheritdoc />
  public double NextDouble() => noise[State++];
}