namespace Ironclad.RandomNumbers.Analysis;

public class RandomNumberHistogramTest4Byte
{
  private const int PingAfter = 100_000_000;

  private readonly uint sampleCount;

  public readonly int[] Buckets;

  private readonly RandomNumberGenerator rng;

  public RandomNumberHistogramTest4Byte(uint seed, int buckets = 1000, uint samples = 1_000_000)
  {
    Buckets = new int[buckets];
    sampleCount = samples;
    rng = new RandomNumberGenerator(seed);
  }

  public void Run()
  {
    var sampleCounter = 0L;
    var pingCounter = 0L;
    while (sampleCounter++ < sampleCount)
    {
      Buckets[(int)(rng.NextFloat() * Buckets.Length)]++;
      if (++pingCounter < PingAfter) continue;
      pingCounter = 0;
      Console.WriteLine($"took {sampleCounter} samples");
    }

    Console.WriteLine($"DONE --- {sampleCount} samples into {Buckets.Length} buckets");
    rng.NextFloat();
  }

  public async Task SaveHistogram()
  {
    var content = $$$"""
                  <head>
                  	<script src='https://cdn.plot.ly/plotly-2.32.0.min.js'></script>
                  </head>

                  <body>
                    <p>Seed: {{{rng.Seed}}}</p>
                    <p>{{{sampleCount}}} sampels in {{{Buckets.Length}}} buckets</p>
                  	<div id='plot'></div>
                  </body>

                  <script>
                    Plotly.newPlot('plot', 
                      [{
                        y: [{{{string.Join(", ", Buckets)}}}],
                        type: 'bar'
                      }], 
                      { showlegend: false }, 
                      { staticPlot: true }
                    );
                  </script>
                  """;
    await File.WriteAllTextAsync("histogram.html", content);
  }
}