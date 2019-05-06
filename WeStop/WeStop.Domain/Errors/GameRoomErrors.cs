namespace WeStop.Domain.Errors
{
    public class GameRoomErrors
    {
        public const string IsFull = "is_full";

        public const string InvalidNameRequired = "invalid_name_required";
        public const string InvalidNameMaxLength = "invalid_name_maxlength";

        public const string InvalidPasswordMinLength = "invalid_password_minlength";
        public const string InvalidPasswordMaxLength = "invalid_password_maxlength";

        public const string InvalidNumberOfRoundsRequired = "invalid_number_of_rounds_required";
        public const string InvalidNumberOfRoundsMin = "invalid_number_of_rounds_min";
        public const string InvalidNumberOfRoundsMax = "invalid_number_of_rounds_max";

        public const string InvalidNumberOfPlayersRequired = "invalid_number_of_players_required";
        public const string InvalidNumberOfPlayersMin = "invalid_number_of_players_min";
        public const string InvalidNumberOfPlayersMax = "invalid_number_of_players_max";

        public const string NameAlreadyExists = "name_already_exists";
    }
}
