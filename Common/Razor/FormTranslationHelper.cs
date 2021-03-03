using SIDPSF.Common.Generics;
using static SIDPSF.Common.StringLocalisation.DatabaseResourceLocalisationProvider;
using static SIDPSF.Common.DataAccess.DBContext;

namespace SIDPSF.Common.Razor
{
    public class FormTranslationHelper
    {
        public static string CrudOpForTitle(CrudOp crudOp, ResourceRepository _resourceRepo)
        {
            string CrudOpForTitle = "";

            switch (crudOp)
            {
                case CrudOp.Create:
                    CrudOpForTitle = _resourceRepo.GetResourceByName("Create");
                    break;

                case CrudOp.Update:
                    CrudOpForTitle = _resourceRepo.GetResourceByName("Update");
                    break;

                case CrudOp.Read:
                    CrudOpForTitle = _resourceRepo.GetResourceByName("Read");
                    break;

                case CrudOp.ReadHistory:
                    CrudOpForTitle = _resourceRepo.GetResourceByName("ReadHistory");
                    break;

                case CrudOp.Enable:
                    CrudOpForTitle = _resourceRepo.GetResourceByName("Enable");
                    break;

                case CrudOp.Disable:
                    CrudOpForTitle = _resourceRepo.GetResourceByName("Disable");
                    break;

                case CrudOp.Delete:
                    CrudOpForTitle = _resourceRepo.GetResourceByName("Delete");
                    break;

                case CrudOp.Uncancel:
                    CrudOpForTitle = _resourceRepo.GetResourceByName("Uncancel");
                    break;

                case CrudOp.Return:
                    CrudOpForTitle = _resourceRepo.GetResourceByName("Return");
                    break;

                case CrudOp.Resend:
                    CrudOpForTitle = _resourceRepo.GetResourceByName("Resend");
                    break;

                case CrudOp.Print:
                    CrudOpForTitle = _resourceRepo.GetResourceByName("Print");
                    break;

                case CrudOp.Cancel:
                    CrudOpForTitle = _resourceRepo.GetResourceByName("Cancel");
                    break;

                case CrudOp.Calculate:
                    CrudOpForTitle = _resourceRepo.GetResourceByName("Calculate");
                    break;

                case CrudOp.Search:
                    CrudOpForTitle = _resourceRepo.GetResourceByName("Search");
                    break;

                case CrudOp.None:
                    CrudOpForTitle = null;
                    break;

                case CrudOp.Manage:
                    CrudOpForTitle = _resourceRepo.GetResourceByName("Manage");
                    break;


                default:
                    break;
            }

            return CrudOpForTitle;
        }

        public static string TableNameDisplay(string TableName)
        {
            string TableNameDisplay = "";

            var obj = GenericGetDdlData.GetDdl("[Administration].[Authorisation]", "IdAuthorisation", $"EnDescription='{TableName}'");

            foreach (var item in obj)
            {
                TableNameDisplay = item.Description;
            }

            return TableNameDisplay;
        }


    }
}
