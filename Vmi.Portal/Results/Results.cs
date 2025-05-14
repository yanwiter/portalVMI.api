namespace Vmi.Portal;

public class Result<T>
{
    public bool IsSuccess { get; set; }
    public ErrorDetails Error { get; set; }
    public T Data { get; set; }
    public PaginationDetails Pagination { get; set; }
}

public class ErrorDetails
{
    public string Message { get; set; }
    public string Code { get; set; }
}

public class PaginationDetails
{
    public int Page { get; set; }
    public int PageSize { get; set; }
    public int TotalItems { get; set; }
    public int TotalPages { get; set; }
}