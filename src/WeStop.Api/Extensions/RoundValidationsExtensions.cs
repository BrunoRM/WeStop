using System;
using System.Collections.Generic;
using System.Linq;
using WeStop.Api.Core;

namespace WeStop.Api.Extensions
{
    public static class RoundValidationsExtensions
    {
        public static int GetValidVotesCountForAnswer(this IEnumerable<RoundValidations> roundValidations, Answer answer) =>
            roundValidations.Count(rv => rv.Validations.Any(v => v.Answer == answer && v.Valid));

        public static int GetInvalidVotesCountForAnswer(this IEnumerable<RoundValidations> roundValidations, Answer answer) =>
            roundValidations.Count(rv => rv.Validations.Any(v => v.Answer == answer && !v.Valid));

        public static bool HasValidatiosOfPlayer(this IEnumerable<RoundValidations> roundValidations, Guid playerId) =>
            roundValidations.Any(rv => rv.PlayerId == playerId);
    }
}
