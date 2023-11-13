using JetBrains.Annotations;

namespace Concurrency;

/// <summary>
/// Provides a mechanism to execute a task sequentially in a multi-threaded environment.
/// </summary>
[PublicAPI]
public class SequentialTaskExecutor
{
  private readonly SemaphoreSlim executionLock = new(1, 1);
  private readonly Func<Task> executeTask;

  /// <summary>
  /// Occurs when all pending executions have completed.
  /// </summary>
  public event Action? ExecutionCompleted;

  /// <summary>
  /// Gets a value indicating whether an execution is pending.
  /// </summary>
  public bool IsExecutionPending { get; private set; }

  /// <summary>
  /// Gets a task that completes when the current execution finishes and no further executions are enqueued.
  /// </summary>
  public Task ExecutionCompletion => IsExecutionPending ? ExecuteIfPending() : Task.CompletedTask;

  /// <summary>
  /// Initializes a new instance of the <see cref="SequentialTaskExecutor"/> class with an asynchronous action.
  /// </summary>
  /// <param name="asyncAction">The asynchronous action to be executed.</param>
  public SequentialTaskExecutor(Func<Task> asyncAction)
  {
    executeTask = asyncAction;
  }

  /// <summary>
  /// Initializes a new instance of the <see cref="SequentialTaskExecutor"/> class with a synchronous action.
  /// </summary>
  /// <param name="action">The synchronous action to be executed.</param>
  public SequentialTaskExecutor(Action action) : this(() => Task.Run(action)) { }

  /// <summary>
  /// Triggers the execution of the task.
  /// </summary>
  public void TriggerExecution()
  {
    IsExecutionPending = true;
    Task.Run(ExecuteIfPending);
  }

  /// <summary>
  /// Executes the task if it is pending, ensuring that no two executions run concurrently.
  /// </summary>
  private async Task ExecuteIfPending()
  {
    while (true)
    {
      await executionLock.WaitAsync();
      try
      {
        if (!IsExecutionPending)
          return;

        IsExecutionPending = false;
        await executeTask();
      }
      finally
      {
        executionLock.Release();
      }

      if (!IsExecutionPending)
      {
        ExecutionCompleted?.Invoke();
        break;
      }
    }
  }
}