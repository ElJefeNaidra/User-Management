namespace SIDPSF.Common
{
    public class Validation_RKSID
    {
        public static bool IsValid(string PersonalNo)
        {
            if (PersonalNo.Length != 10 || !char.IsDigit(PersonalNo[9]))
            {
                return false;
            }

            var Weights = new[] { 4, 3, 2, 7, 6, 5, 4, 3, 2 };
            var Result = 0;
            for (int i = 0; i < 9; i++)
            {
                var IdChar = PersonalNo[i];
                if (!char.IsDigit(IdChar))
                {
                    return false;
                }

                Result += (IdChar - 48) * Weights[i];
            }

            return (11 - Result % 11) % 10 == PersonalNo[9] - 48;
        }
    }
}
