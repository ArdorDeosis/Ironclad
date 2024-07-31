namespace IronClad.Noise;

/// <summary>
/// A deterministic discrete noise field with up to 9 dimensions of input and 8-byte values.
/// Values are retrieved as <tt>double</tt> in range [0,1) or as raw <tt>ulong</tt> values.
/// </summary>
public class Noise8Byte : INoise9D8Byte
{
  private const ulong Prime1 = 0xD5B5C79CF7A1D765;
  private const ulong Prime2 = 0x97B5D4C1F2A7C9;
  private const ulong Prime3 = 0xF4A7B3E59E7D;
  private const ulong Prime4 = 0xC1B7D89E7B;
  private const ulong Prime5 = 0xA7E4B02B3B72A9C1;
  private const ulong Prime6 = 0xE4B3C2D7F1A9B3;
  private const ulong Prime7 = 0x75F3B8E7A4C9;
  private const ulong Prime8 = 0x97C4E5F2D3;

  /// <summary>
  /// This noise functions seed.
  /// </summary>
  public ulong Seed { get; }

  /// <summary>
  /// Initializes a new instance with seed 0.
  /// </summary>
  public Noise8Byte() { }

  /// <summary>
  /// Initializes a new instance with the specified seed.
  /// </summary>
  public Noise8Byte(ulong seed) => Seed = seed;

  /// <summary>
  /// Initializes a new instance with the specified seed.
  /// </summary>
  public Noise8Byte(long seed) => Seed = unchecked((ulong)seed);

  /// <summary>
  /// Initializes a new instance with a seed derived from the hash code of the specified object.
  /// </summary>
  public Noise8Byte(object? seed) => Seed = unchecked((ulong)(seed?.GetHashCode() ?? 0));

  /// <inheritdoc />
  public ulong Raw(ulong d1) => Squirrel3(d1);

  /// <inheritdoc />
  public ulong Raw(ulong d1, ulong d2) => Squirrel3(unchecked(d1 + d2 * Prime1));

  /// <inheritdoc />
  public ulong Raw(ulong d1, ulong d2, ulong d3) => Squirrel3(unchecked(d1 ^ d2 * Prime1 ^ d3 * Prime2));

  /// <inheritdoc />
  public ulong Raw(ulong d1, ulong d2, ulong d3, ulong d4) =>
    Squirrel3(unchecked(d1 ^ d2 * Prime1 ^ d3 * Prime2 ^ d4 * Prime3));

  /// <inheritdoc />
  public ulong Raw(ulong d1, ulong d2, ulong d3, ulong d4, ulong d5) =>
    Squirrel3(unchecked(d1 ^ d2 * Prime1 ^ d3 * Prime2 ^ d4 * Prime3 ^ d5 * Prime4));

  /// <inheritdoc />
  public ulong Raw(ulong d1, ulong d2, ulong d3, ulong d4, ulong d5, ulong d6) =>
    Squirrel3(unchecked(d1 ^ d2 * Prime1 ^ d3 * Prime2 ^ d4 * Prime3 ^ d5 * Prime4 ^ d6 * Prime5));

  /// <inheritdoc />
  public ulong Raw(ulong d1, ulong d2, ulong d3, ulong d4, ulong d5, ulong d6, ulong d7) => 
    Squirrel3(unchecked(d1 ^ d2 * Prime1 ^ d3 * Prime2 ^ d4 * Prime3 ^ d5 * Prime4 ^ d6 * Prime5 ^ d7 * Prime6));

  /// <inheritdoc />
  public ulong Raw(ulong d1, ulong d2, ulong d3, ulong d4, ulong d5, ulong d6, ulong d7, ulong d8) => 
    Squirrel3(unchecked(d1 ^ d2 * Prime1 ^ d3 * Prime2 ^ d4 * Prime3 ^ d5 * Prime4 ^ d6 * Prime5 ^ d7 * Prime6 ^ d8 * Prime7));

  /// <inheritdoc />
  public ulong Raw(ulong d1, ulong d2, ulong d3, ulong d4, ulong d5, ulong d6, ulong d7, ulong d8, ulong d9) =>
    Squirrel3(unchecked(d1 ^ d2 * Prime1 ^ d3 * Prime2 ^ d4 * Prime3 ^ d5 * Prime4 ^ d6 * Prime5 ^ d7 * Prime6 ^ d8 * Prime7 ^ d9 * Prime8));


