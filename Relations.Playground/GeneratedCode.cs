using System.Collections.Immutable;

namespace Ironclad.Relations.Playground;

// generated
// TODO: should also move the manager out of the class
public partial class MonogamousPerson
{
	private static readonly UndirectedRelationshipManager<MonogamousPerson> RelationshipManager = new();

	public partial MonogamousPerson? Partner
	{
		get => RelationshipManager.Get(this);
		set => RelationshipManager.Set(this, value);
	}
}

// === generated file for Sith internal directed relation ===

public partial class Sith
{
	public partial Sith? Master
	{
		get => Relation2.Manager.GetA(this);
		set => Relation2.Manager.SetA(this, value);
	}

	public partial Sith? Apprentice
	{
		get => Relation2.Manager.GetB(this);
		set => Relation2.Manager.SetB(this, value);
	}
}

file static class Relation2 // only named like this, because we are in the same file as the code below
{
	public static readonly DirectedRelationshipManager<Sith, Sith> Manager = new();
}

// === generated file for Pilot <=> Gunner Relation ===

public partial class Pilot
{
	public partial Gunner? Gunner
	{
		get => Relation.Manager.GetB(this);
		set => Relation.Manager.SetB(this, value);
	}
}

public partial class Gunner
{
	public partial Pilot? Pilot
	{
		get => Relation.Manager.GetA(this);
		set => Relation.Manager.SetA(this, value);
	}
}

file static class Relation
{
	public static readonly DirectedRelationshipManager<Pilot, Gunner> Manager = new();
}

// === generated file for Mother <=> Children Relation ===

public partial class Human
{
	// to generic type means relation to same type
	public partial Human? Mother
	{
		get => throw new NotImplementedException();
		set => throw new NotImplementedException();
	}

	public partial ISet<Human> Children
	{
		get => throw new NotImplementedException();
	}
}


file static class Relation3
{
	public static readonly DirectedRelationshipManager<Human, Human> Manager = new();
}