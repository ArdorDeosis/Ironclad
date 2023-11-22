using System.Collections.Immutable;
using System.Diagnostics.CodeAnalysis;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using System.Runtime.CompilerServices;
using JetBrains.Annotations;

namespace Timing;

public enum ValueStreamPartition
{
	FullStream,
	History,
	CurrentAndFuture,
	FutureOnly,
}

public class ValueStream<T> : IObserver<T>
{
	private readonly ReplaySubject<T> full = new();
	private readonly ImmutableList<T> history = ImmutableList<T>.Empty;
	private readonly Subject<T> future = new();
	private readonly ReplaySubject<T> currentAndFuture = new(1);

	public ValueStream()
	{
		future.Subscribe(value => history.Add(value));
	}
	
	public IObservable<T> Get(ValueStreamPartition partition) =>
		partition switch {
			ValueStreamPartition.FullStream => history.ToObservable().Concat(future),
			ValueStreamPartition.History => history.ToObservable(),
			ValueStreamPartition.CurrentAndFuture => future.StartWith(history.Last()),
			ValueStreamPartition.FutureOnly => future,
			_ => throw new ArgumentOutOfRangeException(nameof(partition), partition, null),
		};

	public void OnCompleted()
	{
		throw new NotImplementedException();
	}

	public void OnError(Exception error)
	{
		throw new NotImplementedException();
	}

	public void OnNext(T value)
	{
		throw new NotImplementedException();
	}
}


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
	/// The stream of states the operation goes through including its history.
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
			stateStream.OnCompleted();
		}
		catch (Exception exception)
		{
			stateStream.OnError(exception);
			throw;
		}
	}

	/// <inheritdoc />
	public void Dispose()
	{
		internalCancellationTokenSource.Dispose();
		stateStream.Dispose();
	}
}