  /// <inheritdoc />
  public ulong Raw(long d1) => unchecked(Raw((ulong)d1));

  /// <inheritdoc />
  public ulong Raw(long d1, long d2) => unchecked(Raw((ulong)d1, (ulong)d2));

  /// <inheritdoc />
  public ulong Raw(long d1, long d2, long d3) => unchecked(Raw((ulong)d1, (ulong)d2, (ulong)d3));

  /// <inheritdoc />
  public ulong Raw(long d1, long d2, long d3, long d4) => unchecked(Raw((ulong)d1, (ulong)d2, (ulong)d3, (ulong)d4));

  /// <inheritdoc />
  public ulong Raw(long d1, long d2, long d3, long d4, long d5) =>
    unchecked(Raw((ulong)d1, (ulong)d2, (ulong)d3, (ulong)d4, (ulong)d5));

  /// <inheritdoc />
  public ulong Raw(long d1, long d2, long d3, long d4, long d5, long d6) =>
    unchecked(Raw((ulong)d1, (ulong)d2, (ulong)d3, (ulong)d4, (ulong)d5, (ulong)d6));

  /// <inheritdoc />
  public ulong Raw(long d1, long d2, long d3, long d4, long d5, long d6, long d7) =>
    unchecked(Raw((ulong)d1, (ulong)d2, (ulong)d3, (ulong)d4, (ulong)d5, (ulong)d6, (ulong)d7));

  /// <inheritdoc />
  public ulong Raw(long d1, long d2, long d3, long d4, long d5, long d6, long d7, long d8) =>
    unchecked(Raw((ulong)d1, (ulong)d2, (ulong)d3, (ulong)d4, (ulong)d5, (ulong)d6, (ulong)d7, (ulong)d8));

  /// <inheritdoc />
  public ulong Raw(long d1, long d2, long d3, long d4, long d5, long d6, long d7, long d8, long d9) =>
    unchecked(Raw((ulong)d1, (ulong)d2, (ulong)d3, (ulong)d4, (ulong)d5, (ulong)d6, (ulong)d7, (ulong)d8, (ulong)d9));


  /// <inheritdoc />
  public double this[ulong d1] => Raw(d1).ToDoubleIn01();

  /// <inheritdoc />
  public double this[ulong d1, ulong d2] => Raw(unchecked(d1 + d2 * Prime1)).ToDoubleIn01();

  /// <inheritdoc />
  public double this[ulong d1, ulong d2, ulong d3] => Raw(unchecked(d1 ^ d2 * Prime1 ^ d3 * Prime2)).ToDoubleIn01();

  /// <inheritdoc />
  public double this[ulong d1, ulong d2, ulong d3, ulong d4] =>
    Raw(unchecked(d1 ^ d2 * Prime1 ^ d3 * Prime2 ^ d4 * Prime3)).ToDoubleIn01();

  /// <inheritdoc />
  public double this[ulong d1, ulong d2, ulong d3, ulong d4, ulong d5] =>
    Raw(unchecked(d1 ^ d2 * Prime1 ^ d3 * Prime2 ^ d4 * Prime3 ^ d5 * Prime4)).ToDoubleIn01();

  /// <inheritdoc />
  public double this[ulong d1, ulong d2, ulong d3, ulong d4, ulong d5, ulong d6] =>
    Raw(unchecked(d1 ^ d2 * Prime1 ^ d3 * Prime2 ^ d4 * Prime3 ^ d5 * Prime4 ^ d6 * Prime5)).ToDoubleIn01();

  /// <inheritdoc />
  public double this[ulong d1, ulong d2, ulong d3, ulong d4, ulong d5, ulong d6, ulong d7] =>
    Raw(unchecked(d1 ^ d2 * Prime1 ^ d3 * Prime2 ^ d4 * Prime3 ^ d5 * Prime4 ^ d6 * Prime5 ^ d7 * Prime6))
      .ToDoubleIn01();

