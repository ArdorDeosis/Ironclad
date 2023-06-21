using Ironclad.Relations.Playground;

namespace Ironclad.Relations.Tests;

internal class A
{
	private readonly RelationsManager manager;

	public A(RelationsManager manager)
	{
		this.manager = manager;
	}

	internal B? Other
	{
		get => manager.FromThisProperty<A>().To<B>(b => b.Other).Get(this);
		set => manager.FromThisProperty<A>().To<B>(b => b.Other).Set(this, value);
	}
}

internal class B
{
	private readonly RelationsManager manager;

	public B(RelationsManager manager)
	{
		this.manager = manager;
	}

	internal A? Other
	{
		get => manager.FromThisProperty<B>().To<A>(a => a.Other).Get(this);
		set => manager.FromThisProperty<B>().To<A>(a => a.Other).Set(this, value);
	}
}

public class Tests
{
	[Test]
	public void UnassignedRelationalPropertiesAreNull()
	{
		var manager = new RelationsManager();
		var a = new A(manager);
		var b = new B(manager);
		Assert.Multiple(() =>
		{
			Assert.That(a.Other, Is.Null);
			Assert.That(b.Other, Is.Null);
		});
	}
	
	[Test]
	public void AssigningToOneAssignsTheOther()
	{
		var manager = new RelationsManager();
		var a = new A(manager);
		var b = new B(manager);

		a.Other = b;
		Assert.Multiple(() =>
		{
			Assert.That(a.Other, Is.EqualTo(b));
			Assert.That(b.Other, Is.EqualTo(a));
		});
	}
	
	// TODO: also tests object initializer
	[Test]
	public void AssigningToOneReverseAssignsTheOther()
	{
		var manager = new RelationsManager();
		var a = new A(manager);
		var b = new B(manager)
		{
			Other = a,
		};

		Assert.Multiple(() =>
		{
			Assert.That(a.Other, Is.EqualTo(b));
			Assert.That(b.Other, Is.EqualTo(a));
		});
	}
	
	[Test]
	public void AssigningNullRemovesRelationalPropertyFromTheOther()
	{
		var manager = new RelationsManager();
		var a = new A(manager);
		var b = new B(manager)
		{
			Other = a,
		};
		
		// ACT
		a.Other = null;

		Assert.Multiple(() =>
		{
			Assert.That(a.Other, Is.Null);
			Assert.That(b.Other, Is.Null);
		});
	}
}