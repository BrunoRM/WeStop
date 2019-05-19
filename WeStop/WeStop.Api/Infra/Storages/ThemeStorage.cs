using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WeStop.Api.Classes;
using WeStop.Api.Infra.Storages.Interfaces;

namespace WeStop.Api.Infra.Storages
{
    public class ThemeStorage : IThemeStorage
    {
        public Task<ICollection<Theme>> GetAsync()
        {
            var themes = new List<Theme>
            {
                new Theme { Name = "Nome" },
                new Theme { Name = "CEP" },
                new Theme { Name = "Cor" },
                new Theme { Name = "Carro" },
                new Theme { Name = "Objeto" },
                new Theme { Name = "PCH" },
                new Theme { Name = "Fruta" },
                new Theme { Name = "Comida" },
                new Theme { Name = "FDS" },
                new Theme { Name = "Marca" },
                new Theme { Name = "Profissão" },
                new Theme { Name = "Inseto" },
                new Theme { Name = "Instrumento musical" },
                new Theme { Name = "Programa de televisão" },
                new Theme { Name = "Flor" },
                new Theme { Name = "Banda" },
                new Theme { Name = "Super-herói" },
                new Theme { Name = "MSÉ" }
            };
            
            return Task.FromResult<ICollection<Theme>>(themes);
        }
    }
}