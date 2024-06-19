namespace RandomNumbers;

/// <summary>
/// A 4-byte noise generator with up to 9 dimensions of input.
/// </summary>
public class Noise
{
  private readonly uint seed;

  private const uint Prime1 = 0xBD4BCB5u;
  private const uint Prime2 = 0x63D68Du;
  private const uint Prime3 = 0xBD7C9u;
  private const uint Prime4 = 0xAE23u;
  private const uint Prime5 = 0x5B5E1A3u;
  private const uint Prime6 = 0xE1A363u;
  private const uint Prime7 = 0xA5AB5u;
  private const uint Prime8 = 0x95E3u;

  /// <summary>
  /// Initializes a new instance with seed 0.
  /// </summary>
  public Noise() { }

  /// <summary>
  /// Initializes a new instance with the specified seed.
  /// </summary>
  public Noise(uint seed) => this.seed = seed;

  /// <summary>
  /// Initializes a new instance with the specified seed.
  /// </summary>
  public Noise(int seed) => this.seed = unchecked((uint)seed);

  /// <summary>
  /// Initializes a new instance with a seed derived from the hash code of the specified object.
  /// </summary>
  public Noise(object seed) => this.seed = unchecked((uint)seed.GetHashCode());
  
  /// <summary>
  /// Gets the raw uint noise value of the specified 1-dimensional coordinate.
  /// </summary>
  public uint Raw(uint a) => Squirrel3(a);

  /// <summary>
  /// Gets the raw uint noise value of the specified 2-dimensional coordinates.
  /// </summary>
  public uint Raw(uint a, uint b) => Squirrel3(unchecked(a + b * Prime1));

  /// <summary>
  /// Gets the raw uint noise value of the specified 3-dimensional coordinates.
  /// </summary>
  public uint Raw(uint a, uint b, uint c) => Squirrel3(unchecked(a ^ b * Prime1 ^ c * Prime2));

  /// <summary>
  /// Gets the raw uint noise value of the specified 4-dimensional coordinates.
  /// </summary>
  public uint Raw(uint a, uint b, uint c, uint d) => Squirrel3(unchecked(a ^ b * Prime1 ^ c * Prime2 ^ d * Prime3));

  /// <summary>
  /// Gets the raw uint noise value of the specified 5-dimensional coordinates.
  /// </summary>
  public uint Raw(uint a, uint b, uint c, uint d, uint e) => Squirrel3(unchecked(a ^ b * Prime1 ^ c * Prime2 ^ d * Prime3 ^ e * Prime4));

  /// <summary>
  /// Gets the raw uint noise value of the specified 6-dimensional coordinates.
  /// </summary>
  public uint Raw(uint a, uint b, uint c, uint d, uint e, uint f) => Squirrel3(unchecked(a ^ b * Prime1 ^ c * Prime2 ^ d * Prime3 ^ e * Prime4 ^ f * Prime5));

  /// <summary>
  /// Gets the raw uint noise value of the specified 7-dimensional coordinates.
  /// </summary>
  public uint Raw(uint a, uint b, uint c, uint d, uint e, uint f, uint g) => Squirrel3(unchecked(a ^ b * Prime1 ^ c * Prime2 ^ d * Prime3 ^ e * Prime4 ^ f * Prime5 ^ g * Prime6));

  /// <summary>
  /// Gets the raw uint noise value of the specified 8-dimensional coordinates.
  /// </summary>
  public uint Raw(uint a, uint b, uint c, uint d, uint e, uint f, uint g, uint h) => Squirrel3(unchecked(a ^ b * Prime1 ^ c * Prime2 ^ d * Prime3 ^ e * Prime4 ^ f * Prime5 ^ g * Prime6 ^ h * Prime7));

  /// <summary>
  /// Gets the raw uint noise value of the specified 9-dimensional coordinates.
  /// </summary>
  public uint Raw(uint a, uint b, uint c, uint d, uint e, uint f, uint g, uint h, uint i) => Squirrel3(unchecked(a ^ b * Prime1 ^ c * Prime2 ^ d * Prime3 ^ e * Prime4 ^ f * Prime5 ^ g * Prime6 ^ h * Prime7 ^ i * Prime8));

  /// <summary>
  /// Gets the noise value of the specified 1-dimensional coordinate.
  /// </summary>
  public float this[uint a] => Raw(a) / (float)uint.MaxValue;

  /// <summary>
  /// Gets the noise value of the specified 2-dimensional coordinates.
  /// </summary>
  public float this[uint a, uint b] => Raw(unchecked(a + b * Prime1)) / (float)uint.MaxValue;

  /// <summary>
  /// Gets the noise value of the specified 3-dimensional coordinates.
  /// </summary>
  public float this[uint a, uint b, uint c] => Raw(unchecked(a ^ b * Prime1 ^ c * Prime2)) / (float)uint.MaxValue;

  /// <summary>
  /// Gets the noise value of the specified 4-dimensional coordinates.
  /// </summary>
  public float this[uint a, uint b, uint c, uint d] => Raw(unchecked(a ^ b * Prime1 ^ c * Prime2 ^ d * Prime3)) / (float)uint.MaxValue;

