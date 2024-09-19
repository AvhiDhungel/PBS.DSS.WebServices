using PBS.DataAccess.Core;
using System.Text;

namespace PBS.DSS.WebServices.Server.Utilities
{
    internal class Activity
    {
        private readonly StringBuilder Log = new();

        public int Id { get; set; } = 0;
        public string SerialNumber { get; set; } = string.Empty;
        public string Type { get; set; } = string.Empty;
        public Guid WorkItemRef { get; set; } = Guid.Empty;
        public DateTime Date { get; set; } = DateTime.UtcNow;

        public string LogText { get => Log.ToString(); }

        private Activity() { }

        public Activity(string serial, string type)
        {
            SerialNumber = serial;
            Type = type;
        }

        public void LogMessage(string msg)
        {
            Log.AppendLine(msg);
        }

        public void LogException(Exception ex)
        {
            Log.AppendLine(" ");
            Log.AppendLine("Ran into an exception:");
            Log.AppendLine(ex.Message);
            Log.AppendLine(" ");
        }

        #region Data Access
        public static Activity Fetch(int id)
        {
            var a = new Activity();
            var sql = new SqlQueryBuilder();

            sql.Append("SELECT * FROM tblActivity");
            sql.Append("WHERE fldId = {0}", id);

            using (var dr = DataManager.ExecuteAristoReader(sql))
            {
                if (dr.Read())
                {
                    a.Id = dr.GetInt32("fldId");
                    a.SerialNumber = dr.GetString("fldSerialNumber");
                    a.Type = dr.GetString("fldType");
                    a.WorkItemRef = dr.GetGuid("fldWorkItemRef");
                    a.Date = dr.GetDateTime("fldDate");
                    a.Log.Append(dr.GetString("fldLog"));
                }
            }

            return a;
        }

        public void Update()
        {
            try
            {
                var sql = new SqlQueryBuilder();

                sql.Append("SELECT * FROM tblActivity");
                sql.Append("WHERE fldId = {0}", Id);

                var fields = new Dictionary<string, object>();
                fields["fldId"] = Id;
                fields["fldSerialNumber"] = SerialNumber;
                fields["fldType"] = Type;
                fields["fldWorkItemRef"] = WorkItemRef;
                fields["fldDate"] = Date;
                fields["fldLog"] = LogText;

                Id = DataManager.ExecuteInsertUpdate(sql, fields);
            }
            catch (Exception ex)
            {

            }
        }
        #endregion
    }
}