  /// <inheritdoc />
  public double this[ulong d1, ulong d2, ulong d3, ulong d4, ulong d5, ulong d6, ulong d7, ulong d8] =>
    Raw(unchecked(d1 ^ d2 * Prime1 ^ d3 * Prime2 ^ d4 * Prime3 ^ d5 * Prime4 ^ d6 * Prime5 ^ d7 * Prime6 ^ d8 * Prime7)).ToDoubleIn01();

  /// <inheritdoc />
  public double this[ulong d1, ulong d2, ulong d3, ulong d4, ulong d5, ulong d6, ulong d7, ulong d8, ulong d9] =>
    Raw(unchecked(d1 ^ d2 * Prime1 ^ d3 * Prime2 ^ d4 * Prime3 ^ d5 * Prime4 ^ d6 * Prime5 ^ d7 * Prime6 ^ d8 * Prime7 ^ d9 * Prime8)).ToDoubleIn01();

  /// <inheritdoc />
  public double this[long d1] => this[(ulong)d1];

  /// <inheritdoc />
  public double this[long d1, long d2] => unchecked(this[(ulong)d1, (ulong)d2]);

  /// <inheritdoc />
  public double this[long d1, long d2, long d3] => unchecked(this[(ulong)d1, (ulong)d2, (ulong)d3]);

  /// <inheritdoc />
  public double this[long d1, long d2, long d3, long d4] => unchecked(this[(ulong)d1, (ulong)d2, (ulong)d3, (ulong)d4]);

  /// <inheritdoc />
  public double this[long d1, long d2, long d3, long d4, long d5] =>
    unchecked(this[(ulong)d1, (ulong)d2, (ulong)d3, (ulong)d4, (ulong)d5]);

  /// <inheritdoc />
  public double this[long d1, long d2, long d3, long d4, long d5, long d6] =>
    unchecked(this[(ulong)d1, (ulong)d2, (ulong)d3, (ulong)d4, (ulong)d5, (ulong)d6]);

  /// <inheritdoc />
  public double this[long d1, long d2, long d3, long d4, long d5, long d6, long d7] =>
    unchecked(this[(ulong)d1, (ulong)d2, (ulong)d3, (ulong)d4, (ulong)d5, (ulong)d6, (ulong)d7]);

  /// <inheritdoc />
  public double this[long d1, long d2, long d3, long d4, long d5, long d6, long d7, long d8] =>
    unchecked(this[(ulong)d1, (ulong)d2, (ulong)d3, (ulong)d4, (ulong)d5, (ulong)d6, (ulong)d7, (ulong)d8]);

  /// <inheritdoc />
  public double this[long d1, long d2, long d3, long d4, long d5, long d6, long d7, long d8, long d9] =>
    unchecked(this[(ulong)d1, (ulong)d2, (ulong)d3, (ulong)d4, (ulong)d5, (ulong)d6, (ulong)d7, (ulong)d8, (ulong)d9]);

  /// <remarks>
  /// This algorithm was introduced by <b>Squirrel Eiserloh</b> at GDC 2017.<br/>
  /// https://www.youtube.com/watch?v=LWFzPP8ZbdU
  /// </remarks>
  private ulong Squirrel3(ulong x)
  {
    const ulong bitNoise1 = 0xB5297A4D4B7A3C66;
    const ulong bitNoise2 = 0x68E31DA4927236B8;
    const ulong bitNoise3 = 0x1B56C4E9B25E94B5;

    unchecked
    {
      var mangled = x;
      mangled *= bitNoise1;
      mangled += Seed;
      mangled ^= mangled >> 8;
      mangled += bitNoise2;
      mangled ^= mangled << 8;
      mangled *= bitNoise3;
      mangled ^= mangled >> 8;
      return mangled;
    }
  }
}

file static class Extensions
{
  /// <summary>
  /// Converts the specified <tt>ulong</tt> to a double in the range [0, 1).
  /// </summary>
  /// <remarks>
  /// The constant <tt>1.8446744073709556E+19</tt> is slightly greater than <tt>ulong.MaxValue</tt>. 
  /// Dividing a <tt>ulong</tt> by this constant ensures that the result is always less than 1, providing a normalized
  /// value in the range [0, 1).
  /// </remarks>
  public static double ToDoubleIn01(this ulong x) => x / 1.8446744073709556E+19;
}