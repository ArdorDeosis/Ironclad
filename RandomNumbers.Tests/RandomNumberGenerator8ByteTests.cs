using FluentAssertions;

namespace Ironclad.RandomNumbers.Tests;

public class RandomNumberGenerator8ByteTests
{
  /// <summary>
  /// An assortment of different objects and the seed they are expected to result in.
  /// </summary>
  private static IEnumerable<(object seedObject, ulong expectedSeed)> SeedObjectsAndSeeds =>
    new[]
    {
      "some string",
      true,
      new object(),
      new { Name = "Horst", Number = 1337 },
      (int[]) [1, 2, 3, 4, 5],
    }.Select(obj => (obj, unchecked((ulong)obj.GetHashCode())));

  /// <summary>
  /// All constructors that create a RandomNumberGenerator8Byte with specific state.
  /// </summary>
  private static IEnumerable<Func<ulong, RandomNumberGenerator8Byte>> StateSettingConstructors =>
  [
    state => new RandomNumberGenerator8Byte(0xFU, state),
    state => new RandomNumberGenerator8Byte(0xBEEF, state),
    state => new RandomNumberGenerator8Byte(new { }, state),
  ];

  /// <summary>
  /// Actions on a RandomNumberGenerator8Byte that should increase its state when executed.
  /// </summary>
  private static IEnumerable<Action<RandomNumberGenerator8Byte>> StateChangingActions =>
  [
    rng => rng.NextULong(),
    rng => rng.NextDouble(),
  ];

  [Test]
  public void DefaultSeedAndStateAreZero()
  {
    // ARRANGE
    var rng = new RandomNumberGenerator8Byte();
    
    // ASSERT
    Assert.Multiple(() =>
    {
      rng.Seed.Should().Be(0);
      rng.State.Should().Be(0);
    });
  }
  
  [TestCase(0u)]
  [TestCase(0xBAD_FU)]
  [TestCase(ulong.MaxValue)]
  public void ConstructorWithUIntSeed_SeedIsSetCorrectly(ulong seed)
  {
    // ACT
    var rng = new RandomNumberGenerator8Byte(seed);
    
    // ASSERT
    rng.Seed.Should().Be(seed);
  }
  
  [TestCase(0, 0u)]
  [TestCase(-1, ulong.MaxValue)]
  [TestCase(int.MinValue, 0xFFFFFFFF80000000)]
  [TestCase(long.MinValue, 0x8000000000000000)]
  public void ConstructorWithIntSeed_SeedIsSetCorrectly(long seed, ulong expectedSeed)
  {
    // ACT
    var rng = new RandomNumberGenerator8Byte(seed);
    
    // ASSERT
    rng.Seed.Should().Be(expectedSeed);
  }

  [TestCaseSource(nameof(SeedObjectsAndSeeds))]
  public void ConstructorWithObjectSeed_SeedIsSetToHashCode((object seedObject, ulong expectedSeed) data)
  {
    // ACT
    var rng = new RandomNumberGenerator8Byte(data.seedObject);
    
    // ASSERT
    rng.Seed.Should().Be(data.expectedSeed);
  }

  [Test]
  public void Constructor_NullSeed_SeedIsZero()
  {
    // ACT
    var rng = new RandomNumberGenerator8Byte(null);
    
    // ASSERT
    rng.Seed.Should().Be(0);
  }

  [TestCaseSource(nameof(StateSettingConstructors))]
  public void StateSettingConstructor_StateIsSet(Func<ulong, RandomNumberGenerator8Byte> constructorMethod)
  {
    // ARRANGE
    const ulong state = 0xC0FFEE;
    
    // ACT
    var rngInt = constructorMethod(state);
    
    // ASSERT
    rngInt.State.Should().Be(state);
  }

  [Test]
  public void NextMethod_StateIsIncreased(
    [ValueSource(nameof(StateChangingActions))] Action<RandomNumberGenerator8Byte> action, 
    [Values(0u, ulong.MaxValue)] ulong initialState)
  {
    // ARRANGE
    var rng = new RandomNumberGenerator8Byte(0xFU, initialState);
    var expectedState = initialState + 1;
    
    // ACT
    action(rng);
    
    // ASSERT
    rng.State.Should().Be(expectedState);
  }
  
  [Test]
  public void SettingStateManually_DeterministicResult([Values(0u, 42u, 0xC0DEBEEF, ulong.MaxValue)] ulong state)
  {
    // ARRANGE
    var rng = new RandomNumberGenerator8Byte(0xFU, state);
    
    // ACT
    var result1 = rng.NextByte();
    rng.State = state;
    var result2 = rng.NextByte();

    // ASSERT
    result1.Should().Be(result2);
  }
}