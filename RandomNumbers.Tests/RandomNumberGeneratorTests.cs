namespace Ironclad.RandomNumbers.Tests;

public class RandomNumberGeneratorTests
{
  private static IEnumerable<object> SeedObjects()
  {
    yield return "some string";
    yield return true;
    yield return new object();
    yield return new { Name = "Horst", Number = 1337 };
    yield return (int[]) [1, 2, 3, 4, 5];
  }

  private static IEnumerable<Action<RandomNumberGenerator>> StateChangingActions()
  {
    yield return rng => rng.NextUInt();
    yield return rng => rng.NextFloat();
    yield return rng => rng.NextDouble();
  }
  
  private static IEnumerable<Func<uint, RandomNumberGenerator>> StateSettingConstructors()
  {
    yield return state => new RandomNumberGenerator(0xFU, state);
    yield return state => new RandomNumberGenerator(0xBEEF, state);
    yield return state => new RandomNumberGenerator(new {}, state);
  }
  
  [Test]
  public void DefaultSeedAndStateAreZero()
  {
    // ARRANGE
    var rng = new RandomNumberGenerator();
    
    // ASSERT
    Assert.Multiple(() =>
    {
      Assert.That(rng.Seed, Is.EqualTo(0));
      Assert.That(rng.State, Is.EqualTo(0));
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
    Assert.That(rng.Seed, Is.EqualTo(seed));
  }
  
  [TestCase(0, 0u)]
  [TestCase(-1, uint.MaxValue)]
  [TestCase(int.MinValue, 0x80000000)]
  public void ConstructorWithIntSeed_SeedIsSetCorrectly(int seed, uint expectedSeed)
  {
    // ACT
    var rng = new RandomNumberGenerator(seed);
    
    // ASSERT
    Assert.That(rng.Seed, Is.EqualTo(expectedSeed));
  }

  [TestCaseSource(nameof(SeedObjects))]
  public void ConstructorWithObjectSeed_SeedIsSetToHashCode(object obj)
  {
    // ACT
    var rng = new RandomNumberGenerator(obj);
    var expectedHashCode = unchecked((uint)obj.GetHashCode());
    
    // ASSERT
    Assert.That(rng.Seed, Is.EqualTo(expectedHashCode));
  }

  [Test]
  public void Constructor_NullSeed_SeedIsZero()
  {
    // ACT
    var rng = new RandomNumberGenerator(null);
    
    // ASSERT
    Assert.That(rng.Seed, Is.EqualTo(0));
  }

  [TestCaseSource(nameof(StateSettingConstructors))]
  public void StateSettingConstructor_StateIsSet(Func<uint, RandomNumberGenerator> constructorMethod)
  {
    // ARRANGE
    const uint state = 0xC0FFEE;
    
    // ACT
    var rngInt = constructorMethod(state);
    
    // ASSERT
    Assert.That(rngInt.State, Is.EqualTo(state));
  }

  [Test]
  public void NextMethod_StateIsIncreased(
    [ValueSource(nameof(StateChangingActions))] Action<RandomNumberGenerator> action, 
    [Values(0u, uint.MaxValue)] uint initialState)
  {
    // ARRANGE
    var rng = new RandomNumberGenerator(0xFU, initialState);
    
    // ACT
    action(rng);
    
    // ASSERT
    Assert.That(rng.State, Is.EqualTo(initialState + 1));
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
    Assert.That(result1, Is.EqualTo(result2));
  }
}