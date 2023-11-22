using System.Diagnostics.CodeAnalysis;
using System.Reactive.Subjects;

namespace Timing;

/// <summary>
/// Represents a collection of subjects indexed by a index, allowing observers to subscribe to different subjects based on the index.
/// </summary>
/// <typeparam name="TIndex">The type of the index.</typeparam>
/// <typeparam name="TValue">The type of the values emitted by the subjects.</typeparam>
public sealed class IndexedMultiSubject<TIndex, TValue> : IObserver<TValue> where TIndex : notnull
{
	// TODO: make it a FrozenDictionary
	private readonly IDictionary<TIndex, ISubject<TValue>> subjects;
	private readonly object operationLock = new { };

	/// <summary>
	/// Initializes a new instance of the IndexedMultiSubject class with a predefined dictionary of subjects.
	/// </summary>
	/// <param name="subjects">A dictionary of subjects indexed by TIndex.</param>
	public IndexedMultiSubject(IDictionary<TIndex, ISubject<TValue>> subjects)
	{
		// TODO: make it a FrozenDictionary
		this.subjects = new Dictionary<TIndex, ISubject<TValue>>(subjects);
	}

	/// <summary>
	/// Initializes a new instance of the IndexedMultiSubject class with a params array of index-value pairs.
	/// </summary>
	/// <param name="subjects">A params array of index-value pairs to be used as subjects.</param>
	public IndexedMultiSubject(params KeyValuePair<TIndex, ISubject<TValue>>[] subjects)
	{
		// TODO: make it a FrozenDictionary
		this.subjects = new Dictionary<TIndex, ISubject<TValue>>(subjects);
	}

	/// <summary>
	/// Gets the observable sequence associated with the specified index.
	/// </summary>
	/// <param name="index">The index to retrieve the observable sequence.</param>
	/// <returns>The observable sequence associated with the specified index.</returns>
	/// <exception cref="ArgumentNullException">The index is null.</exception>
	/// <exception cref="KeyNotFoundException">The index is not found.</exception>
	[SuppressMessage("ReSharper", "InconsistentlySynchronizedField")]
	// read-only access to a (after initialization) read-only dictionary does not need to be synchronized
	public IObservable<TValue> this[TIndex index] => subjects[index];

	/// <summary>
	/// Determines whether the IndexedMultiSubject contains an IObservable with the specified index.
	/// </summary>
	/// <param name="index">The index to locate in the IndexedMultiSubject.</param>
	/// <returns>true if the IndexedMultiSubject contains an element with the index; otherwise, false.</returns>
	/// <exception cref="ArgumentNullException">The index is null.</exception>
	[SuppressMessage("ReSharper", "InconsistentlySynchronizedField")]
	// read-only access to a (after initialization) read-only dictionary does not need to be synchronized
	public bool ContainsIndex(TIndex index) => subjects.ContainsKey(index);

	/// <summary>
	/// Tries to get the value associated with the specified index from the IndexedMultiSubject.
	/// </summary>
	/// <param name="index">The index of the value to get.</param>
	/// <param name="subject">
	/// When this method returns, contains the object that has the specified index, or the default
	/// value of the type if the operation failed.
	/// </param>
	/// <returns>true if the IndexedMultiSubject contains an element with the specified index; otherwise, false.</returns>
	/// <exception cref="ArgumentNullException">The index is null.</exception>
	[SuppressMessage("ReSharper", "InconsistentlySynchronizedField")]
	// read-only access to a (after initialization) read-only dictionary does not need to be synchronized
	public bool TryGet(TIndex index, [NotNullWhen(true)] out ISubject<TValue>? subject)
	{
		return subjects.TryGetValue(index, out subject);
	}

	/// <inheritdoc />
	public void OnCompleted()
	{
		lock (operationLock)
		{
			foreach (var subject in subjects.Values)
				subject.OnCompleted();
		}
	}

	/// <inheritdoc />
	public void OnError(Exception error)
	{
		lock (operationLock)
		{
			foreach (var subject in subjects.Values)
				subject.OnError(error);
		}
	}

	/// <inheritdoc />
	public void OnNext(TValue value)
	{
		lock (operationLock)
		{
			foreach (var subject in subjects.Values)
				subject.OnNext(value);
		}
	}
}