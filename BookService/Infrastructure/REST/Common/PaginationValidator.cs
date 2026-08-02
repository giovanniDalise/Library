using Microsoft.AspNetCore.Mvc;

namespace Library.BookService.Infrastructure.REST.Common
{
    public static class PaginationValidator
    {
        public static ActionResult? Validate(int page, int pageSize, int maxPageSize = 10)
        {
            if (page < 1)
                return new BadRequestObjectResult(new { error = "Page must be greater than or equal to 1." });

            if (pageSize < 1 || pageSize > maxPageSize)
                return new BadRequestObjectResult(new { error = $"PageSize must be between 1 and {maxPageSize}." });

            return null;
        }
    }
}