namespace Ironclad.RandomNumbers.Analysis;

internal class ScatterData<TValue>
{
  public required string[] ParameterDescriptions { get; init; }
  
  private readonly IReadOnlyList<TValue> data;

  public ScatterData(IReadOnlyList<TValue> data)
  {
    this.data = data;
  }

  public (IEnumerable<TValue> x, IEnumerable<TValue> y) GetScatterData(int skip) =>
    skip switch
    {
      <= 0 => throw new Exception("skip less than 1 is not supported"),
      > 10 => throw new Exception("skip larger than 9 is not supported"),
      _ => (
        data.Where((_, index) => index % (skip * 2) < skip),
        data.Where((_, index) => index % (skip * 2) >= skip)
      ),
    };
}