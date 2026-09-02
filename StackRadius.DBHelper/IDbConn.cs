using System.Data;

namespace StackRadius.DBHelper
{
    public interface IDbConn : IDbConnection
    {
        string QueryText { get; set; }

        IDbTransaction DBTransaction { get; }
    }
}
