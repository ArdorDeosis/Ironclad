namespace Ironclad.RandomNumbers.Analysis;

public readonly struct HistogramData
{
  public required string[] ParameterDescriptions { get; init; }
  public required int[] BucketValues { get; init; }
  public required double ChiSquare { get; init; }
  public required double KolmogorovSmirnov { get; init; }
}