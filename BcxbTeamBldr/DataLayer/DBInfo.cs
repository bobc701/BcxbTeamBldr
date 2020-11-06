using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data;
using System.Data.SqlClient;
using System.Configuration;
using System.Diagnostics;

using BcxbTeamBldr.Models;

namespace BcxbTeamBldr.DataLayer {

   public class DbInfo {
      // This class, DBInfo, is responsible for all intereaction with the database.

      public SqlConnection con1;

      public DbInfo() {
         // -----------------------------------------
         string connect = ConfigurationManager.ConnectionStrings["ConnectionString1"].ConnectionString;
         con1 = new SqlConnection(connect);
         con1.Open();
         
      }


      public List<CUserTeam> GetUserTeamList(string user) {
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


      public void AddNewTeam(string user, string team, bool dh) {
      // ---------------------------------------------------------------------
         //string sql = $"EXEC AddNewTeam '{user}', '{team}', {(dh ? '1' : '0')}";
         string sql = $"INSERT INTO UserTeams (UserName, TeamName, UsesDh) VALUES ({user}, {team}, {(dh ? 1 : 0)})";
         using (var cmd = new SqlCommand(sql, con1)) {
            cmd.ExecuteNonQuery();
         }
         //GetUserTeamList(user);

      }


      public int AddNewUser(string user, string pwd) {
         // ----------------------------------------------------------
         int n;
         //string sql = $"EXEC AddNewUser '{user}', '{pwd}'";

         //using (var cmd = new SqlCommand(sql, con1)) {
         using (SqlCommand cmd = new SqlCommand("dbo.AddNewUser", con1)) {
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

      public int Login(string user, string pwd) {
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



      public void AddPlayerToTeam(string user, string team, string id) {
         // --------------------------------------------------------------------
         //string sql = $"EXEC AddPlayerToTeam '{user}', '{team}', '{id}'";
         string sql = $"INSERT INTO UserPlayers(UserName, TeamName, PlayerId) VALUES('{user}', '{team}', '{id}')

         using (var cmd = new SqlCommand(sql, con1)) {
            cmd.ExecuteNonQuery();
         }
      }


      public void UpdateLineups(string user, string team, List<CUserPlayer> roster) {
         // ----------------------------------------------------------------------------

         SqlTransaction sqlTran = null;
         try {
            
            sqlTran = con1.BeginTransaction();

            using SqlCommand cmd1 = new SqlCommand("RemovePlayerFromTeam", con1);
            cmd1.CommandType = CommandType.StoredProcedure;
            cmd1.Transaction = sqlTran;

            using SqlCommand cmd2 = new SqlCommand("UpdateLineups", con1);
            cmd2.CommandType = CommandType.StoredProcedure;
            cmd2.Transaction = sqlTran;

            foreach (CUserPlayer player in roster) {
               if (player.Remove) {
               // Player is flagged for removal...
                  cmd1.Parameters.Clear();
                  cmd1.Parameters.AddWithValue("@user", user);
                  cmd1.Parameters.AddWithValue("@team", team);
                  cmd1.Parameters.AddWithValue("@pid", player.PlayerId);
                  cmd1.ExecuteNonQuery();
               }
               else {
               // Player not flagged, so update lineups (NoDh & Dh)...
                  cmd2.Parameters.Clear();
                  cmd2.Parameters.AddWithValue("@user", user);
                  cmd2.Parameters.AddWithValue("@team", team);
                  cmd2.Parameters.AddWithValue("@pid", player.PlayerId);
                  cmd2.Parameters.AddWithValue("@slotNoDh", player.Slot_NoDH);
                  cmd2.Parameters.AddWithValue("@posnNoDh", player.Posn_NoDH);
                  cmd2.Parameters.AddWithValue("@slotDh", player.Slot_DH);
                  cmd2.Parameters.AddWithValue("@posnDh", player.Posn_DH);
                  cmd2.ExecuteNonQuery();
               }
            }
            sqlTran.Commit();
            Debug.WriteLine("Both records were written to database.");
         }
         catch (Exception ex) {
            Debug.WriteLine(ex.Message);

            try {
               sqlTran.Rollback();
               throw ex;
            }
            catch (Exception exRollback) {
               // Throws an InvalidOperationException if the connection 
               // is closed or the transaction has already been rolled 
               // back on the server.
               Console.WriteLine(exRollback.Message);
               throw exRollback;

            }
         }

      }


      public void RemovePlayerFromTeam(string user, string team, string id) {
         // --------------------------------------------------------------------
         string sql = $"EXEC RemovePlayerFromTeam '{user}', '{team}', '{id}'";
         using (var cmd = new SqlCommand(sql, con1)) {
            cmd.ExecuteNonQuery();
         }
         //GetUserTeamList(user);
      }


      public void RemoveAllPlayersFromTeam(string user, string team) {
         // --------------------------------------------------------------------
         string sql = $"EXEC RemoveAllPlayersFromTeam '{user}', '{team}'";
         using (var cmd = new SqlCommand(sql, con1)) {
            cmd.ExecuteNonQuery();
         }
         //GetUserTeamList(user);
      }


      public bool TeamNameExists(string user, string team) {
         // ------------------------------------------------------------------
         string sql = $"EXEC TeamNameExists '{user}', '{team}'";
         using (var cmd = new SqlCommand(sql, con1)) {
            return (int)cmd.ExecuteScalar() == 1 ? true : false;
         }

      }


      public List<CMlbPlayer> GetPlayerList(string user, string team) {
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
                  player.MlbLeague = rdr["MlbLeague"].ToString();

                  list.Add(player);
               }

            }
         }
         return list;

      }


      public List<CUserPlayer> GetUserPlayerList(string user, string team) {
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
                  player.MlbLeague = rdr["MlbLeague"].ToString();

                  player.Slot_NoDH = (int)rdr["Slot_NoDH"];
                  player.Posn_NoDH = (int)rdr["Posn_NoDH"];
                  player.Slot_DH = (int)rdr["Slot_DH"];
                  player.Posn_DH = (int)rdr["Posn_DH"];

                  list.Add(player);
               }

            }
         }
         return list;

      }


      public List<CMlbPlayer> SearchPlayers(string crit) {
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


      public List<CMlbPlayer> SearchPlayersMulti(string critName, string critTeam, string critYear, string critPosn) {
         // ---------------------------------------------------------------------
         const string posnMin = 5;
      
         var list = new List<CMlbPlayer>();

         // Build search string for SQL select...
         string crit = "", delim = "";
         if (critName != "All") { crit = $"PlayerName LIKE '%{critName}%'"; delim = " AND "; }
         if (critTeam != "All") { crit += delim + $"MlbTeam LIKE '%{critTeam}%'"; delim = " AND "; } // EG: 'NYA2019' LIKE '%NYA%'
         if (critYear != "All") { crit += delim + $"Year = '{critYear}'"; delim = " AND "; }

         if (critPosn != "All") { 
            crit += delim + critPosn switch {
               "p" => $"G_p > {posnMin}",
               "c" => $"G_c > {posnMin}",
               "1b" => $"G_1b > {posnMin}",
               "2b" => $"G_2b > {posnMin}",
               "3b" => $"G_3b > {posnMin}",
               "ss" => $"G_ss > {posnMin}",
               "lf" => $"G_lf > {posnMin}",
               "cf" => $"G_cf > {posnMin}",
               "rf" => $"G_rf > {posnMin}",
               "of" => $"G_of > {posnMin}"
            };
         }
         
         string sql = $"SELECT * FROM MultiSearchView WHERE {crit}";
         //string sql = $"EXEC SearchPlayers '{crit}'";
         using (var cmd = new SqlCommand(sql, con1)) {

            using (SqlDataReader rdr = cmd.ExecuteReader()) {
               while (rdr.Read()) {
                  var player = new CMlbPlayer();
                  player.PlayerId = rdr["PlayerId"].ToString();
                  player.PlayerName = rdr["PlayerName"].ToString();
                  player.PlayerType = rdr["PlayerType"].ToString()[0];
                  player.FieldingString = GetFieldingString(rdr);
                  player.Year = (int)rdr["Year"];
                  player.MlbTeam = rdr["MlbTeam"].ToString();
                  player.MlbLeague = rdr["MlbLeague"].ToString(); 

                  list.Add(player);
               }

            }
         }
         return list;

         string GetFieldingString(SqlDataReader rdr) {
         // ------------------------------------------------------------
            const int fMin = 8;
            string s = "";
            string del = "";
            if ((int)rdr["G_p"] >= fMin) { s += del + "p"; del = ","; }
            if ((int)rdr["G_c"] >= fMin) { s += del + "c"; del = ","; }
            if ((int)rdr["G_1b"] >= fMin) { s += del + "1b"; del = ","; }
            if ((int)rdr["G_2b"] >= fMin) { s += del + "2b"; del = ","; }
            if ((int)rdr["G_3b"] >= fMin) { s += del + "3b"; del = ","; }
            if ((int)rdr["G_ss"] >= fMin) { s += del + "ss"; del = ","; }
            if ((int)rdr["G_lf"] >= fMin) { s += del + "lf"; del = ","; }
            if ((int)rdr["G_cf"] >= fMin) { s += del + "cf"; del = ","; }
            if ((int)rdr["G_rf"] >= fMin) { s += del + "rf"; del = ","; }

            return s;
         }

      }




   }


   private string GetFieldingString() {
      // ------------------------------------------------------------
            public string FieldingString {
         // ----------------------------------------------------
         get {
            string s = "";
            string del = "";
            if (G_p >= fMin) { s += del + "p"; del = ","; }
            if (G_c >= fMin) { s += del + "c"; del = ","; }
            if (G_1b >= fMin) { s += del + "1b"; del = ","; }
            if (G_2b >= fMin) { s += del + "2b"; del = ","; }
            if (G_3b >= fMin) { s += del + "3b"; del = ","; }
            if (G_ss >= fMin) { s += del + "ss"; del = ","; }
            if (G_lf >= fMin) { s += del + "lf"; del = ","; }
            if (G_cf >= fMin) { s += del + "cf"; del = ","; }
            if (G_rf >= fMin) { s += del + "rf"; del = ","; }

            return s;
         }

      }



   }

   public void SetLineup(string user, string team, Models.UserPlayerListVM model) {
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

