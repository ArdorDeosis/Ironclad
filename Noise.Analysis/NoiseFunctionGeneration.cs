using System.Linq.Expressions;

namespace IronClad.Noise.Analysis;

internal static class NoiseFunctionGeneration
{
  public static Func<int, int, float> GenerateFunction(Noise4Byte noise, int dx, int dy)
  {
    var noiseIndexProperty = noise.GetType().GetProperty("Item", Enumerable.Repeat(typeof(int), 9).ToArray());

    var xParam = Expression.Parameter(typeof(int), "x");
    var yParam = Expression.Parameter(typeof(int), "y");

    var noiseParams = new Expression[9];
    for (var i = 0; i < 9; i++)
    {
      if (i == dx)
        noiseParams[i] = xParam;
      else if (i == dy)
        noiseParams[i] = yParam;
      else
        noiseParams[i] = Expression.Constant(0);
    }

    var noiseAccess = Expression.MakeIndex(
      Expression.Constant(noise),
      noiseIndexProperty,
      noiseParams);

    var lambda = Expression.Lambda<Func<int, int, float>>(noiseAccess, xParam, yParam);

    return lambda.Compile();
  }
}