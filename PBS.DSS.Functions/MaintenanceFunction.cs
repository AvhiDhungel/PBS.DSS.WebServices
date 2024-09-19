using System;
using System.Threading.Tasks;
using Microsoft.Azure.WebJobs;
using Microsoft.Extensions.Logging;
using PBS.DataAccess.Core;
using PBS.WebserviceMaintenance.Client;

namespace PBS.DSS.Functions
{
    public class MaintenanceFunction
    {
        [FunctionName("DatabaseMaintenance")]
        public async Task Run([TimerTrigger("0 0 9 * * 0")] TimerInfo myTimer, ILogger log)
        {
            log.LogInformation($"C# Timer trigger function executed at: {DateTime.Now}");

            ConfigurationManager.GetConnectionString = Utility.GetConnectionString;

            using (var c = await DatabaseMaintenance.Fetch("DSSWebService", "DatabaseMaintenance"))
            {
                try
                {
                    var sql = new SqlQueryBuilder() { TimeOut = 60 * 60 * 2 };
                    sql.Append(@"
    					DECLARE @CurrentMinId int
                        SET @CurrentMinId = (SELECT MIN(fldId) FROM tblActivity)

                        DECLARE @CurrentMinDate datetime
                        SET @CurrentMinDate = (SELECT [fldDate] FROM tblActivity WHERE fldId = @CurrentMinId)

                        WHILE (@CurrentMinDate < DATEADD(MONTH, -1, GETDATE()))
                        BEGIN
	
	                        WITH CTE AS
                            (
		                        SELECT TOP 200 *
                                FROM tblActivity
                                ORDER BY fldId ASC
                            )
	                        DELETE FROM CTE

	                        SET @CurrentMinId = (SELECT MIN(fldId) FROM tblActivity)
	                        SET @CurrentMinDate = (SELECT [fldDate] FROM tblActivity WHERE fldId = @CurrentMinId)
                        END");

                    c.TryExecuteNonQuery(sql);
                }
                catch (Exception ex)
                {
                    c.WriteLog(ex.ToString());
                    c.WasSuccessful = false;
                }

                log.LogInformation("Database Maintenance Complete");
            }
        }
    }
}
