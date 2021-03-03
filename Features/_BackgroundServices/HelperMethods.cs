using System.Text;

namespace UserManagement.Features._BackgroundServices
{
    public class HelperMethods
    {
        private static Random random = new Random();

        public static string GenerateSRID(int length)
        {
            const string chars = "0123456789"; // Only digits
            var stringBuilder = new StringBuilder("SRID_");

            for (int i = 0; i < length; i++)
            {
                stringBuilder.Append(chars[random.Next(chars.Length)]);
            }

            return stringBuilder.ToString();
        }
    }
}
