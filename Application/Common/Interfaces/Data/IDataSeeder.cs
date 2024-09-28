namespace Application.Common.Interfaces.Data;

public interface IDataSeeder
{
    Task SeedAllAsync();

    IEnumerable<Type> GetDependentDataSeeders()
    {
        return Enumerable.Empty<Type>();
    }
}