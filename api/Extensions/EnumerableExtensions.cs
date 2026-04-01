namespace RestuarantAPI.Extensions;

public static class EnumerableExtensions
{
    /// <summary>
    /// Checks if enumerable is null or empty
    /// </summary>
    public static bool IsNullOrEmpty<T>(this IEnumerable<T>? source)
    {
        return source == null || !source.Any();
    }

    /// <summary>
    /// Safe foreach that handles null collections
    /// </summary>
    public static void SafeForEach<T>(this IEnumerable<T>? source, Action<T> action)
    {
        if (source == null || action == null)
            return;

        foreach (var item in source)
        {
            action(item);
        }
    }

    /// <summary>
    /// Paginates an enumerable
    /// </summary>
    public static IEnumerable<T> Paginate<T>(this IEnumerable<T> source, int page, int pageSize)
    {
        if (page < 1) page = 1;
        if (pageSize < 1) pageSize = 10;

        return source.Skip((page - 1) * pageSize).Take(pageSize);
    }

    /// <summary>
    /// Converts enumerable to comma-separated string
    /// </summary>
    public static string ToCommaSeparatedString<T>(this IEnumerable<T> source)
    {
        if (source.IsNullOrEmpty())
            return string.Empty;

        return string.Join(", ", source);
    }

    /// <summary>
    /// Chunks enumerable into smaller lists
    /// </summary>
    public static IEnumerable<IEnumerable<T>> ChunkBy<T>(this IEnumerable<T> source, int chunkSize)
    {
        if (chunkSize <= 0)
            throw new ArgumentException("Chunk size must be greater than 0", nameof(chunkSize));

        var list = source.ToList();
        for (int i = 0; i < list.Count; i += chunkSize)
        {
            yield return list.Skip(i).Take(chunkSize);
        }
    }
}