using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using System.Data;
using System.Data.SqlClient;
using System.Configuration;
using System.Diagnostics;

using BcxbTeamBldr.Models;
using BcxbTeamBldr.DataLayer;

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


      public List<CUserTeamSpecs> GetUserTeamList(string user) {
         // --------------------------------------------------------------------
         // Purpose: Retrieve from database, a list of all teams for this user.

         var list = new List<CUserTeamSpecs>();
         //string sql = $"SELECT * FROM UserTeams WHERE UserName = '{user}'";
         string sql = $"EXEC GetUserTeamList '{user}'";
         using (var cmd = new SqlCommand(sql, con1)) {

            using (SqlDataReader rdr = cmd.ExecuteReader()) {
               while (rdr.Read()) {
                  var team = new CUserTeamSpecs();
                  team.UserName = user;
                  team.TeamName = rdr["TeamName"].ToString();
                  team.UserTeamID = (int)rdr["UserTeamID"];
                  team.NumPit = (int)rdr["TotPit"];
                  team.NumPos = (int)rdr["TotPos"];
                  team.UsesDh = (bool)rdr["UsesDh"];
                  team.IsComplete = (bool)rdr["IsComplete"];
                  team.StatusMsg = rdr["StatusMsg"].ToString();
                  list.Add(team);
               }

            }
            int n = cmd.ExecuteNonQuery();
         }
         return list;
      }


      public void AddNewTeam(string user, string team, bool dh) {
      // ---------------------------------------------------------------------
      // Let's try using parameterized query here, for security...
         string sql2 = "INSERT INTO UserTeams (UserName, TeamName, UsesDh) VALUES (@user, @team, @dh)";
         using (var cmd = new SqlCommand(sql2, con1)) { 
            cmd.Parameters.AddWithValue("@user", user);
            cmd.Parameters.AddWithValue("@team", team);
            cmd.Parameters.AddWithValue("@dh", dh ? 1 : 0);

            cmd.ExecuteNonQuery();
         }


         //string sql = $"INSERT INTO UserTeams (UserName, TeamName, UsesDh) VALUES ({user}, {team}, {(dh ? 1 : 0)})";
         //using (var cmd = new SqlCommand(sql, con1)) {
         //   cmd.ExecuteNonQuery();
         //}

      }

      public void DeleteTeam(string user, int teamID) {
      // ------------------------------------------
         string sql;

         sql = "DELETE FROM UserTeamRosters WHERE UserName = @user AND UserTeamID = @teamID";
         using (var cmd = new SqlCommand(sql, con1)) {
            cmd.Parameters.AddWithValue("@user", user);
            cmd.Parameters.AddWithValue("@teamID", teamID);
            cmd.ExecuteNonQuery();
         }

         sql = "DELETE FROM UserTeams WHERE UserName = @user AND UserTeamID = @teamID";
         using (var cmd = new SqlCommand(sql, con1)) {
            cmd.Parameters.AddWithValue("@user", user);
            cmd.Parameters.AddWithValue("@teamID", teamID);
            cmd.ExecuteNonQuery();
         }

      }


      public void UpdUserTeamSpecs(string userName, int teamID, string teamName, bool usesDh) {
         // ------------------------------------------------------------------------------
         int nUsesDh = usesDh ? 1 : 0;
         string sql = "UPDATE UserTeams SET TeamName = @teamName, UsesDh = @usesDh  WHERE UserName = @userName AND UserTeamID = @teamID";
         using (var cmd = new SqlCommand(sql, con1)) {
            cmd.Parameters.AddWithValue("@userName", userName);
            cmd.Parameters.AddWithValue("@teamID", teamID);
            cmd.Parameters.AddWithValue("@teamName", teamName);
            cmd.Parameters.AddWithValue("@usesDh", nUsesDh);

            cmd.ExecuteNonQuery();
         }

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



      public void AddPlayerToTeam(string user, int teamID, string pid, string teamTag, int yr) {
         // --------------------------------------------------------------------
         //string sql = $"EXEC AddPlayerToTeam '{user}', '{team}', '{id}'";
         string sql = 
            @$"INSERT INTO UserTeamRosters(UserName, UserTeamID, playerID, teamID, yearID) 
               VALUES(@user, @team, @pid, @teamTag, @year)";

         using (var cmd = new SqlCommand(sql, con1)) {

            cmd.Parameters.AddWithValue("@user", user);
            cmd.Parameters.AddWithValue("@team", teamID);
            cmd.Parameters.AddWithValue("@pid", pid);
            cmd.Parameters.AddWithValue("@teamTag", teamTag);
            cmd.Parameters.AddWithValue("@year", yr);

            cmd.ExecuteNonQuery();
         }
      }


      public void UpdateLineups(string user, int teamID, List<CUserPlayer> roster) {
      // ----------------------------------------------------------------------------
      // This uses ADO i/o SP due to transaction.

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
                  cmd1.Parameters.AddWithValue("@teamID", teamID);
                  cmd1.Parameters.AddWithValue("@pid", player.pid);
                  cmd1.Parameters.AddWithValue("@teamTag", player.teamTag);
                  cmd1.Parameters.AddWithValue("@year", player.year);
                  cmd1.ExecuteNonQuery();
               }
               else {
                  // Player not flagged, so update lineups (NoDh & Dh)...
                  cmd2.Parameters.Clear();
                  cmd2.Parameters.AddWithValue("@user", user);
                  cmd2.Parameters.AddWithValue("@teamID", teamID);
                  cmd2.Parameters.AddWithValue("@pid", player.pid);
                  cmd2.Parameters.AddWithValue("@teamTag", player.teamTag);
                  cmd2.Parameters.AddWithValue("@year", player.year);

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
      

      public void RemovePlayerFromTeam(string user, int teamID, (string pid, string teamTag, int yr) key) { 
         // --------------------------------------------------------------------
         string sql = $"EXEC RemovePlayerFromTeam '{user}', '{teamID}', '{key.pid}', '{key.teamTag}', {key.yr}";

         //This is used in several places so use the SP.
         //string sql = 
         //   @$"DELETE FROM TeamRosters 
         //      WHERE UserName = '{user}' AND TeamName = '{team}' 
         //      AND yearID = {key.yr} AND teamID = '{key.teamTag}' AND PlayerId = '{key.pid}'";

         using (var cmd = new SqlCommand(sql, con1)) {
            cmd.ExecuteNonQuery();
         }
         //GetUserTeamList(user);
      }


      public void RemoveAllPlayersFromTeam(string user, string team) {
         // --------------------------------------------------------------------
         //string sql = $"EXEC RemoveAllPlayersFromTeam '{user}', '{team}'";
         string sql = $"DELETE FROM UserTeamRosters WHERE UserName = '{user}' AND TeamName = '{team}'";
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


      //public List<CMlbPlayer> GetPlayerList(string user, string team) {
      //   // ---------------------------------------------------------------------
      //   // Since this returns (obsolete) MlbPlayer, it seems it s/not be used anymore. //11'20
      //   var list = new List<CMlbPlayer>();
      //   string sql = $"EXEC GetPlayerList '{user}', '{team}'";
      //   using (var cmd = new SqlCommand(sql, con1)) {

      //      using (SqlDataReader rdr = cmd.ExecuteReader()) {
      //         while (rdr.Read()) {
      //            var player = new CMlbPlayer();
      //            player.PlayerId = rdr["PlayerId"].ToString();
      //            player.PlayerName = rdr["PlayerName"].ToString();
      //            player.PlayerType = rdr["PlayerType"].ToString()[0];
      //            player.FieldingString = rdr["FieldingString"].ToString();
      //            player.Year = (int)rdr["Year"];
      //            player.MlbTeam = rdr["MlbTeam"].ToString();
      //            player.MlbLeague = rdr["MlbLeague"].ToString();

      //            list.Add(player);
      //         }

      //      }
      //   }
      //   return list;

      //}


      public CUserTeam GetUserTeam(string user, int teamID) {
      // ---------------------------------------------------------------------
         var tm = new CUserTeam(); ;

      // First, get the team-level information...
         string sql = "SELECT * FROM UserTeams WHERE UserName = @user AND UserTeamID = @teamID";
         using (var cmd = new SqlCommand(sql, con1)) {
            cmd.Parameters.AddWithValue("@user", user);
            cmd.Parameters.AddWithValue("@teamID", teamID);

            using (SqlDataReader rdr = cmd.ExecuteReader()) {
               if (rdr.Read()) {
                  tm.TeamSpecs = new CUserTeamSpecs();
                  tm.TeamSpecs.UserTeamID = teamID;
                  tm.TeamSpecs.TeamName = rdr["TeamName"].ToString();
                  tm.TeamSpecs.UserName = user;
                  tm.TeamSpecs.UsesDh = (bool)rdr["UsesDh"];
                  tm.TeamSpecs.IsComplete = false;
                  tm.TeamSpecs.StatusMsg = "";
               }
            }
         }

      // Then get to the roster, as a list of players...
         sql = $"EXEC GetUserTeamRoster @user, @teamID";
         using (var cmd = new SqlCommand(sql, con1)) {
            cmd.Parameters.AddWithValue("@user", user);
            cmd.Parameters.AddWithValue("@teamID", teamID);

            using (SqlDataReader rdr = cmd.ExecuteReader()) {
               tm.Roster = new List<CUserPlayer>();
               while (rdr.Read()) {
                  var player = new CUserPlayer();

                  player.UserName = user;
                  player.UserTeamID = teamID;
                  player.pid = rdr["PlayerID"].ToString();
                  player.teamTag = rdr["teamID"].ToString();
                  player.year = (int)rdr["yearID"];
                  player.PlayerName = rdr["nameLast"].ToString();
                  player.PlayerType = (int)rdr["G_p"] >= 5 ? 'P' : 'B';
                  player.FieldingString = GetFieldingString(rdr);
                  player.MlbLeague = rdr["lgID"].ToString();

                  player.Slot_NoDH = (int)rdr["Slot_NoDH"];
                  player.Posn_NoDH = (int)rdr["Posn_NoDH"];
                  player.Slot_DH = (int)rdr["Slot_DH"];
                  player.Posn_DH = (int)rdr["Posn_DH"];

                  tm.Roster.Add(player);
               }
            }
         }
         return tm;

      }


            private string GetFieldingString(SqlDataReader rdr) {
               // -----------------------------------------------------------------------

               const int fMin = 8; // Min games at posn to be listed in posn string
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


            //private string GetFieldingString0
            //   (object p, object c, object b1, object b2, object b3, object ss,
            //    object lf, object cf, object rf) {
            //// -----------------------------------------------------------------------

            //   const int fMin = 8; // Min games at posn to be listed in posn string
            //   string s = "";
            //   string del = "";

            //   if ((int)p >= fMin) { s += del + "p"; del = ","; }
            //   if ((int)c >= fMin) { s += del + "c"; del = ","; }
            //   if ((int)b1 >= fMin) { s += del + "1b"; del = ","; }
            //   if ((int)b2 >= fMin) { s += del + "2b"; del = ","; }
            //   if ((int)b3 >= fMin) { s += del + "3b"; del = ","; }
            //   if ((int)ss >= fMin) { s += del + "ss"; del = ","; }
            //   if ((int)lf >= fMin) { s += del + "lf"; del = ","; }
            //   if ((int)cf >= fMin) { s += del + "cf"; del = ","; }
            //   if ((int)rf >= fMin) { s += del + "rf"; del = ","; }

            //   return s;

            //}

            //public List<CMlbPlayer> SearchPlayers(string crit) {
            //   // ---------------------------------------------------------------------
            //   var list = new List<CMlbPlayer>();
            //   string sql = $"EXEC SearchPlayers '{crit}'";
            //   using (var cmd = new SqlCommand(sql, con1)) {

            //      using (SqlDataReader rdr = cmd.ExecuteReader()) {
            //         while (rdr.Read()) {
            //            var player = new CMlbPlayer();
            //            player.PlayerId = rdr["PlayerId"].ToString();
            //            player.PlayerName = rdr["PlayerName"].ToString();
            //            player.PlayerType = rdr["PlayerType"].ToString()[0];
            //            player.FieldingString = rdr["FieldingString"].ToString();
            //            player.Year = (int)rdr["Year"];
            //            player.MlbTeam = rdr["MlbTeam"].ToString();
            //            player.MlbLeague = rdr["MlbLeague"].ToString(); ;

            //            list.Add(player);
            //         }

            //      }
            //   }
            //   return list;

            //}


            public List<SelectListItem> GetTeamsForYear(int yr) {
               // -------------------------------------------------
               // This gets a List of dropdown items for the Team dropdown on the search page.
               // Note: I was using anonymous types passed back as objects.
               // But I changed my mind, and am using List<SelectListItem>.

               string sql =
                  @$"SELECT teamID, LineName, NickName
               FROM ZTeams
               WHERE yearID = {yr}";

               var ret = new List<SelectListItem>();
               using (var cmd = new SqlCommand(sql, con1))
               using (SqlDataReader rdr = cmd.ExecuteReader())

                  while (rdr.Read()) {
                     ret.Add(new SelectListItem {
                        Value = rdr["teamID"].ToString(),
                        Text = $"{rdr["LineName"]} {rdr["NickName"]}" }); // Eg: "NYY Yankees"
                  }
               return ret;


            }


            public List<MultiSearchView> SearchPlayersMulti(string critName, string critTeam, string critYear, string critPosn) {
               // ---------------------------------------------------------------------
               const int posnMin = 5;

               var list = new List<MultiSearchView>();

               // Build search string for SQL select...
               string crit = "", delim = "";
               if (critName != "All") { crit = $"nameLast LIKE '%{critName}%'"; delim = " AND "; }
               if (critTeam != "All") { crit += delim + $"teamID LIKE '%{critTeam}%'"; delim = " AND "; } // EG: 'NYA2019' LIKE '%NYA%'
               if (critYear != "All") { crit += delim + $"yearID = '{critYear}'"; delim = " AND "; }

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
                        var msv = new MultiSearchView();
                        msv.nameLast = rdr["nameLast"].ToString();
                        msv.ZPlayerId = (int)rdr["ZPlayerID"];
                        msv.playerID = rdr["playerID"].ToString();
                        msv.yearID = (int)rdr["yearID"];
                        msv.teamID = rdr["teamID"].ToString();
                        msv.teamName = rdr["teamName"].ToString();
                        msv.G_p = (int)rdr["G_p"];
                        msv.G_c = (int)rdr["G_c"];
                        msv.G_1b = (int)rdr["G_p"];
                        msv.G_2b = (int)rdr["G_2b"];
                        msv.G_3b = (int)rdr["G_3b"];
                        msv.G_ss = (int)rdr["G_ss"];
                        msv.G_lf = (int)rdr["G_lf"];
                        msv.G_cf = (int)rdr["G_cf"];
                        msv.G_rf = (int)rdr["G_rf"];
                        msv.G_of = (int)rdr["G_of"];
                        msv.G_all = (int)rdr["G_all"];
                        msv.lgID = rdr["lgID"].ToString();

                        list.Add(msv);
                     }

                  }
               }
               return list;

            }


            //public void SetLineup(string user, string team, Models.CUserTeamDetail model) {
            //   // ---------------------------------------------------------------------------------
            //   // First, delete all existing lineup data...
            //   string sql = $"EXEC ClearLineup '{user}', '{team}'";
            //   using (var cmd = new SqlCommand(sql, con1)) {
            //      cmd.ExecuteNonQuery();
            //   }

            //   // Now, update with new lineup data...
            //   // NOTE: This should probably be a transaction!!!
            //   foreach (CUserPlayer player in model.Roster
            //      .Where(p => p.Slot_NoDH != 0 || p.Posn_NoDH != 0 || p.Slot_DH != 0 || p.Posn_DH != 0)) {
            //      sql = $"EXEC SetLineup '{user}', '{team}', '{player.playerID}'";
            //      using var cmd = new SqlCommand(sql, con1);
            //      cmd.ExecuteNonQuery();
            //   }
            //}

         }

}



