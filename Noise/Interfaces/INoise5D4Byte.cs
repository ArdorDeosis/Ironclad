﻿namespace Ironclad.Noise;

/// <summary>
/// A deterministic discrete 5-dimensional noise field with 4-byte values.
/// Values are retrieved as <tt>float</tt> in range [0,1) or as raw <tt>uint</tt> values.
/// </summary>
public interface INoise5D4Byte : INoise4D4Byte
{
  /// <summary>
  /// Gets the raw <tt>uint</tt> noise value of the specified 5-dimensional coordinates.
  /// </summary>
  uint Raw(uint d1, uint d2, uint d3, uint d4, uint d5);

  /// <summary>
  /// Gets the raw <tt>uint</tt> noise value of the specified 5-dimensional coordinates.
  /// </summary>
  uint Raw(int d1, int d2, int d3, int d4, int d5);

  /// <summary>
  /// Gets the noise value of the specified 5-dimensional coordinates. The value is in [0,1).
  /// </summary>
  float this[uint d1, uint d2, uint d3, uint d4, uint d5] { get; }

  /// <summary>
  /// Gets the noise value of the specified 5-dimensional coordinates. The value is in [0,1).
  /// </summary>
  float this[int d1, int d2, int d3, int d4, int d5] { get; }
}