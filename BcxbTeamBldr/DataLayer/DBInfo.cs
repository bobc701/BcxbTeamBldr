using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data;
using System.Data.SqlClient;
using System.Configuration;

using BcxbTeamBldr.Models;

namespace BcxbTeamBldr.DataLayer {

   public static class DbInfo {
      // This class, DBInfo, is responsible for all intereaction with the database.

      public static SqlConnection con1;

      static DbInfo() {
         // -----------------------------------------
         string connect = ConfigurationManager.ConnectionStrings["ConnectionString1"].ConnectionString;
         con1 = new SqlConnection(connect);
         con1.Open();
      }

      public static List<CUserTeam> GetUserTeamList(string user) {
         // --------------------------------------------------------------------
         // Purpose: Retrieve from database, a list of all teams for this user.

         var list = new List<CUserTeam>();
         //string sql = $"SELECT * FROM UserTeams WHERE UserName = '{user}'";
         string sql = $"EXEC GetUserTeamList '{user}'";
         using (var cmd = new SqlCommand(sql, con1)) {

            using (SqlDataReader rdr = cmd.ExecuteReader()) {
               while (rdr.Read()) {
                  var team = new CUserTeam();
                  team.UserName = user;
                  team.TeamName = rdr["TeamName"].ToString(); 
                  team.NumPit = (int)rdr["TotPit"];
                  team.NumPos = (int)rdr["TotPos"];
                  team.UsesDh = (bool)rdr["UsesDh"];
                  list.Add(team);
               }

            }
            int n = cmd.ExecuteNonQuery();
         }
         return list;
      }


      public static void AddNewTeam(string user, string team, bool dh) {
         // ---------------------------------------------------------------------
         string sql = $"EXEC AddNewTeam '{user}', '{team}', {(dh ? '1' : '0')}";
         using (var cmd = new SqlCommand(sql, con1)) {
            cmd.ExecuteNonQuery();
         }
         //GetUserTeamList(user);

      }


      public static int AddNewUser(string user, string pwd) {
         // ----------------------------------------------------------
         int n;
         //string sql = $"EXEC AddNewUser '{user}', '{pwd}'";

         //using (var cmd = new SqlCommand(sql, con1)) {
         using (SqlCommand cmd = new SqlCommand("u196491_BcxbUser.AddNewUser", con1)) {
            cmd.CommandType = CommandType.StoredProcedure;

         // Add parameter for return value...
            SqlParameter returnValue = new SqlParameter();
            returnValue.Direction = ParameterDirection.ReturnValue;
            cmd.Parameters.Add(returnValue);

         // Add input parameters...
            cmd.Parameters.AddWithValue("@user", user);
            cmd.Parameters.AddWithValue("@pwd", pwd);
            
            cmd.ExecuteNonQuery(); 
            n = (int)returnValue.Value;
         }
         return n;

      }

      public static int Login(string user, string pwd) {
         // ----------------------------------------------------------
         int n;

         //using (var cmd = new SqlCommand(sql, con1)) {
         using (SqlCommand cmd = new SqlCommand("u196491_BcxbUser.Login", con1)) {
            cmd.CommandType = CommandType.StoredProcedure;

            // Add parameter for return value...
            SqlParameter loginResult = new SqlParameter();
            loginResult.Direction = ParameterDirection.ReturnValue;
            cmd.Parameters.Add(loginResult);

            // Add input parameters...
            cmd.Parameters.AddWithValue("@user", user);
            cmd.Parameters.AddWithValue("@pwd", pwd);

            cmd.ExecuteNonQuery();
            n = (int)loginResult.Value;
         }
         return n;

      }



      public static void AddPlayerToTeam(string user, string team, int id) {
         // --------------------------------------------------------------------
         string sql = $"EXEC AddPlayerToTeam '{user}', '{team}', {id}";
         using (var cmd = new SqlCommand(sql, con1)) {
            cmd.ExecuteNonQuery();
         }
         //GetUserTeamList(user);
      }


      public static void RemovePlayerFromTeam(string user, string team, int id) {
         // --------------------------------------------------------------------
         string sql = $"EXEC RemovePlayerFromTeam '{user}', '{team}', {id}";
         using (var cmd = new SqlCommand(sql, con1)) {
            cmd.ExecuteNonQuery();
         }
         //GetUserTeamList(user);
      }


      public static bool TeamNameExists(string user, string team) {
      // ------------------------------------------------------------------
         string sql = $"EXEC TeamNameExists '{user}', '{team}'";
         using (var cmd = new SqlCommand(sql, con1)) {
            return (int)cmd.ExecuteScalar() == 1 ? true : false;
         }

      }


      public static List<CMlbPlayer> GetPlayerList (string user, string team) {
      // ---------------------------------------------------------------------
         var list = new List<CMlbPlayer>();
         string sql = $"EXEC GetPlayerList '{user}', '{team}'";
         using (var cmd = new SqlCommand(sql, con1)) {

            using (SqlDataReader rdr = cmd.ExecuteReader()) {
               while (rdr.Read()) {
                  var player = new CMlbPlayer();
                  player.PlayerId = (int)rdr["PlayerId"];
                  player.PlayerName = rdr["PlayerName"].ToString();
                  player.PlayerType = rdr["PlayerType"].ToString()[0];
                  player.FieldingString = rdr["FieldingString"].ToString();
                  player.Year = (int)rdr["Year"];
                  player.MlbTeam = rdr["MlbTeam"].ToString();
                  player.MlbLeague = rdr["MlbLeague"].ToString(); ;

                  list.Add(player);
               }

            }
         }
         return list;

      }


   public static List<CMlbPlayer> SearchPlayers(string crit) {
      // ---------------------------------------------------------------------
      var list = new List<CMlbPlayer>();
      string sql = $"EXEC SearchPlayers '{crit}'";
      using (var cmd = new SqlCommand(sql, con1)) {

         using (SqlDataReader rdr = cmd.ExecuteReader()) {
            while (rdr.Read()) {
               var player = new CMlbPlayer();
               player.PlayerId = (int)rdr["PlayerId"];
               player.PlayerName = rdr["PlayerName"].ToString();
               player.PlayerType = rdr["PlayerType"].ToString()[0];
               player.FieldingString = rdr["FieldingString"].ToString();
               player.Year = (int)rdr["Year"];
               player.MlbTeam = rdr["MlbTeam"].ToString();
               player.MlbLeague = rdr["MlbLeague"].ToString(); ;

               list.Add(player);
            }

         }
      }
      return list;

   }

}



}