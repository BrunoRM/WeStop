namespace WeStop.Domain.Errors
{
    public class PlayerErrors
    {
        public const string InvalidNameRequired = "invalid_name_required";
        public const string InvalidNameMaxLength = "invalid_name_maxlength";

        public const string InvalidUserNameRequired = "invalid_username_required";
        public const string InvalidUserNameMaxLength = "invalid_username_maxlength";

        public const string InvalidEmailRequired = "invalid_email_required";
        public const string InvalidEmail = "invalid_email";

        public const string InvalidPasswordRequired = "invalid_password_required";
        public const string InvalidPasswordMinLength = "invalid_password_minlength_required";

        public const string UserNameAlreadyTaken = "username_already_taken";
        public const string EmailAlreadyTaken = "email_already_taken";
    }
}
