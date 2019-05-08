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

        public const string InvalidThemesRequired = "invalid_themes_required";
        public const string InvalidThemesMin = "invalid_themes_min";
        public const string InvalidThemesMax = "invalid_themes_max";

        public const string InvalidAvailableLetters = "invalid_available_letters";
        public const string InvalidAvailableLettersRequired = "invalid_available_letters_required";
        public const string InvalidAvailableLettersMin = "invalid_available_letters_min";
        public const string InvalidAvailableLettersMax = "invalid_available_letters_max";

        public const string NameAlreadyExists = "name_already_exists";
        public const string GameRoomNotFound = "game_room_not_found";
        public const string InvalidPasswordRequired = "invalid_passsword_required";
        public const string NotAdmin = "not_admin";

    }
}
