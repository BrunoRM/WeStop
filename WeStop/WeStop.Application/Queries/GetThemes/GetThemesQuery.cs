using MediatR;
using System.Collections.Generic;

namespace WeStop.Application.Queries.GetThemes
{
    public class GetThemesQuery : IRequest<IEnumerable<string>>
    {
    }
}
