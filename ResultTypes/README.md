# Ironclad.ResultTypes

`Ironclad.ResultTypes` offers a way to represent the result of operations that can fail. It is very opinionated and only offers interaction methods that I believe enforce clean code.

⚠️ Note: I'd like to create custom Roslyn Analyzers that mark errors when a result's value is used without checking beforehand.

## Usage

### Creating a Result

```csharp
var successWithoutValue = Result<string>.Success;
var successWithValue = Result<int, string>.Success(0xC0FFEE);

var error = Result<string>.Error("😯");
var anotherError = Result<int, string>.Error("😯");
```
Both result types offer implicit conversion operators to create a result from a value or an error value.
```csharp
Result<int, string> successWithValue = 0xBEEF;

Result<string> error = "😯";
Result<int, string> anotherError = "😯";
```

### Handling a Result
#### Checking for Errors
To check for errors, use the `IsError` property or the `IsError(out TError error)` method.
```csharp
Result<string> result = DoSomething();

if (result.IsError)
{
    // Handle error
}
```
```csharp
Result<string> result = DoSomething();

if (result.IsError(out string error))
{
    // Handle error
}
```

#### Using Return Values
The `Result<TValue, TError>` type is implicitly convertible to `TValue` if it is a success result. If it is an error result, it will throw an `InvalidOperationException`.
```csharp
Result<int, string> result = GetData();
int x = result;
```
Fallback values can be provided using the `Or(TValue fallback)` method and `OrDefault` Property.
```csharp
Result<int, string> result = GetData();
int x = result.Or(0xBEEF);
int y = result.OrDefault;
```

#### Throwing Custom Exceptions
Custom exceptions can be thrown using the `OrThrow(Exception exception)` method.
```csharp
Result<int, string> result = GetData();
int x = result.OrThrow(new Exception("Something went wrong"));

Result<string> result = DoSomething();
result.OrThrow(new Exception("Something went wrong"));
```