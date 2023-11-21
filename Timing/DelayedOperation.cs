using JetBrains.Annotations;

namespace Timing;

/// <summary>
/// A single-use delayed operation that is invoked after a given delay.
/// This can be awaited and will return its final state after completion.
/// </summary>
[PublicAPI]
public sealed class DelayedOperation : StatefulOperation<DelayedOperationState>
{
	private readonly TimeSpan delay;
	private readonly Func<CancellationToken, Task> operation;

	/// <param name="delay">The delay after which the <paramref name="operation"/> is performed.</param>
	/// <param name="operation">The operation to perform.</param>
	/// <exception cref="ArgumentException">If <paramref name="delay"/> is negative.</exception>
	public DelayedOperation(TimeSpan delay, Func<CancellationToken, Task> operation)
		: base(DelayedOperationState.NotStarted)
	{
		if (delay < TimeSpan.Zero)
			throw new ArgumentException("{0} cannot be negative", nameof(delay));
		this.delay = delay;
		this.operation = operation;
	}

	/// <inheritdoc />
	public DelayedOperation(TimeSpan delay, Action operation)
		: this(delay, _ =>
		{
			operation();
			return Task.CompletedTask;
		}) { }

	/// <inheritdoc />
	public DelayedOperation(TimeSpan delay, Action<CancellationToken> operation)
		: this(delay, cancellationToken =>
		{
			operation(cancellationToken);
			return Task.CompletedTask;
		}) { }

	/// <inheritdoc />
	public DelayedOperation(TimeSpan delay, Func<Task> operation)
		: this(delay, _ => operation()) { }

	/// <inheritdoc />
	protected override async Task Execute(CancellationToken cancellationToken)
	{
		State = DelayedOperationState.Waiting;
		await Task.Delay(delay, cancellationToken);
		State = DelayedOperationState.Running;
		await operation(cancellationToken);
		State = DelayedOperationState.CompletedSuccessfully;
	}
}