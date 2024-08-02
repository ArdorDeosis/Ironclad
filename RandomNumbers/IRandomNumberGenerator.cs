namespace Ironclad.RandomNumbers;

/// <summary>
/// A random number generator with a state counter.
/// </summary>
public interface IRandomNumberGenerator
{
  /// <summary>
  /// Gets or sets the state of the random number generator.
  /// <para> 
  /// This property represents a counter that increments with each generated number, allowing you to track the sequence.
  /// You can set the state to a specific value to reproduce or jump to a different point in the sequence.
  /// </para>
  /// </summary>
  uint State { get; set; }

  /// <summary>
  /// Generates the next random unsigned integer.
  /// </summary>
  uint NextUInt();

  /// <summary>
  /// Generates the next random float in [0,1).
  /// </summary>
  float NextFloat();
}