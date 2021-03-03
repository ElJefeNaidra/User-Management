namespace SIDPSF.Common
{
    public class Validation_BBAN
    {
        public static bool IsValid(string BBAN)
        {
            if (BBAN.Length == 0) // Check if BBAN is null
            {
                return true;
            }
            else
            {
                if (BBAN.Length != 16) // Check if BBAN is not 16 chars long
                {
                    return false;
                }
                else
                {
                    string _BBANWOChecksum = BBAN.Substring(0, 14); // Take first 14 digits
                    string _Checksum = BBAN.Substring(14, 2); // Take last two digits
                    string _BBANFull = _BBANWOChecksum + "00";
                    double x = Convert.ToInt64(_BBANFull) / 97;
                    long _Base = Convert.ToInt64(Math.Floor(x));
                    _Base = _Base * 97;
                    _Base = Convert.ToInt64(_BBANFull) - _Base;
                    _Base = 98 - _Base;
                    if (Convert.ToInt64(_Checksum) == _Base)
                    {
                        return true;
                    }
                    else
                    {
                        return false;
                    }
                }
            }
        }
    }
}