  /// <summary>
  /// Gets the noise value of the specified 5-dimensional coordinates.
  /// </summary>
  public float this[uint a, uint b, uint c, uint d, uint e] => Raw(unchecked(a ^ b * Prime1 ^ c * Prime2 ^ d * Prime3 ^ e * Prime4)) / (float)uint.MaxValue;

  /// <summary>
  /// Gets the noise value of the specified 6-dimensional coordinates.
  /// </summary>
  public float this[uint a, uint b, uint c, uint d, uint e, uint f] => Raw(unchecked(a ^ b * Prime1 ^ c * Prime2 ^ d * Prime3 ^ e * Prime4 ^ f * Prime5)) / (float)uint.MaxValue;

  /// <summary>
  /// Gets the noise value of the specified 7-dimensional coordinates.
  /// </summary>
  public float this[uint a, uint b, uint c, uint d, uint e, uint f, uint g] => Raw(unchecked(a ^ b * Prime1 ^ c * Prime2 ^ d * Prime3 ^ e * Prime4 ^ f * Prime5 ^ g * Prime6)) / (float)uint.MaxValue;

  /// <summary>
  /// Gets the noise value of the specified 8-dimensional coordinates.
  /// </summary>
  public float this[uint a, uint b, uint c, uint d, uint e, uint f, uint g, uint h] => Raw(unchecked(a ^ b * Prime1 ^ c * Prime2 ^ d * Prime3 ^ e * Prime4 ^ f * Prime5 ^ g * Prime6 ^ h * Prime7)) / (float)uint.MaxValue;

  /// <summary>
  /// Gets the noise value of the specified 9-dimensional coordinates.
  /// </summary>
  public float this[uint a, uint b, uint c, uint d, uint e, uint f, uint g, uint h, uint i] => Raw(unchecked(a ^ b * Prime1 ^ c * Prime2 ^ d * Prime3 ^ e * Prime4 ^ f * Prime5 ^ g * Prime6 ^ h * Prime7 ^ i * Prime8)) / (float)uint.MaxValue;
  
  /// <summary>
  /// Gets the noise value of the specified 1-dimensional coordinate.
  /// </summary>
  public float this[int a] => this[unchecked((uint)a)];

  /// <summary>
  /// Gets the noise value of the specified 2-dimensional coordinates.
  /// </summary>
  public float this[int a, int b] => this[unchecked((uint)a), unchecked((uint)b)];

  /// <summary>
  /// Gets the noise value of the specified 3-dimensional coordinates.
  /// </summary>
  public float this[int a, int b, int c] => this[unchecked((uint)a), unchecked((uint)b), unchecked((uint)c)];

  /// <summary>
  /// Gets the noise value of the specified 4-dimensional coordinates.
  /// </summary>
  public float this[int a, int b, int c, int d] => this[unchecked((uint)a), unchecked((uint)b), unchecked((uint)c), unchecked((uint)d)];

  /// <summary>
  /// Gets the noise value of the specified 5-dimensional coordinates.
  /// </summary>
  public float this[int a, int b, int c, int d, int e] => this[unchecked((uint)a), unchecked((uint)b), unchecked((uint)c), unchecked((uint)d), unchecked((uint)e)];

  /// <summary>
  /// Gets the noise value of the specified 6-dimensional coordinates.
  /// </summary>
  public float this[int a, int b, int c, int d, int e, int f] => this[unchecked((uint)a), unchecked((uint)b), unchecked((uint)c), unchecked((uint)d), unchecked((uint)e), unchecked((uint)f)];

  /// <summary>
  /// Gets the noise value of the specified 7-dimensional coordinates.
  /// </summary>
  public float this[int a, int b, int c, int d, int e, int f, int g] => this[unchecked((uint)a), unchecked((uint)b), unchecked((uint)c), unchecked((uint)d), unchecked((uint)e), unchecked((uint)f), unchecked((uint)g)];

  /// <summary>
  /// Gets the noise value of the specified 8-dimensional coordinates.
  /// </summary>
  public float this[int a, int b, int c, int d, int e, int f, int g, int h] => this[unchecked((uint)a), unchecked((uint)b), unchecked((uint)c), unchecked((uint)d), unchecked((uint)e), unchecked((uint)f), unchecked((uint)g), unchecked((uint)h)];

  /// <summary>
  /// Gets the noise value of the specified 9-dimensional coordinates.
  /// </summary>
  public float this[int a, int b, int c, int d, int e, int f, int g, int h, int i] => this[unchecked((uint)a), unchecked((uint)b), unchecked((uint)c), unchecked((uint)d), unchecked((uint)e), unchecked((uint)f), unchecked((uint)g), unchecked((uint)h), unchecked((uint)i)];

  /// <summary>
  /// Generates a noise value based on the specified input.
  /// </summary>
  /// <remarks>
  /// This algorithm was introduced by <b>Prof. Squirrel Eiserloh</b> at GDC 2017.<br/>
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
      mangled += seed;
      mangled ^= mangled >> 8;
      mangled += bitNoise2;
      mangled ^= mangled << 8;
      mangled *= bitNoise3;
      mangled ^= mangled >> 8;
      return mangled;
    }
  }
}