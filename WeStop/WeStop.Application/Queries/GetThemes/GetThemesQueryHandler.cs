using MediatR;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using WeStop.Infra;

namespace WeStop.Application.Queries.GetThemes
{
    public class GetThemesQueryHandler : IRequestHandler<GetThemesQuery, IEnumerable<string>>
    {
        private readonly WeStopDbContext _db;

        public GetThemesQueryHandler(WeStopDbContext db)
        {
            _db = db;
        }

        public async Task<IEnumerable<string>> Handle(GetThemesQuery request, CancellationToken cancellationToken)
        {
            var themes = await _db.Themes.ToListAsync();

            return themes.Select(t => t.Name);
        }
    }
}
