namespace SIDPSF.Common.Generics
{
    public class GenericGetNonStandardDdlDataModel
    {
        // Item Id Value
        public int Id { get; set; }

        // Item description
        public string Description { get; set; }

        //// Row Checksum Generated from checksum of total columns to check if the remote lookup table 
        //// has updated data on this row
        //public string Checksum { get; set; }

        //// This property should be set from outside this model, probably from a service or controller
        //// Item Encrypted Id value generated from SessionID to be used for preventing fake values submission using 
        //// inspect element
        //public string IdValueEncrypted => GetEncryptedIdValue();

        //private string GetEncryptedIdValue()
        //{
        //    string sessionId = _accessor.HttpContext.Session.Id;

        //    if (Id != 0)
        //    {
        //        return EncryptionDecryptionRoutines.Encrypt(Id.ToString(), "Astrolite$2023"); //Modify here if you want strong password
        //    }

        //    return string.Empty;
        //}
    }
}
