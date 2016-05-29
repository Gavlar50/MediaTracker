using System;
using System.Data.SqlClient;
using System.Linq;
using Gavlar50.Umbraco.MediaTracker.Models;
using Umbraco.Core.Logging;
using Umbraco.Web;

namespace Gavlar50.Umbraco.MediaTracker.DbProviders
{
   public class SqlProvider : IMediaTrackerDbProvider
   {
      public SqlProvider(string dbConnString)
      {
         DbConnString = dbConnString;
      }

      public string DbConnString { get; set; }

      public void AddMediaTrack(MediaTrackerModel track)
      {
         var existsSql = string.Format("select * from mediatracker where mediaid={0} and contentid={1} and propertyid={2}",
            track.MediaId, track.ContentId, track.PropertyId);

         using (var conn = new SqlConnection(DbConnString))
         {
            try
            {
               conn.Open();
               var sqlCommand = new SqlCommand(existsSql, conn);
               var rdr = sqlCommand.ExecuteReader();
               var exists = rdr.HasRows;
               rdr.Close();
               if (exists) return;
               var insertSql =
                  string.Format(
                     "insert into mediatracker (mediaid,contentid,propertyid) values ({0},{1},{2})",
                     track.MediaId, track.ContentId, track.PropertyId);
               var comm = new SqlCommand(insertSql, conn);
               comm.ExecuteNonQuery();
               conn.Close();
            }
            catch (SqlException e)
            {
               LogHelper.Error<SqlException>(e.Message, e);
            }
         }
      }

      public void RemoveMediaTrack(int contentId)
      {
         using (var conn = new SqlConnection(DbConnString))
         {
            try
            {
               conn.Open();
               var deleteSql =
                  string.Format(
                     "delete from mediatracker where contentid={0}",
                     contentId);
               var comm = new SqlCommand(deleteSql, conn);
               comm.ExecuteNonQuery();
               conn.Close();
            }
            catch (SqlException e)
            {
               LogHelper.Error<SqlException>(e.Message, e);
            }
         }
      }

      public int GetMediaIdFromUrl(string url)
      {
         var id = 0;
         using (var conn = new SqlConnection(DbConnString))
         {
            try
            {
               conn.Open();
               var sql = string.Format(
                  "select contentnodeid from cmspropertydata d inner join cmspropertytype t on d.propertytypeid=t.id and t.alias='umbracoFile' and d.datanvarchar='{0}'",
                  url);
               var comm = new SqlCommand(sql, conn);
               var rdr = comm.ExecuteReader();
               if (rdr.HasRows)
               {
                  rdr.Read();
                  id = Convert.ToInt32(rdr["contentnodeid"].ToString());
                  rdr.Close();
               }
               conn.Close();
            }
            catch (SqlException e)
            {
               LogHelper.Error<SqlException>(e.Message, e);
            }
         }
         return id;
      }
   }
}
