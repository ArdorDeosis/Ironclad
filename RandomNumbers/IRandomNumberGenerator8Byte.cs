namespace Ironclad.RandomNumbers;

/// <summary>
/// A random number generator with a state counter with 8-byte values.
/// </summary>
public interface IRandomNumberGenerator8Byte
{
  /// <summary>
  /// Gets or sets the state of the random number generator.
  /// <para> 
  /// This property represents a counter that increments with each generated number, allowing you to track the sequence.
  /// You can set the state to a specific value to reproduce or jump to a different point in the sequence.
  /// </para>
  /// </summary>
  ulong State { get; set; }

  /// <summary>
  /// Generates the next random unsigned long integer.
  /// </summary>
  ulong NextULong();

  /// <summary>
  /// Generates the next random double.
  /// </summary>
  double NextDouble();

  /// <summary>
  /// Generates the next random float.
  /// </summary>
  float NextFloat();
}