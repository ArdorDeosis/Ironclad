namespace Ironclad.Relations.Playground;

public partial class Gunner
{
  [Relation<Pilot>(nameof(Pilot.Gunner))]
  public partial Pilot? Pilot { get; set; }
}