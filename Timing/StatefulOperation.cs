using System.Diagnostics.CodeAnalysis;
using System.Reactive.Subjects;
using JetBrains.Annotations;

namespace Timing;

/// <summary>
/// Represents an operation that maintains a state and can be cancelled.
/// </summary>
/// <typeparam name="TState">The type of the state.</typeparam>
[PublicAPI]
public abstract class StatefulOperation<TState> : IDisposable where TState: notnull
{
	private readonly CancellationTokenSource internalCancellationTokenSource = new();
	private readonly ReplaySubject<TState> stateStream = new();
	private TState state;
	private readonly Task executionTask;

	/// <summary>
	/// The stream of state changes.
	/// </summary>
	public IObservable<TState> StateStream => stateStream;

	/// <summary>
	/// The current state of this operation.
	/// </summary>
	protected TState State
	{
		get => state;
		[MemberNotNull(nameof(state))]
		set
		{
			if (value is null)
				throw new ArgumentNullException(nameof(value));
			if (value.Equals(state))
				return;
			state = value;
			stateStream.OnNext(value);
		}
	}

	/// <param name="initialState">The initial state.</param>
	/// <param name="cancellationTokens">Cancellation tokens to cancel this operation.</param>
	protected StatefulOperation(TState initialState, IEnumerable<CancellationToken> cancellationTokens)
	{
		State = initialState;
		executionTask = ExecuteWrapped(CancellationTokenSource.CreateLinkedTokenSource(
			cancellationTokens.Append(internalCancellationTokenSource.Token).ToArray()
		).Token);
	}

	/// <inheritdoc />
	protected StatefulOperation(TState initialState, params CancellationToken[] cancellationTokens)
	: this(initialState, cancellationTokens.AsEnumerable())
	{ }

	/// <summary>
	/// Cancels the operation.
	/// </summary>
	public void Cancel() => internalCancellationTokenSource.Cancel();

	/// <summary>
	/// The operation to execute. Set <see cref="State"/> to provide state changes to observers.
	/// </summary>
	/// <param name="cancellationToken">A cancellation token to cancel the operation.</param>
	protected abstract Task Execute(CancellationToken cancellationToken);

	/// <summary>
	/// Safely executes the operation and send termination events afterwards.
	/// </summary>
	private async Task ExecuteWrapped(CancellationToken cancellationToken)
	{
		try
		{
			await Execute(cancellationToken);
			stateStream.OnCompleted();
		}
		catch (Exception exception)
		{
			stateStream.OnError(exception);
		}
	}

	/// <inheritdoc />
	public void Dispose()
	{
		internalCancellationTokenSource.Dispose();
		stateStream.Dispose();
	}
}