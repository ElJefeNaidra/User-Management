namespace SIDPSF.Common.DataAccess
{
    public partial class DBContext
    {

        /// <summary>
        /// Enumerates the types of CRUD (Create, Read, Update, Delete) operations and other common actions.
        /// </summary>
        public enum CrudOp
        {
            None,
            Create,
            Read,
            Update,
            Delete,
            Enable,
            Disable,
            ChangeStatus,
            Execute,
            Print,
            History,
            ReadHistory,
            Cancel,
            Uncancel,
            Resend,
            Return,
            Calculate,
            Search,
            Manage,
            Reimburse
        }
    }
}
