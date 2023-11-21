namespace Timing.Tests;

internal class TestException : Exception { }

public readonly struct ExpectedDelayedOperationStates
{
	internal DelayedOperationState FinalState => StateChanges.Last();
	internal IList<DelayedOperationState> StateChanges { get; init; }

	internal static ExpectedDelayedOperationStates SuccessfulOperation { get; } = new()
	{
		StateChanges = new[]
		{
			DelayedOperationState.Waiting,
			DelayedOperationState.Running,
			DelayedOperationState.CompletedSuccessfully,
		},
	};

	internal static ExpectedDelayedOperationStates FailedOperation { get; } = new()
	{
		StateChanges = new[]
		{
			DelayedOperationState.Waiting,
			DelayedOperationState.Running,
			DelayedOperationState.Faulted,
		},
	};

	internal static ExpectedDelayedOperationStates OperationCanceledDuringExecution { get; } = new()
	{
		StateChanges = new[]
		{
			DelayedOperationState.Waiting,
			DelayedOperationState.Running,
			DelayedOperationState.Canceled,
		},
	};

	internal static ExpectedDelayedOperationStates OperationCanceledBeforeExecution { get; } = new()
	{
		StateChanges = new[]
		{
			DelayedOperationState.Waiting,
			DelayedOperationState.Canceled,
		},
	};
}

public class DelayedOperationTests
{
	private static readonly Action DoNothing = () => { Console.WriteLine("doing noting"); };
	private static readonly Action FailingAction = () =>
	{
		Console.WriteLine("throwing now");
		throw new TestException();
	};
	private static readonly Action SelfCancelingAction = () =>
	{
		Console.WriteLine("canceling now");
		throw new OperationCanceledException();
	};

	private static readonly Func<CancellationToken, Task> WaitingAction =
		token => Task.Delay(TimeSpan.FromSeconds(1), token);

	private CancellationTokenSource? cancellationTokenSource;
	private CancellationToken CancellationToken => cancellationTokenSource!.Token;

	[SetUp]
	public void Setup()
	{
		cancellationTokenSource = new CancellationTokenSource();
	}

	[TearDown]
	public void TearDown()
	{
		cancellationTokenSource!.Cancel();
		cancellationTokenSource = null;
	}

	[Test]
	public void Constructor_ThrowsWhenDelayIsNegative()
	{
		// ARRANGE
		var negativeTimeSpan = TimeSpan.FromSeconds(-1);

		// ACT + ASSERT
		Assert.That(() => new DelayedOperation(negativeTimeSpan, DoNothing), Throws.ArgumentException);
	}

	internal static TestCaseData[] OperationsAndStates =
	{
		new TestCaseData(
			DoNothing,
			ExpectedDelayedOperationStates.SuccessfulOperation
		).SetName("successful operation"),
		new TestCaseData(
			FailingAction,
			ExpectedDelayedOperationStates.FailedOperation
		).SetName("failing operation"),
		new TestCaseData(
			SelfCancelingAction,
			ExpectedDelayedOperationStates.OperationCanceledDuringExecution
		).SetName("self-canceling operation"),
	};


	[TestCaseSource(nameof(OperationsAndStates))]
	public async Task ActionFinishes_FinalStateIsAsExpected(Action operation, ExpectedDelayedOperationStates expected)
	{
		// ARRANGE
		var delayedOperation = new DelayedOperation(TimeSpan.Zero, operation);

		// ACT
		await delayedOperation;

		// ASSERT
		Assert.That(delayedOperation.State, Is.EqualTo(expected.FinalState));
	}
	
	[Test]
	public async Task ActionIsCanceledDuringWaiting_FinalStateIsAsExpected()
	{
		// ARRANGE
		var delayedOperation = new DelayedOperation(TimeSpan.FromSeconds(1), DoNothing);

		// ACT
		await Task.Delay(TimeSpan.FromMilliseconds(10), CancellationToken);
		delayedOperation.Cancel();
		await Task.Delay(TimeSpan.FromMilliseconds(10), CancellationToken);

		// ASSERT
		Assert.That(delayedOperation.State, Is.EqualTo(DelayedOperationState.Canceled));
	}

	private async Task<DelayedOperation> SetupDelayedOperationInWaitingState()
	{
		var delayedOperation = new DelayedOperation(TimeSpan.FromSeconds(1), DoNothing);
		await Task.Delay(TimeSpan.FromMilliseconds(10), CancellationToken);
		return delayedOperation;
	}

	[Test]
	public void DuringWaitingPeriod_StateIsWaiting()
	{
		// ARRANGE
		var operation = new DelayedOperation(TimeSpan.FromSeconds(1), DoNothing);

		// ASSERT
		Assert.That(operation.State, Is.EqualTo(DelayedOperationState.Waiting));
	}

	[Test]
	public async Task DuringExecution_StateIsRunning()
	{
		// ARRANGE
		var operation = new DelayedOperation(TimeSpan.Zero, WaitingAction);
		await Task.Delay(TimeSpan.FromMilliseconds(10), CancellationToken);

		// ASSERT
		Assert.That(operation.State, Is.EqualTo(DelayedOperationState.Running));
	}

	[TestCaseSource(nameof(OperationsAndStates))]
	public async Task ActionFinishes_StateTransitionsAreAsExpected(Action operation, ExpectedDelayedOperationStates expected)
	{
		// ARRANGE
		List<DelayedOperationState> stateChanges = new();
		var delayedOperation = new DelayedOperation(TimeSpan.Zero, operation, stateChanges.Add);

		// ACT
		await delayedOperation;

		// ASSERT
		Assert.That(stateChanges, Is.EquivalentTo(expected.StateChanges));
	}
}