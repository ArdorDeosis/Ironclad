namespace IronClad.Noise;

/// <summary>
/// A deterministic discrete noise field with up to 9 dimensions of input and 4-byte values.
/// Values are retrieved as <tt>float</tt> in range [0,1) or as raw <tt>uint</tt> values.
/// </summary>
public class Noise4Byte : INoise9D4Byte
{
  private const uint Prime1 = 0xBD4BCB5u;
  private const uint Prime2 = 0x63D68Du;
  private const uint Prime3 = 0xBD7C9u;
  private const uint Prime4 = 0xAE23u;
  private const uint Prime5 = 0x5B5E1A3u;
  private const uint Prime6 = 0xE1A363u;
  private const uint Prime7 = 0xA5AB5u;
  private const uint Prime8 = 0x95E3u;

  /// <summary>
  /// This noise functions seed.
  /// </summary>
  public uint Seed { get; }

  /// <summary>
  /// Initializes a new instance with seed 0.
  /// </summary>
  public Noise4Byte() { }

  /// <summary>
  /// Initializes a new instance with the specified seed.
  /// </summary>
  public Noise4Byte(uint seed) => Seed = seed;

  /// <summary>
  /// Initializes a new instance with the specified seed.
  /// </summary>
  public Noise4Byte(int seed) => Seed = unchecked((uint)seed);

  /// <summary>
  /// Initializes a new instance with a seed derived from the hash code of the specified object.
  /// </summary>
  public Noise4Byte(object? seed) => Seed = unchecked((uint)(seed?.GetHashCode() ?? 0));

  /// <inheritdoc />
  public uint Raw(uint d1) => Squirrel3(d1);

  /// <inheritdoc />
  public uint Raw(uint d1, uint d2) => Squirrel3(unchecked(d1 + d2 * Prime1));

  /// <inheritdoc />
  public uint Raw(uint d1, uint d2, uint d3) => Squirrel3(unchecked(d1 ^ d2 * Prime1 ^ d3 * Prime2));

  /// <inheritdoc />
  public uint Raw(uint d1, uint d2, uint d3, uint d4) =>
    Squirrel3(unchecked(d1 ^ d2 * Prime1 ^ d3 * Prime2 ^ d4 * Prime3));

  /// <inheritdoc />
  public uint Raw(uint d1, uint d2, uint d3, uint d4, uint d5) =>
    Squirrel3(unchecked(d1 ^ d2 * Prime1 ^ d3 * Prime2 ^ d4 * Prime3 ^ d5 * Prime4));

  /// <inheritdoc />
  public uint Raw(uint d1, uint d2, uint d3, uint d4, uint d5, uint d6) =>
    Squirrel3(unchecked(d1 ^ d2 * Prime1 ^ d3 * Prime2 ^ d4 * Prime3 ^ d5 * Prime4 ^ d6 * Prime5));

  /// <inheritdoc />
  public uint Raw(uint d1, uint d2, uint d3, uint d4, uint d5, uint d6, uint d7) => 
    Squirrel3(unchecked(d1 ^ d2 * Prime1 ^ d3 * Prime2 ^ d4 * Prime3 ^ d5 * Prime4 ^ d6 * Prime5 ^ d7 * Prime6));

  /// <inheritdoc />
  public uint Raw(uint d1, uint d2, uint d3, uint d4, uint d5, uint d6, uint d7, uint d8) => 
    Squirrel3(unchecked(d1 ^ d2 * Prime1 ^ d3 * Prime2 ^ d4 * Prime3 ^ d5 * Prime4 ^ d6 * Prime5 ^ d7 * Prime6 ^ d8 * Prime7));

  /// <inheritdoc />
  public uint Raw(uint d1, uint d2, uint d3, uint d4, uint d5, uint d6, uint d7, uint d8, uint d9) => 
    Squirrel3(unchecked(d1 ^ d2 * Prime1 ^ d3 * Prime2 ^ d4 * Prime3 ^ d5 * Prime4 ^ d6 * Prime5 ^ d7 * Prime6 ^ d8 * Prime7 ^ d9 * Prime8));
  
  /// <inheritdoc />
  public uint Raw(int d1) => Raw((uint)d1);

  /// <inheritdoc />
  public uint Raw(int d1, int d2) => unchecked(Raw((uint)d1, (uint)d2));

  /// <inheritdoc />
  public uint Raw(int d1, int d2, int d3) => unchecked(Raw((uint)d1, (uint)d2, (uint)d3));

  /// <inheritdoc />
  public uint Raw(int d1, int d2, int d3, int d4) => unchecked(Raw((uint)d1, (uint)d2, (uint)d3, (uint)d4));

  /// <inheritdoc />
  public uint Raw(int d1, int d2, int d3, int d4, int d5) =>
    unchecked(Raw((uint)d1, (uint)d2, (uint)d3, (uint)d4, (uint)d5));

  /// <inheritdoc />
  public uint Raw(int d1, int d2, int d3, int d4, int d5, int d6) => 
    unchecked(Raw((uint)d1, (uint)d2, (uint)d3, (uint)d4, (uint)d5, (uint)d6));

  /// <inheritdoc />
  public uint Raw(int d1, int d2, int d3, int d4, int d5, int d6, int d7) => 
    unchecked(Raw((uint)d1, (uint)d2, (uint)d3, (uint)d4, (uint)d5, (uint)d6, (uint)d7));

  /// <inheritdoc />
  public uint Raw(int d1, int d2, int d3, int d4, int d5, int d6, int d7, int d8) => 
    unchecked(Raw((uint)d1, (uint)d2, (uint)d3, (uint)d4, (uint)d5, (uint)d6, (uint)d7, (uint)d8));

