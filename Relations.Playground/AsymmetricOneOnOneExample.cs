namespace Ironclad.Relations.Playground;

public class Driver
{
	private readonly SingleValueRelationHandle<Driver,Driver> partnerRelation;
	private readonly SingleValueRelationHandle<Driver,Car> carRelation;

	public Driver(RelationsManager relationsManager)
	{
		partnerRelation = relationsManager
			.From<Driver>(driver => driver.Partner)
			.ToSelf;
		carRelation = relationsManager
			.From<Driver>(driver => driver.Car)
			.To<Car>(car => car.Driver);
	}
	
	public Driver? Partner
	{
		get => partnerRelation.Get(this);
		set => partnerRelation.Set(this, value);
	}
	
	public Car? Car
	{
		get => carRelation.Get(this);
		set => carRelation.Set(this, value);
	}
}

public class Car
{
	private readonly RelationsManager relationsManager;

	public Car(RelationsManager relationsManager)
	{
		this.relationsManager = relationsManager;
	}

	public Driver? Driver
	{
		get => relationsManager.FromThisProperty<Car>().To<Driver>(driver => driver.Car).Get(this);
		set => relationsManager.FromThisProperty<Car>().To<Driver>(driver => driver.Car).Set(this, value);
	}
}