using MongoDB.Bson.Serialization;
using MongoDB.Driver;
using System.Linq;
using WeStop.Api.Core;

namespace WeStop.Api.Infra.Storages.MongoDb
{
    public class MongoContext
    {
        private readonly IMongoDatabase _database;

        public MongoContext(string connectionString)
        {
            BsonClassMap.RegisterClassMap<Game>(cm =>
            {
                cm.MapMember(x => x.Id)
                    .SetElementName("id");

                cm.MapMember(x => x.Name)
                    .SetElementName("name");

                cm.MapMember(x => x.Password)
                    .SetElementName("password");

                cm.MapMember(x => x.Options)
                    .SetElementName("options");

                cm.MapMember(x => x.State)
                    .SetElementName("state");

                cm.MapMember(x => x.Rounds)
                    .SetElementName("rounds");

                cm.MapMember(x => x.Players)
                    .SetShouldSerializeMethod(x => false) // Para ignorar a propriedade no insert e update
                    .SetIsRequired(false);
            });

            BsonClassMap.RegisterClassMap<GameOptions>(cm =>
            {
                cm.MapMember(x => x.Rounds)
                    .SetElementName("rounds");

                cm.MapMember(x => x.NumberOfPlayers)
                    .SetElementName("number_of_players");

                cm.MapMember(x => x.RoundTime)
                    .SetElementName("round_time");

                cm.MapMember(x => x.AvailableLetters)
                    .SetElementName("available_letters");

                cm.MapMember(x => x.Themes)
                    .SetElementName("themes");
            });

            BsonClassMap.RegisterClassMap<Round>(cm =>
            {
                cm.MapMember(x => x.GameId)
                    .SetElementName("game_id");

                cm.MapMember(x => x.Number)
                    .SetElementName("number");

                cm.MapMember(x => x.SortedLetter)
                    .SetElementName("sorted_letter");

                cm.MapMember(x => x.ThemeBeingValidated)
                    .SetElementName("theme_being_validated");

                cm.MapMember(x => x.Finished)
                    .SetElementName("finished");

                cm.MapMember(x => x.ValidatedThemes)
                    .SetElementName("validated_themes");
            });

            BsonClassMap.RegisterClassMap<Player>(cm =>
            {
                cm.MapMember(x => x.User)
                    .SetElementName("user");

                cm.MapMember(x => x.Id)
                    .SetElementName("id");

                cm.MapMember(x => x.UserName)
                    .SetElementName("username");

                cm.MapMember(x => x.ImageUri)
                    .SetElementName("image_uri");

                cm.MapMember(x => x.GameId)
                    .SetElementName("game_id");

                cm.MapMember(x => x.IsReady)
                    .SetElementName("is_ready");

                cm.MapMember(x => x.InRound)
                    .SetElementName("in_round");

                cm.MapMember(x => x.IsAdmin)
                    .SetElementName("is_admin");

                cm.MapMember(x => x.IsOnline)
                    .SetElementName("is_online");

                cm.MapMember(x => x.Answers)
                    .SetElementName("answers");

                cm.MapMember(x => x.Validations)
                    .SetElementName("validations");

                cm.MapMember(x => x.Pontuations)
                    .SetElementName("pontuations");

                cm.UnmapMember(x => x.TotalPontuation);
            });

            BsonClassMap.RegisterClassMap<RoundAnswers>(cm =>
            {
                cm.MapMember(x => x.PlayerId)
                    .SetElementName("player_id");

                cm.MapMember(x => x.GameId)
                    .SetElementName("game_id");

                cm.MapMember(x => x.RoundNumber)
                    .SetElementName("round_number");

                cm.MapMember(x => x.Answers)
                    .SetElementName("round_number");

                cm.MapMember(x => x.Answers)
                    .SetElementName("answers");
            });

            BsonClassMap.RegisterClassMap<Answer>(cm =>
            {
                cm.MapMember(x => x.Theme)
                    .SetElementName("theme");

                cm.MapMember(x => x.Value)
                    .SetElementName("value");
            });

            BsonClassMap.RegisterClassMap<RoundValidations>(cm =>
            {
                cm.MapMember(x => x.PlayerId)
                    .SetElementName("player_id");

                cm.MapMember(x => x.GameId)
                    .SetElementName("game_id");

                cm.MapMember(x => x.RoundNumber)
                    .SetElementName("round_number");

                cm.MapMember(x => x.Theme)
                    .SetElementName("theme");

                cm.MapMember(x => x.Validations)
                    .SetElementName("validations");
            });

            BsonClassMap.RegisterClassMap<Validation>(cm =>
            {
                cm.MapMember(x => x.Answer)
                    .SetElementName("answer");

                cm.MapMember(x => x.Valid)
                    .SetElementName("is_valid");
            });

            BsonClassMap.RegisterClassMap<RoundPontuations>(cm =>
            {
                cm.MapMember(x => x.GameId)
                    .SetElementName("game_id");

                cm.MapMember(x => x.PlayerId)
                    .SetElementName("player_id");

                cm.MapMember(x => x.RoundNumber)
                    .SetElementName("round_number");

                cm.MapMember(x => x.ThemesPontuations)
                    .SetElementName("themes_pontuations");

                cm.UnmapMember(x => x.TotalPontuation);
            });

            BsonClassMap.RegisterClassMap<ThemePontuation>(cm =>
            {
                cm.MapMember(x => x.Theme)
                    .SetElementName("theme");

                cm.MapMember(x => x.Pontuation)
                    .SetElementName("pontuation");
            });

            BsonClassMap.RegisterClassMap<User>(cm =>
            {
                cm.MapMember(x => x.Id)
                    .SetElementName("id");

                cm.MapMember(x => x.UserName)
                    .SetElementName("username");

                cm.MapMember(x => x.ImageUri)
                    .SetElementName("image_uri");
            });

            var client = new MongoClient(connectionString);
            _database = client.GetDatabase("westop");
        }

        public IMongoCollection<Game> GamesCollection => _database.GetCollection<Game>(Consts.GAMES_COLLECTION_NAME);
        public IMongoCollection<Player> PlayersCollection => _database.GetCollection<Player>(Consts.PLAYERS_COLLECTION_NAME);
    }
}