namespace RestaurantAPI.Helpers;

/// <summary>
/// Pagination request parameters
/// Phase B.4: Standardized pagination query parameters
/// </summary>
public class PaginationParams
{
    private int _pageNumber = 1;
    private int _pageSize = 10;

    public const int MinPageSize = 1;
    public const int MaxPageSize = 100;
    public const int DefaultPageSize = 10;
    public const int DefaultPageNumber = 1;

    /// <summary>
    /// Page number (1-indexed)
    /// </summary>
    public int PageNumber
    {
        get => _pageNumber;
        set => _pageNumber = value < 1 ? 1 : value;
    }

    /// <summary>
    /// Items per page (1-100)
    /// </summary>
    public int PageSize
    {
        get => _pageSize;
        set => _pageSize = value < MinPageSize ? DefaultPageSize 
                          : value > MaxPageSize ? MaxPageSize 
                          : value;
    }

    /// <summary>
    /// Calculate offset for database queries
    /// </summary>
    public int GetOffset() => (PageNumber - 1) * PageSize;

    /// <summary>
    /// Validate pagination parameters
    /// </summary>
    public bool IsValid() => PageNumber >= 1 && PageSize >= MinPageSize && PageSize <= MaxPageSize;
}

/// <summary>
/// Pagination metadata for responses
/// Phase B.4: Standardized pagination response structure
/// </summary>
public class PaginationMetadataResponse
{
    public int PageNumber { get; set; }
    public int PageSize { get; set; }
    public int TotalCount { get; set; }
    public int TotalPages { get; set; }
    public bool HasNextPage { get; set; }
    public bool HasPreviousPage { get; set; }
    public int? NextPage => HasNextPage ? PageNumber + 1 : null;
    public int? PreviousPage => HasPreviousPage ? PageNumber - 1 : null;

    public static PaginationMetadataResponse Create(int pageNumber, int pageSize, int totalCount)
    {
        var totalPages = (int)Math.Ceiling((double)totalCount / pageSize);
        
        return new PaginationMetadataResponse
        {
            PageNumber = pageNumber,
            PageSize = pageSize,
            TotalCount = totalCount,
            TotalPages = totalPages,
            HasNextPage = pageNumber < totalPages,
            HasPreviousPage = pageNumber > 1
        };
    }
}

/// <summary>
/// Generic paginated response envelope
/// Phase B.4: Standardized structure for all paginated responses
/// </summary>
public class PaginatedResponse<T>
{
    public List<T> Data { get; set; } = new();
    public PaginationMetadataResponse Pagination { get; set; } = new();

    public static PaginatedResponse<T> Create(IEnumerable<T> data, int pageNumber, int pageSize, int totalCount)
    {
        return new PaginatedResponse<T>
        {
            Data = data.ToList(),
            Pagination = PaginationMetadataResponse.Create(pageNumber, pageSize, totalCount)
        };
    }
}

/// <summary>
/// Helper methods for pagination
/// </summary>
public static class PaginationHelper
{
    /// <summary>
    /// Paginate a query result
    /// </summary>
    public static PaginatedResponse<T> Paginate<T>(IEnumerable<T> items, int pageNumber, int pageSize, int totalCount)
    {
        return PaginatedResponse<T>.Create(items, pageNumber, pageSize, totalCount);
    }

    /// <summary>
    /// Paginate a queryable result
    /// </summary>
    public static async Task<PaginatedResponse<T>> PaginateAsync<T>(
        IQueryable<T> query,
        int pageNumber,
        int pageSize)
    {
        var totalCount = query.Count();
        var items = query
            .Skip((pageNumber - 1) * pageSize)
            .Take(pageSize)
            .ToList();

        return PaginatedResponse<T>.Create(items, pageNumber, pageSize, totalCount);
    }

    /// <summary>
    /// Extract pagination parameters from query string
    /// </summary>
    public static PaginationParams ExtractFromQuery(string? pageNumberStr, string? pageSizeStr)
    {
        var pageNumber = int.TryParse(pageNumberStr, out var pn) ? pn : PaginationParams.DefaultPageNumber;
        var pageSize = int.TryParse(pageSizeStr, out var ps) ? ps : PaginationParams.DefaultPageSize;

        return new PaginationParams { PageNumber = pageNumber, PageSize = pageSize };
    }
}
