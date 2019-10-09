using System.Collections.Generic;
using System.Threading.Tasks;
using WeStop.Api.Core;

namespace WeStop.Api.Infra.Storages.Interfaces
{
    public interface IThemeStorage
    {
        Task<ICollection<Theme>> GetAsync();
    }
}