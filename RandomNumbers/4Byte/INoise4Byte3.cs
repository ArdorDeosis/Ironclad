namespace Ironclad.RandomNumbers;

/// <summary>
/// A 4-byte noise generator with up to 3 dimensions of input.
/// </summary>
public interface INoise4Byte3 : INoise4Byte2
{
  /// <summary>
  /// Gets the raw uint noise value of the specified 3-dimensional coordinates.
  /// </summary>
  uint Raw(uint d1, uint d2, uint d3);

  /// <summary>
  /// Gets the raw uint noise value of the specified 3-dimensional coordinates.
  /// </summary>
  uint Raw(int d1, int d2, int d3);

  /// <summary>
  /// Gets the noise value of the specified 3-dimensional coordinates.
  /// </summary>
  float this[uint d1, uint d2, uint d3] { get; }

  /// <summary>
  /// Gets the noise value of the specified 3-dimensional coordinates.
  /// </summary>
  float this[int d1, int d2, int d3] { get; }
}