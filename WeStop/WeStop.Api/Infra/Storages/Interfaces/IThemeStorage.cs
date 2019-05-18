using System.Collections.Generic;
using System.Threading.Tasks;
using WeStop.Api.Classes;

namespace WeStop.Api.Infra.Storages.Interfaces
{
    public interface IThemeStorage
    {
        Task<ICollection<Theme>> GetAsync();
    }
}