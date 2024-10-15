using PBS.Blazor.ServerFramework;
using PBS.DataAccess.Core;

namespace PBS.DigitalServiceSuite.Server.Integrations
{
    public class WebAppointmentsIntegration
    {
        public static string GetShopBanner(string serialNumber, Guid shopRef)
        {
            if (serialNumber.IsEmpty() || shopRef == Guid.Empty) return string.Empty;

            try
            {
                var sql = new SqlQueryBuilder() { ConnectionString = Utilities.Utility.WebAppointmentsDBConnectionString() };

                sql.Append("SELECT fldImgData");
                sql.Append("FROM tblBanner WITH (NOLOCK)");
                sql.Append("WHERE fldSerialNumber = {0}", serialNumber);
                sql.Append("AND fldShopRef = {0}", shopRef);

                using (var dr = DataManager.ExecuteAristoReader(sql))
                {
                    if (dr.Read() && !dr.IsDBNull("fldImgData"))
                    {
                        byte[] imgData = (byte[])dr.GetValue("fldImgData");
                        return Convert.ToBase64String(imgData);
                    }
                }

                sql = new SqlQueryBuilder() { ConnectionString = Utilities.Utility.InvoiceHubDBConnectionString() };

                sql.Append("SELECT fldImgData");
                sql.Append("FROM OP_tblOnlinePaymentSetting WITH (NOLOCK)");
                sql.Append("WHERE fldSerialNo = {0}", serialNumber.ToUpper().Replace(".QA", ""));

                using (var dr = DataManager.ExecuteAristoReader(sql))
                {
                    if (dr.Read() && !dr.IsDBNull("fldImgData"))
                    {
                        byte[] imgData = (byte[])dr.GetValue("fldImgData");
                        return Convert.ToBase64String(imgData);
                    }
                }
            }
            catch (Exception ex)
            {
                var log = new Activity(serialNumber, "ShopBannerFetch");

                log.LogMessage("Ran into an exception when trying to fetch the shop banner");
                log.LogException(ex);
                log.Update();
            }

            return string.Empty;
        }
    }
}
