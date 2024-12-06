using BuildingBlocks.Application.Common.Interfaces.Data;

namespace BuildingBlocks.Infrastucture.Common.Persistence.Seeder;

public class SeederExecutor(IEnumerable<IDataSeeder> seeders)
{
    public async Task ExecuteAllAsync()
    {
        var executedSeeders = new HashSet<Type>();

        foreach (var seeder in seeders) await ExecuteSeederAsync(seeder, executedSeeders);
    }

    private async Task ExecuteSeederAsync(IDataSeeder seeder, HashSet<Type> executedSeeders)
    {
        foreach (var dependency in seeder.GetDependentDataSeeders())
        {
            if (!typeof(IDataSeeder).IsAssignableFrom(dependency))
                throw new InvalidOperationException($"{dependency.Name} does not implement IDataSeeder.");
            var dependentSeeder = seeders.FirstOrDefault(s => s.GetType() == dependency);
            if (dependentSeeder != null && !executedSeeders.Contains(dependency))
                await ExecuteSeederAsync(dependentSeeder, executedSeeders);
        }

        if (!executedSeeders.Contains(seeder.GetType()))
        {
            await seeder.SeedAllAsync();
            executedSeeders.Add(seeder.GetType());
        }
    }
}