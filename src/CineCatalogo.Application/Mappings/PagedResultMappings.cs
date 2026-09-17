using CineCatalogo.Application.Dtos;
using CineCatalogo.Domain.Common;

namespace CineCatalogo.Application.Mappings;

public static class PagedResultMappings
{
    public static PagedResultDto<TDestination> ToDto<TSource, TDestination>(
        this PagedResult<TSource> source,
        Func<TSource, TDestination> map)
    {
        var items = source.Items.Select(map).ToList();
        return new PagedResultDto<TDestination>(
            items,
            source.PageNumber,
            source.PageSize,
            source.TotalCount,
            source.TotalPages,
            source.HasPreviousPage,
            source.HasNextPage);
    }
}
