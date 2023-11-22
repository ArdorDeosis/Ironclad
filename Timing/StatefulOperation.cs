using System.Diagnostics.CodeAnalysis;
using System.Reactive.Subjects;
using System.Runtime.CompilerServices;
using JetBrains.Annotations;

namespace Timing;

public enum ValueStreamPartition
{
	FullStream,
	LastAndFuture,
	FutureOnly,
}

/// <summary>
/// Represents an operation that maintains a state and can be cancelled.
/// </summary>
/// <typeparam name="TState">The type of the state.</typeparam>
[PublicAPI]
public abstract class StatefulOperation<TState> : IDisposable where TState : notnull
{
	private readonly CancellationTokenSource internalCancellationTokenSource = new();
	private readonly IndexedMultiSubject<ValueStreamPartition, TState> stateStreams;
	private TState state;
	private readonly Task executionTask;

	/// <summary>
	/// The stream of states the operation goes through including its history.
	/// </summary>
	public IObservable<TState> StateStream(ValueStreamPartition partition) => stateStreams[partition];

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
			stateStreams.OnNext(value);
		}
	}

	/// <param name="initialState">The initial state.</param>
	/// <param name="cancellationTokens">Cancellation tokens to cancel this operation.</param>
	protected StatefulOperation(TState initialState, IEnumerable<CancellationToken> cancellationTokens)
	{
		stateStreams = new IndexedMultiSubject<ValueStreamPartition, TState>(new Dictionary<ValueStreamPartition, ISubject<TState>>()
		{
			[ValueStreamPartition.FullStream] = new ReplaySubject<TState>(),
			[ValueStreamPartition.LastAndFuture] = new ReplaySubject<TState>(1),
			[ValueStreamPartition.FutureOnly] = new Subject<TState>(),
		});
		State = initialState;
		executionTask = ExecuteWrapped(CancellationTokenSource.CreateLinkedTokenSource(
			cancellationTokens.Append(internalCancellationTokenSource.Token).ToArray()
		).Token);
	}

	/// <inheritdoc />
	protected StatefulOperation(TState initialState, params CancellationToken[] cancellationTokens)
		: this(initialState, cancellationTokens.AsEnumerable()) { }

	/// <summary>
	/// Gets an awaiter used to await this <see cref="StatefulOperation{TState}"/>.
	/// </summary>
	/// <returns>An awaiter instance.</returns>
	public TaskAwaiter GetAwaiter() => executionTask.GetAwaiter();


	/// <summary>
	/// Cancels the operation.
	/// </summary>
	public void Cancel() => internalCancellationTokenSource.Cancel();

	/// <summary>
	/// The operation to execute. Set <see cref="State"/> to provide state changes to observers.
	/// </summary>
	/// <param name="cancellationToken">A cancellation token to cancel the operation.</param>
	/// <remarks>
	/// This class is built under the assumption that this <see cref="Execute"/> method runs single-threaded. Setting the
	/// <see cref="State"/> property from multiple threads can lead to inconsistent states.
	/// </remarks>
	protected abstract Task Execute(CancellationToken cancellationToken);

	/// <summary>
	/// Safely executes the operation and send termination events afterwards.
	/// </summary>
	private async Task ExecuteWrapped(CancellationToken cancellationToken)
	{
		try
		{
			await Execute(cancellationToken);
			stateStreams.OnCompleted();
		}
		catch (Exception exception)
		{
			stateStreams.OnError(exception);
			throw;
		}
	}

	/// <inheritdoc />
	public void Dispose()
	{
		internalCancellationTokenSource.Dispose();
		stateStreams.Dispose();
	}
}