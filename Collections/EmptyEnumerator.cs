using System.Collections;

namespace Ironclad.Collections;

public struct EmptyEnumerator<T> : IEnumerator<T>
{
	public bool MoveNext() => false;

	public void Reset() { }

	public T Current => default!;

	object IEnumerator.Current => Current!;

	public void Dispose() { }
}