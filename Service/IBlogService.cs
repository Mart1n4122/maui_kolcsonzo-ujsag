using Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Services
{
    public interface IBlogService
    {
        Task<NewspaperDto> GetAsync(int id);
        Task<NewspaperDto> ListAllAsync();
        Task CreateAsync(NewspaperDto dto);
        Task UpdateAsync(NewspaperDto dto);
        Task DeleteAsync(int id);
    }
}
