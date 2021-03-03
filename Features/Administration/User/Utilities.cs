namespace UserManagement.Features.Administration.User
{
    public class Utilities
    {
        public static bool PasswordIsSQLI(string password)
        {
            // Check if the password contains any disallowed special characters
            return password.Any(ch => !char.IsLetterOrDigit(ch) && !"#@$!".Contains(ch));
        }
    }
}
