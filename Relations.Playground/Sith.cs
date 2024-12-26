namespace Ironclad.Relations.Playground;

public partial class Sith
{
  [Relation(nameof(Apprentice))] // to generic type means relation to same type
  public partial Sith? Master { get; set; }
	
  [Relation(nameof(Master))]
  public partial Sith? Apprentice { get; set; }
}