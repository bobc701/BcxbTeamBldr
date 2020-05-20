using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data;
using System.Data.SqlClient;
using System.Configuration;

namespace BcxbTeamBldr.DataLayer {

   public static class TalkToDB {


      public static string GetConnectionString(string connectionName = "ConnectionString1") {
      // ----------------------------------------------------------------------------
         return ConfigurationManager.ConnectionStrings[connectionName].ConnectionString;

      }


      public static SqlDataReader GetReader(string sql) {
         // -----------------------------------------------------
         using var con = new SqlConnection(GetConnectionString());
         con.Open();
         using var cmd = new SqlCommand(sql, con);
         SqlDataReader rdr = cmd.ExecuteReader();
         return rdr;

      }

      public static void ExecuteSql(string sql) {
      // -----------------------------------------------------------
         using var con = new SqlConnection(GetConnectionString());
         using var cmd = new SqlCommand(sql, con);
         int nRecs = cmd.ExecuteNonQuery();

      }
   }

}