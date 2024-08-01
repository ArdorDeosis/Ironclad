using System.Text;

namespace Ironclad.RandomNumbers.Analysis;

internal static class RenderExtensions
{
  public static string ToHtmlPage(this IEnumerable<Func<string, (string body, string script)>> renderFragments)
  {
    const string head = "<head><script src='https://cdn.plot.ly/plotly-2.32.0.min.js'></script></head>";
    var body = new StringBuilder("<body>");
    var script = new StringBuilder("<script>");

    var id = 0;
    foreach (var renderFragment in renderFragments)
    {
      var (bodyFragment, scriptFragment) = renderFragment((id++).ToString());
      body.Append(bodyFragment);
      script.Append(scriptFragment);
    }

    body.AppendLine("</body>");
    script.AppendLine("</script>");

    var html = new StringBuilder();
    html.AppendLine(head);
    html.AppendLine(body.ToString());
    html.AppendLine(script.ToString());

    return html.ToString();
  }

  public static Func<string, (string body, string script)> ToRenderFragment(this HistogramData data) =>
    id =>
    {
      var body = new StringBuilder();
      body.Append("<p>");
      foreach (var description in data.ParameterDescriptions)
      {
        body.Append(description);
        body.Append("<br/>");
      }

      body.Append($"Chi-Square: {data.ChiSquare}");
      body.Append($"Kolmogorov-Smirnov: {data.KolmogorovSmirnov}");
      body.Append($"<div id='{id}'></div>");
      body.Append("</p>");

      var script =
          $$"""
            Plotly.newPlot('{{id}}', 
              [{
                y: [{{string.Join(", ", data.BucketValues)}}],
                type: 'bar'
              }], 
              { showlegend: false }, 
              { staticPlot: true }
            );
            """
        ;

      return (body.ToString(), script);
    };

  public static Func<string, (string body, string script)> ToRenderFragment<TValue>(this ScatterData<TValue> data) =>
    id =>
    {
      var body = new StringBuilder();
      var script = new StringBuilder();

      body.Append("<p>");
      foreach (var description in data.ParameterDescriptions)
      {
        body.Append(description);
        body.Append("<br/>");
      }

      body.Append("</p>");
      for (var skip = 1; skip < 10; skip++)
      {
        body.Append($"<span>skip {skip}</span>");
        body.Append($"<div id='{id}-{skip}'></div>");

        var (x, y) = data.GetScatterData(skip);
        script.Append(
          $$"""
            Plotly.newPlot('{{id}}-{{skip}}', 
              [{
                x: [{{string.Join(", ", x)}}],
                y: [{{string.Join(", ", y)}}],
                mode: 'markers',
                type: 'scatter'
              }], 
              { showlegend: false }, 
              { staticPlot: true }
            );
            """
        );
      }

      return (body.ToString(), script.ToString());
    };
}