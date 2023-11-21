namespace RxToAsync;

public static class ObservableExtensions
{
  /// <summary>
  /// Converts an IObservable into a Task that completes when the observable sequence completes.
  /// </summary>
  /// <typeparam name="T">The type of elements in the source sequence.</typeparam>
  /// <param name="observable">The observable sequence to convert.</param>
  /// <returns>A Task that represents the completion of the observable sequence.</returns>
  /// <remarks>
  /// The returned task will complete with an exception if the observable sequence
  /// terminates with an error. If the error is an OperationCanceledException, the task will be
  /// in the Canceled state. Otherwise, the task will be in the Faulted state with the original exception.
  /// </remarks>
  public static Task CompletionAsync<T>(this IObservable<T> observable)
  {
    var source = new TaskCompletionSource();
    observable.Subscribe(
      _ => { },
      exception =>
      {
        if (exception is OperationCanceledException)
          source.SetCanceled();
        else
          source.SetException(exception);
      },
      () => source.SetResult()
    );
    return source.Task;
  }
}