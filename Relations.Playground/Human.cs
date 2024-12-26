namespace Ironclad.Relations.Playground;

public partial class Human
{
  [Relation(nameof(Children))] // to generic type means relation to same type
  public partial Human? Mother { get; set; }
	
  [Relation(nameof(Mother))]
  public partial ISet<Human> Children { get; }
}