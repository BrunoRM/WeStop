using System.Collections.Generic;
using System.Threading.Tasks;
using WeStop.Api.Domain;

namespace WeStop.Api.Infra.Storages.Interfaces
{
    public interface IThemeStorage
    {
        Task<ICollection<Theme>> GetAsync();
    }
}