using JetBrains.Annotations;

namespace Timing;

[PublicAPI]
public class DelayedOperationRegistry<T> where T : notnull
{
	private readonly Dictionary<T, DelayedOperation> operations = new();
	private readonly Func<T, CancellationToken, Task> operationBlueprint;
	private readonly IObservable<(T, DelayedOperationState)> operationStateChangeStream = new ;

	public IObservable<(T, DelayedOperationState)> OperationStateChangeStream => operationStateChangeStream;

	public DelayedOperationRegistry(Func<T, CancellationToken, Task> operation)
	{
		operationBlueprint = operation;
	}

	public DelayedOperationRegistry(Func<T, Task> operation)
		: this(async (id, _) => await operation(id)) { }

	public DelayedOperationRegistry(Action<T> operation)
		: this((id, _) =>
		{
			operation(id);
			return Task.CompletedTask;
		}) { }

	public DelayedOperationRegistry(Action<T, CancellationToken> operation)
		: this((id, cancellationToken) =>
		{
			operation(id, cancellationToken);
			return Task.CompletedTask;
		}) { }

	/// <returns>Whether this registry has a delayed operation registered for the given ID.</returns>
	public bool HasId(T id) => operations.ContainsKey(id);
	
	/// <summary>
	/// Sets a delayed operation (and possibly overwrites an existing one) for the given ID.
	/// </summary>
	/// <param name="id">The ID for which the delayed action is registered.</param>
	/// <param name="delay">The delay after which the action will be performed for the given ID.</param>
	/// <exception cref="ArgumentException">If <paramref name="delay"/> is zero or negative.</exception>
	public void Set(T id, TimeSpan delay)
	{
		if (delay < TimeSpan.Zero)
			throw new ArgumentException("Parameter {0} cannot be negative.", nameof(delay));

		Cancel(id);
		
		operations[id] = new DelayedOperation(delay, 
			cancellationToken => operationBlueprint(id, cancellationToken), 
			state => InvokeOperationStateChanged(id, state));
	}

	/// <summary>
	/// Cancels the delayed operation registered for the given ID.
	/// </summary>
	/// <param name="id">The ID for which the operation to be canceled is registered.</param>
	/// <returns>
	/// <tt>true</tt> if the operation was actually canceled or <tt>false</tt> if there was no operation registered for
	/// the given ID or if the registered operation was already completed, faulted or canceled before.
	/// </returns>
	public bool Cancel(T id) => operations.Remove(id, out var operation) && operation.Cancel();

	/// <summary>
	/// Cancels all registered operations.
	/// </summary>
	public void CancelAll()
	{
		foreach (var operation in operations.Values)
			operation.Cancel();
		operations.Clear();
	}

	private void InvokeOperationStateChanged(T id, DelayedOperationState state) =>
		OperationStateChanged?.Invoke(id, state);
}