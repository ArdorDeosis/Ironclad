using FluentAssertions;

namespace Ironclad.RandomNumbers.Tests;

public class RandomNumberGeneratorTests
{
  /// <summary>
  /// An assortment of different objects and the seed they are expected to result in.
  /// </summary>
  private static IEnumerable<(object seedObject, uint expectedSeed)> SeedObjectsAndSeeds =>
    new[]
    {
      "some string",
      true,
      new object(),
      new { Name = "Horst", Number = 1337 },
      (int[]) [1, 2, 3, 4, 5],
    }.Select(obj => (obj, unchecked((uint)obj.GetHashCode())));

  /// <summary>
  /// All constructors that create a RandomNumberGenerator with specific state.
  /// </summary>
  private static IEnumerable<Func<uint, RandomNumberGenerator>> StateSettingConstructors =>
  [
    state => new RandomNumberGenerator(0xFU, state),
    state => new RandomNumberGenerator(0xBEEF, state),
    state => new RandomNumberGenerator(new { }, state),
  ];

  /// <summary>
  /// Actions on a RandomNumberGenerator that should increase its state when executed.
  /// </summary>
  private static IEnumerable<Action<RandomNumberGenerator>> StateChangingActions =>
  [
    rng => rng.NextUInt(),
    rng => rng.NextFloat(),
  ];

  [Test]
  public void DefaultSeedAndStateAreZero()
  {
    // ARRANGE
    var rng = new RandomNumberGenerator();
    
    // ASSERT
    Assert.Multiple(() =>
    {
      rng.Seed.Should().Be(0);
      rng.State.Should().Be(0);
    });
  }
  
  [TestCase(0u)]
  [TestCase(0xBAD_FU)]
  [TestCase(uint.MaxValue)]
  public void ConstructorWithUIntSeed_SeedIsSetCorrectly(uint seed)
  {
    // ACT
    var rng = new RandomNumberGenerator(seed);
    
    // ASSERT
    rng.Seed.Should().Be(seed);
  }
  
  [TestCase(0, 0u)]
  [TestCase(-1, uint.MaxValue)]
  [TestCase(int.MinValue, 0x80000000)]
  public void ConstructorWithIntSeed_SeedIsSetCorrectly(int seed, uint expectedSeed)
  {
    // ACT
    var rng = new RandomNumberGenerator(seed);
    
    // ASSERT
    rng.Seed.Should().Be(expectedSeed);
  }

  [TestCaseSource(nameof(SeedObjectsAndSeeds))]
  public void ConstructorWithObjectSeed_SeedIsSetToHashCode((object seedObject, uint expectedSeed) data)
  {
    // ACT
    var rng = new RandomNumberGenerator(data.seedObject);
    
    // ASSERT
    rng.Seed.Should().Be(data.expectedSeed);
  }

  [Test]
  public void Constructor_NullSeed_SeedIsZero()
  {
    // ACT
    var rng = new RandomNumberGenerator(null);
    
    // ASSERT
    rng.Seed.Should().Be(0);
  }

  [TestCaseSource(nameof(StateSettingConstructors))]
  public void StateSettingConstructor_StateIsSet(Func<uint, RandomNumberGenerator> constructorMethod)
  {
    // ARRANGE
    const uint state = 0xC0FFEE;
    
    // ACT
    var rngInt = constructorMethod(state);
    
    // ASSERT
    rngInt.State.Should().Be(state);
  }

  [Test]
  public void NextMethod_StateIsIncreased(
    [ValueSource(nameof(StateChangingActions))] Action<RandomNumberGenerator> action, 
    [Values(0u, uint.MaxValue)] uint initialState)
  {
    // ARRANGE
    var rng = new RandomNumberGenerator(0xFU, initialState);
    var expectedState = initialState + 1;
    
    // ACT
    action(rng);
    
    // ASSERT
    rng.State.Should().Be(expectedState);
  }
  
  [Test]
  public void SettingStateManually_DeterministicResult([Values(0u, 42u, 0xC0DEBEEF, uint.MaxValue)] uint state)
  {
    // ARRANGE
    var rng = new RandomNumberGenerator(0xFU, state);
    
    // ACT
    var result1 = rng.NextByte();
    rng.State = state;
    var result2 = rng.NextByte();

    // ASSERT
    result1.Should().Be(result2);
  }
}