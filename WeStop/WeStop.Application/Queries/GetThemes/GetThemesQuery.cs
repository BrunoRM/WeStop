using MediatR;
using System;
using System.Collections.Generic;
using System.Text;
using WeStop.Application.Dtos.Theme;

namespace WeStop.Application.Queries.GetThemes
{
    public class GetThemesQuery : IRequest<IEnumerable<ThemeDto>>
    {
    }
}
