using System.Collections.Generic;
using System.Threading.Tasks;

namespace WeStop.Core.Storages
{
    public interface IThemeStorage
    {
        Task<ICollection<Theme>> GetAsync();
    }
}