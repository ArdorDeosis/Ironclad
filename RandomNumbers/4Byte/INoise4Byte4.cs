namespace Ironclad.RandomNumbers;

/// <summary>
/// A 4-byte noise generator with up to 4 dimensions of input.
/// </summary>
public interface INoise4Byte4 : INoise4Byte3
{
  /// <summary>
  /// Gets the raw uint noise value of the specified 4-dimensional coordinates.
  /// </summary>
  uint Raw(uint d1, uint d2, uint d3, uint d4);

  /// <summary>
  /// Gets the raw uint noise value of the specified 4-dimensional coordinates.
  /// </summary>
  uint Raw(int d1, int d2, int d3, int d4);

  /// <summary>
  /// Gets the noise value of the specified 4-dimensional coordinates.
  /// </summary>
  float this[uint d1, uint d2, uint d3, uint d4] { get; }

  /// <summary>
  /// Gets the noise value of the specified 4-dimensional coordinates.
  /// </summary>
  float this[int d1, int d2, int d3, int d4] { get; }
}