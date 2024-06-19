namespace Ironclad.RandomNumbers;

/// <summary>
/// A 4-byte noise generator with up to 1 dimension of input.
/// </summary>
public interface INoise4Byte1
{
  /// <summary>
  /// Gets the raw uint noise value of the specified 1-dimensional coordinate.
  /// </summary>
  uint Raw(uint d1);

  /// <summary>
  /// Gets the raw uint noise value of the specified 1-dimensional coordinate.
  /// </summary>
  uint Raw(int d1);

  /// <summary>
  /// Gets the noise value of the specified 1-dimensional coordinate.
  /// </summary>
  float this[uint d1] { get; }

  /// <summary>
  /// Gets the noise value of the specified 1-dimensional coordinate.
  /// </summary>
  float this[int d1] { get; }
}