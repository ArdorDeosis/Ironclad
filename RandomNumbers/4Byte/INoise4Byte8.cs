namespace Ironclad.RandomNumbers;

/// <summary>
/// A 4-byte noise generator with up to 8 dimensions of input.
/// </summary>
public interface INoise4Byte8 : INoise4Byte7
{
  /// <summary>
  /// Gets the raw uint noise value of the specified 8-dimensional coordinates.
  /// </summary>
  uint Raw(uint d1, uint d2, uint d3, uint d4, uint d5, uint d6, uint d7, uint d8);

  /// <summary>
  /// Gets the raw uint noise value of the specified 8-dimensional coordinates.
  /// </summary>
  uint Raw(int d1, int d2, int d3, int d4, int d5, int d6, int d7, int d8);

  /// <summary>
  /// Gets the noise value of the specified 8-dimensional coordinates.
  /// </summary>
  float this[uint d1, uint d2, uint d3, uint d4, uint d5, uint d6, uint d7, uint d8] { get; }

  /// <summary>
  /// Gets the noise value of the specified 8-dimensional coordinates.
  /// </summary>
  float this[int d1, int d2, int d3, int d4, int d5, int d6, int d7, int d8] { get; }
}