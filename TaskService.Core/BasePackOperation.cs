namespace TaskService.Core;

public abstract class BasePackOperation
{
    protected static async Task EntityPackOperationAsync<TEntity>(IEnumerable<TEntity> entities, Func<TEntity, Task> operation)
    {
        foreach (TEntity entity in entities)
        {
            await operation(entity);
        }
    }

    protected static void EntityPackOperation<TEntity>(IEnumerable<TEntity> entities, Action<TEntity> operation)
    {
        foreach (TEntity entity in entities)
        {
            operation(entity);
        }
    }

    protected static Task<IEnumerable<TEntity>> EntityPackOperationAsync<TEntity>(IEnumerable<TEntity> entities, Func<TEntity, Task<TEntity>> operation)
    {
        return BasePackOperation.EntityPackOperationAsync<TEntity, TEntity>(entities, operation);
    }

    protected static IEnumerable<TEntity> EntityPackOperation<TEntity>(IEnumerable<TEntity> entities, Func<TEntity, TEntity> operation)
    {
        return BasePackOperation.EntityPackOperation<TEntity, TEntity>(entities, operation);
    }

    protected static async Task<IEnumerable<TOutput>> EntityPackOperationAsync<TInput, TOutput>(IEnumerable<TInput> entities, Func<TInput, Task<TOutput>> operation)
    {
        List<TOutput> outputs = new();

        foreach (TInput item in entities)
        {
            outputs.Add(await operation(item));
        }

        return outputs;
    }

    protected static IEnumerable<TOutput> EntityPackOperation<TInput, TOutput>(IEnumerable<TInput> entities, Func<TInput, TOutput> operation)
    {
        List<TOutput> outputs = new();

        foreach (TInput item in entities)
        {
            outputs.Add(operation(item));
        }

        return outputs;
    }
}
