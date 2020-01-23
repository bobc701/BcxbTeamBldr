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
         using (SqlCommand cmd = new SqlCommand("Login", con1)) {
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



      public static void AddPlayerToTeam(string user, string team, string id) {
         // --------------------------------------------------------------------
         string sql = $"EXEC AddPlayerToTeam '{user}', '{team}', '{id}'";
         using (var cmd = new SqlCommand(sql, con1)) {
            cmd.ExecuteNonQuery();
         }
         //GetUserTeamList(user);
      }


      public static void RemovePlayerFromTeam(string user, string team, string id) {
         // --------------------------------------------------------------------
         string sql = $"EXEC RemovePlayerFromTeam '{user}', '{team}', '{id}'";
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


      public static List<CMlbPlayer> GetPlayerList(string user, string team) {
         // ---------------------------------------------------------------------
         var list = new List<CMlbPlayer>();
         string sql = $"EXEC GetPlayerList '{user}', '{team}'";
         using (var cmd = new SqlCommand(sql, con1)) {

            using (SqlDataReader rdr = cmd.ExecuteReader()) {
               while (rdr.Read()) {
                  var player = new CMlbPlayer();
                  player.PlayerId = rdr["PlayerId"].ToString();
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


      public static List<CUserPlayer> GetUserPlayerList(string user, string team) {
         // ---------------------------------------------------------------------
         var list = new List<CUserPlayer>();
         string sql = $"EXEC GetPlayerList '{user}', '{team}'";
         using (var cmd = new SqlCommand(sql, con1)) {

            using (SqlDataReader rdr = cmd.ExecuteReader()) {
               while (rdr.Read()) {
                  var player = new CUserPlayer();
                  player.PlayerId = rdr["PlayerId"].ToString();
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
                  player.PlayerId = rdr["PlayerId"].ToString();
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


      public static List<CMlbPlayer> SearchPlayersMulti(string critName, string critTeam, string critYear, string critPosn) {
         // ---------------------------------------------------------------------
         var list = new List<CMlbPlayer>();

         // Build search string for SQL select...
         string crit = "", delim = "";
         if (critName != "All") { crit = $"PlayerName LIKE '%{critName}%'"; delim = " AND "; }
         if (critTeam != "All") { crit += delim + $"MlbTeam LIKE '%{critTeam}%'"; delim = " AND "; } // EG: 'NYA2019' LIKE '%NYA%'
         if (critYear != "All") { crit += delim + $"Year = '{critYear}'"; delim = " AND "; }

         switch (critPosn) {
            case "p": crit += delim + "SUBSTRING(FieldingString,1,1) != '-'"; break;
            case "c": crit += delim + "SUBSTRING(FieldingString,2,1) != '-'"; break;
            case "1b": crit += delim + "SUBSTRING(FieldingString,3,1) != '-'"; break;
            case "2b": crit += delim + "SUBSTRING(FieldingString,4,1) != '-'"; break;
            case "3b": crit += delim + "SUBSTRING(FieldingString,5,1) != '-'"; break;
            case "ss": crit += delim + "SUBSTRING(FieldingString,6,1) != '-'"; break;
            case "lf": crit += delim + "SUBSTRING(FieldingString,7,1) != '-'"; break;
            case "cf": crit += delim + "SUBSTRING(FieldingString,8,1) != '-'"; break;
            case "rf": crit += delim + "SUBSTRING(FieldingString,9,1) != '-'"; break;
            case "of":
               crit += delim +
                  "(SUBSTRING(FieldingString,7,1) != '-' OR SUBSTRING(FieldingString,8,1) != '-' OR SUBSTRING(FieldingString,9,1) != '-')";
               break;
         }

         string sql = $"SELECT * FROM MlbPlayers WHERE {crit}";
         //string sql = $"EXEC SearchPlayers '{crit}'";
         using (var cmd = new SqlCommand(sql, con1)) {

            using (SqlDataReader rdr = cmd.ExecuteReader()) {
               while (rdr.Read()) {
                  var player = new CMlbPlayer();
                  player.PlayerId = rdr["PlayerId"].ToString();
                  player.PlayerName = rdr["PlayerName"].ToString();
                  player.PlayerType = rdr["PlayerType"].ToString()[0];
                  player.FieldingString = rdr["FieldingString"].ToString();
                  player.Year = (int)rdr["Year"];
                  player.MlbTeam = rdr["MlbTeam"].ToString();
                  player.MlbLeague = rdr["MlbLeague"].ToString(); 

                  list.Add(player);
               }

            }
         }
         return list;

      }


      public static void SetLineup(string user, string team, Models.UserPlayerListVM model) {
      // ---------------------------------------------------------------------------------
      // First, delete all existing lineup data...
         string sql = $"EXEC ClearLineup '{user}', '{team}'";
         using (var cmd = new SqlCommand(sql, con1)) {
            cmd.ExecuteNonQuery();
         }

      // Now, update with new lineup data...
      // NOTE: This should probably be a transaction!!!
         foreach (CUserPlayer player in model.Players
            .Where(p => p.Slot_NoDH != 0 || p.Posn_NoDH != 0 || p.Slot_DH != 0 || p.Posn_DH != 0)) { 
               sql = $"EXEC SetLineup '{user}', '{team}', '{player.PlayerId}'";
               using var cmd = new SqlCommand(sql, con1);
               cmd.ExecuteNonQuery();
         }
      }

   }

}

