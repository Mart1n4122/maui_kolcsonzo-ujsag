using Common;

namespace Services;

public interface IBlogService
{
    Task<NewspaperDto> GetAsync(int id);
    Task<List<NewspaperDto>> ListAllAsync();
    Task CreateAsync(NewspaperDto dto);
    Task UpdateAsync(NewspaperDto dto);
    Task DeleteAsync(int id);
}
