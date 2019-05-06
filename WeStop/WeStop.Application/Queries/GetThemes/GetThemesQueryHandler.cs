using MediatR;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using WeStop.Application.Dtos.Theme;
using WeStop.Infra;

namespace WeStop.Application.Queries.GetThemes
{
    public class GetThemesQueryHandler : IRequestHandler<GetThemesQuery, IEnumerable<ThemeDto>>
    {
        private readonly WeStopDbContext _db;

        public GetThemesQueryHandler(WeStopDbContext db)
        {
            _db = db;
        }

        public async Task<IEnumerable<ThemeDto>> Handle(GetThemesQuery request, CancellationToken cancellationToken)
        {
            var themes = await _db.Themes.ToListAsync();

            return themes.Select(t => new ThemeDto
            {
                Name = t.Name
            });
        }
    }
}
