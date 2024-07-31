using Ironclad.RandomNumbers.Analysis;


Func<string, (string body, string script)>[] renderFragments =
[
  new DistributionTest4Byte(0).Execute().ToRenderFragment(),
  new DistributionTest8Byte(0).Execute().ToRenderFragment(),
  new ScatterTest4Byte(0).Execute().ToRenderFragment(),
  new ScatterTest8Byte(0).Execute().ToRenderFragment(),
];

var html = renderFragments.ToHtmlPage();

await File.WriteAllTextAsync("samples.html", html);