using System.Collections.Generic;
using System.Threading.Tasks;
using WeStop.Api.Domain;
using WeStop.Api.Infra.Storages.Interfaces;

namespace WeStop.Api.Infra.Storages.InMemory
{
    public class ThemeStorage : IThemeStorage
    {
        public Task<ICollection<Theme>> GetAsync()
        {
            var themes = new List<Theme>
            {
                new Theme ("Nome"),
                new Theme ("CEP"),
                new Theme ("Cor"),
                new Theme ("Carro"),
                new Theme ("Objeto"),
                new Theme ("PCH"),
                new Theme ("Fruta"),
                new Theme ("Comida"),
                new Theme ("FDS"),
                new Theme ("Marca"),
                new Theme ("Profissão"),
                new Theme ("Inseto"),
                new Theme ("Instrumento musical"),
                new Theme ("Programa de televisão"),
                new Theme ("Flor"),
                new Theme ("Banda"),
                new Theme ("Super-herói"),
                new Theme ("MSÉ")
            };
            
            return Task.FromResult<ICollection<Theme>>(themes);
        }
    }
}