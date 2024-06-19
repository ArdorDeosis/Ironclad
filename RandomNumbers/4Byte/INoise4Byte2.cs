namespace Ironclad.RandomNumbers;

/// <summary>
/// A 4-byte noise generator with up to 2 dimensions of input.
/// </summary>
public interface INoise4Byte2 : INoise4Byte1
{
  /// <summary>
  /// Gets the raw uint noise value of the specified 2-dimensional coordinates.
  /// </summary>
  uint Raw(uint d1, uint d2);
  
  /// <summary>
  /// Gets the raw uint noise value of the specified 2-dimensional coordinates.
  /// </summary>
  uint Raw(int d1, int d2);

  /// <summary>
  /// Gets the noise value of the specified 2-dimensional coordinates.
  /// </summary>
  float this[uint d1, uint d2] { get; }

  /// <summary>
  /// Gets the noise value of the specified 2-dimensional coordinates.
  /// </summary>
  float this[int d1, int d2] { get; }
}