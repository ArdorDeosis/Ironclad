namespace Ironclad.Relations.Playground;

public partial class Pilot
{
  [Relation<Gunner>(nameof(Gunner.Pilot))]
  public partial Gunner? Gunner { get; set; }
}