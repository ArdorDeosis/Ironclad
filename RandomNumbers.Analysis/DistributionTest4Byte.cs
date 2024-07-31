namespace Ironclad.RandomNumbers.Analysis;

internal sealed class DistributionTest4Byte : DistributionTest<float>
{
  private readonly RandomNumberGenerator rng;
  
  protected override string[] ParameterDescriptions { get; }

  protected override int BucketCount => 1000;
  protected override int SampleCount => 1_000_000;

  public DistributionTest4Byte(uint seed)
  {
    rng = new RandomNumberGenerator(seed);
    ParameterDescriptions = [
      "4 Byte Random Number Generator",
      $"seed {seed}",
      "1,000,000 samples in 1,000 buckets",
    ];
  }
  
  protected override float GetNextSample() => rng.NextFloat();
}