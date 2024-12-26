namespace Ironclad.Relations.Playground;

public partial class MonogamousPerson
{
  [Relation] // when nothing is added, it is a relation to self
  public partial MonogamousPerson? Partner { get; set; }
}