  /// <inheritdoc />
  public uint Raw(int d1, int d2, int d3, int d4, int d5, int d6, int d7, int d8, int d9) => 
    unchecked(Raw((uint)d1, (uint)d2, (uint)d3, (uint)d4, (uint)d5, (uint)d6, (uint)d7, (uint)d8, (uint)d9));

  /// <inheritdoc />
  public float this[uint d1] => Raw(d1).ToFloatIn01();

  /// <inheritdoc />
  public float this[uint d1, uint d2] => Raw(unchecked(d1 + d2 * Prime1)).ToFloatIn01();

  /// <inheritdoc />
  public float this[uint d1, uint d2, uint d3] => Raw(unchecked(d1 ^ d2 * Prime1 ^ d3 * Prime2)).ToFloatIn01();

  /// <inheritdoc />
  public float this[uint d1, uint d2, uint d3, uint d4] =>
    Raw(unchecked(d1 ^ d2 * Prime1 ^ d3 * Prime2 ^ d4 * Prime3)).ToFloatIn01();

  /// <inheritdoc />
  public float this[uint d1, uint d2, uint d3, uint d4, uint d5] =>
    Raw(unchecked(d1 ^ d2 * Prime1 ^ d3 * Prime2 ^ d4 * Prime3 ^ d5 * Prime4)).ToFloatIn01();

  /// <inheritdoc />
  public float this[uint d1, uint d2, uint d3, uint d4, uint d5, uint d6] =>
    Raw(unchecked(d1 ^ d2 * Prime1 ^ d3 * Prime2 ^ d4 * Prime3 ^ d5 * Prime4 ^ d6 * Prime5)).ToFloatIn01();

  /// <inheritdoc />
  public float this[uint d1, uint d2, uint d3, uint d4, uint d5, uint d6, uint d7] =>
    Raw(unchecked(d1 ^ d2 * Prime1 ^ d3 * Prime2 ^ d4 * Prime3 ^ d5 * Prime4 ^ d6 * Prime5 ^ d7 * Prime6)).ToFloatIn01();

  /// <inheritdoc />
  public float this[uint d1, uint d2, uint d3, uint d4, uint d5, uint d6, uint d7, uint d8] =>
    Raw(unchecked(d1 ^ d2 * Prime1 ^ d3 * Prime2 ^ d4 * Prime3 ^ d5 * Prime4 ^ d6 * Prime5 ^ d7 * Prime6 ^ d8 * Prime7)).ToFloatIn01();

  /// <inheritdoc />
  public float this[uint d1, uint d2, uint d3, uint d4, uint d5, uint d6, uint d7, uint d8, uint d9] =>
    Raw(unchecked(d1 ^ d2 * Prime1 ^ d3 * Prime2 ^ d4 * Prime3 ^ d5 * Prime4 ^ d6 * Prime5 ^ d7 * Prime6 ^ d8 * Prime7 ^ d9 * Prime8)).ToFloatIn01();

  /// <inheritdoc />
  public float this[int d1] => this[(uint)d1];

  /// <inheritdoc />
  public float this[int d1, int d2] => unchecked(this[(uint)d1, (uint)d2]);

  /// <inheritdoc />
  public float this[int d1, int d2, int d3] => unchecked(this[(uint)d1, (uint)d2, (uint)d3]);

  /// <inheritdoc />
  public float this[int d1, int d2, int d3, int d4] => unchecked(this[(uint)d1, (uint)d2, (uint)d3, (uint)d4]);

  /// <inheritdoc />
  public float this[int d1, int d2, int d3, int d4, int d5] => 
    unchecked(this[(uint)d1, (uint)d2, (uint)d3, (uint)d4, (uint)d5]);

  /// <inheritdoc />
  public float this[int d1, int d2, int d3, int d4, int d5, int d6] => 
    unchecked(this[(uint)d1, (uint)d2, (uint)d3, (uint)d4, (uint)d5, (uint)d6]);

  /// <inheritdoc />
  public float this[int d1, int d2, int d3, int d4, int d5, int d6, int d7] => 
    unchecked(this[(uint)d1, (uint)d2, (uint)d3, (uint)d4, (uint)d5, (uint)d6, (uint)d7]);

  /// <inheritdoc />
  public float this[int d1, int d2, int d3, int d4, int d5, int d6, int d7, int d8] => 
    unchecked(this[(uint)d1, (uint)d2, (uint)d3, (uint)d4, (uint)d5, (uint)d6, (uint)d7, (uint)d8]);

  /// <inheritdoc />
  public float this[int d1, int d2, int d3, int d4, int d5, int d6, int d7, int d8, int d9] => 
    unchecked(this[(uint)d1, (uint)d2, (uint)d3, (uint)d4, (uint)d5, (uint)d6, (uint)d7, (uint)d8, (uint)d9]);

  /// <remarks>
  /// This algorithm was introduced by <b>Squirrel Eiserloh</b> at GDC 2017.<br/>
  /// https://www.youtube.com/watch?v=LWFzPP8ZbdU
  /// </remarks>
  private uint Squirrel3(uint x)
  {
    const uint bitNoise1 = 0xB5297A4D;
    const uint bitNoise2 = 0x68E31DA4;
    const uint bitNoise3 = 0x1B56C4E9;

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
  /// Converts the specified <tt>uint</tt> to a float in [0,1). 
  /// </summary>
  /// <remarks>
  /// The constant <tt>4.294968E+09f</tt> is slightly greater than <tt>uint.MaxValue</tt>. 
  /// Dividing a <tt>uint</tt> by this constant ensures that the result is always less than 1, providing a normalized
  /// value in the range [0, 1).
  /// </remarks>
  public static float ToFloatIn01(this uint x) => x / 4.294968E+09f;
}