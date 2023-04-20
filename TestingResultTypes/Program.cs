// See https://aka.ms/new-console-template for more information

using TestingResultTypes;

var result = (Result<int, string>)42;
var errorResult = (Result<int, string>)"Error";

Console.WriteLine(result.Or(15675) * 2);

if (errorResult.IsError(out var error))
{
  Console.WriteLine(error);
  return;
}

var dict = new Dictionary<int, int>();

if(dict.ContainsKey(5))
  dict[5] = 5;
else
  dict.Add(5, 5);

var x = GetData().OrDefault;
var y = GetData();

var z = x + y;

int myInt = GetData().Or(3412567);


Console.WriteLine(errorResult.Or);


Result<int, string> GetData() => 3;

void MyFunction() {}