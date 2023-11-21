namespace Timing;

/// <summary>
/// The state a delayed operation can have.
/// </summary>
public enum DelayedOperationState
{
	/// <summary>The delay of this delayed action has not yet been started.</summary>
	/// <remarks>This state is changed to <see cref="Waiting"/> in the constructor and should never be observed.</remarks>
	NotStarted = default,
	
	/// <summary>The delayed action has not yet been invoked, nor has it been cancelled.</summary>
	/// <remarks>This is a volatile state and can change at any given time.</remarks>
	Waiting,
	
	/// <summary>The delayed action is executing.</summary>
	/// <remarks>This is a volatile state and can change at any given time.</remarks>
	Running,

	/// <summary>The operation was invoked and ran to completion successfully.</summary>
	/// <remarks>This is a final state; if any delayed operation reaches it, it will not change again.</remarks>
	CompletedSuccessfully,

	/// <summary>The operation was invoked and failed.</summary>
	/// <remarks>This is a final state; if any delayed operation reaches it, it will not change again.</remarks>
	Faulted,

	/// <summary>The operation was cancelled before it completed.</summary>
	/// <remarks>This is a final state; if any delayed operation reaches it, it will not change again.</remarks>
	Canceled,
}