using System;
using System.Collections.Generic;
using System.Data.Entity.Core.Common.CommandTrees;
using System.Diagnostics.Eventing.Reader;
using System.Linq;
using System.Runtime.InteropServices.ComTypes;
using System.Text;
using System.Threading.Tasks;
using System.Data;
using System.Data.SqlClient;
using System.Diagnostics;



namespace DBAccess2 {

   public class DbInfoEF {
      // This class, DBInfo, is responsible for all intereaction with the database.

      public SqlConnection con1 = null; // Just for compiling -- take it out.




      public static List<UserTeam> GetUserTeamList(string user) {
      // --------------------------------------------------------------------
      // Purpose: Retrieve from database, a list of all teams for this user.

         List<UserTeam> list;
         using (var ctx = new DB_133455_bcxbteambldrEntities()) {
            list = ctx.UserTeams.Where(t => t.UserName == user).ToList();
            list = (from t in ctx.UserTeams where t.UserName == user select t).ToList();
         }
         return list;


         //string sql = $"EXEC GetUserTeamList '{user}'";
         //using (var cmd = new SqlCommand(sql, con1)) {

         //   using (SqlDataReader rdr = cmd.ExecuteReader()) {
         //      while (rdr.Read()) {
         //         var team = new CUserTeam();
         //         team.UserName = user;
         //         team.TeamName = rdr["TeamName"].ToString();
         //         team.NumPit = (int)rdr["TotPit"];
         //         team.NumPos = (int)rdr["TotPos"];
         //         team.UsesDh = (bool)rdr["UsesDh"];
         //         list.Add(team);
         //      }

         //   }
         //   int n = cmd.ExecuteNonQuery();
         //}
         //return list;
      }


      public void AddNewTeam(string user, string team, bool dh) {
         // ---------------------------------------------------------------------
         string sql = $"EXEC AddNewTeam '{user}', '{team}', {(dh ? '1' : '0')}";
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
         string sql = $"EXEC AddPlayerToTeam '{user}', '{team}', '{id}'";
         using (var cmd = new SqlCommand(sql, con1)) {
            cmd.ExecuteNonQuery();
         }
      }


      public void UpdateLineups(string user, string team, List<UserPlayer> roster) {
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

            foreach (UserPlayer player in roster) {
               if (true) { //if (player.Remove) {
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
         using (var ctx = new DB_133455_bcxbteambldrEntities()) {
            UserPlayer userPlayer = ctx.UserPlayers.FirstOrDefault(
               p => p.UserName == user && p.TeamName == team && p.PlayerId == id);
            ctx.UserPlayers.Remove(userPlayer);
            ctx.SaveChanges();
         }

         //string sql = $"EXEC RemovePlayerFromTeam '{user}', '{team}', '{id}'";
         //using (var cmd = new SqlCommand(sql, con1)) {
         //   cmd.ExecuteNonQuery();
         //}
         ////GetUserTeamList(user);
      }


      public void RemoveAllPlayersFromTeam(string user, string team) {
         // --------------------------------------------------------------------
         // Looks like no references
         //string sql = $"EXEC RemoveAllPlayersFromTeam '{user}', '{team}'";
         //using (var cmd = new SqlCommand(sql, con1)) {
         //   cmd.ExecuteNonQuery();
         //}
         ////GetUserTeamList(user);
      }


      public bool TeamNameExists(string user, string team) {
      // ------------------------------------------------------------------
         using (var ctx = new DBAccess2.DB_133455_bcxbteambldrEntities()) {
            UserTeam team1 = ctx.UserTeams.FirstOrDefault(t => t.UserName == user && t.TeamName == team);
            return (team1 != null);

            //string sql = $"EXEC TeamNameExists '{user}', '{team}'";
            //using (var cmd = new SqlCommand(sql, con1)) {
            //return (int)cmd.ExecuteScalar() == 1 ? true : false;
         }

      }


      public List<MlbPlayer> GetPlayerList(string user, string team) {
         // ---------------------------------------------------------------------

         //  SELECT up.PlayerId, mp.PlayerName, mp.MlbTeam, mp.MlbLeague, mp.Year, mp.FieldingString, mp.PlayerType,
         //mp.Stats, mp.LgStats, up.Slot_NoDH, up.Posn_NoDH, up.Slot_DH, up.Posn_DH
         //  FROM UserPlayers up
         //  JOIN MlbPlayers mp ON mp.PlayerId = up.PlayerId
         //  WHERE up.UserName = @user AND up.TeamName = @team



         using (var ctx = new DB_133455_bcxbteambldrEntities()) {

            //List<string> pids = ctx.UserPlayers
            //   .Where(p => p.UserName == user && p.TeamName == team)
            //   .Select(p => p.PlayerId)
            //   .ToList();

            List<MlbPlayer> mps2 =
              (from mp in ctx.MlbPlayers
               join up in ctx.UserPlayers.Where(up => up.UserName == user && up.TeamName == team)
               on mp.PlayerId equals up.PlayerId
               select mp).ToList();

            mps2[0].PlayerName = "Smith";

            List<MlbPlayer> mps = ctx.MlbPlayers
               .Join(
                  ctx.UserPlayers.Where(up => up.UserName == user && up.TeamName == team),
                  mp => mp.PlayerId,
                  up => up.PlayerId,
                  (mp, up) => mp)
               .ToList();

            return mps;
         }


         //   string sql = $"EXEC GetPlayerList '{user}', '{team}'";
         //   using (var cmd = new SqlCommand(sql, con1)) {

         //   using (SqlDataReader rdr = cmd.ExecuteReader()) {
         //      while (rdr.Read()) {
         //         var player = new CMlbPlayer();
         //         player.PlayerId = rdr["PlayerId"].ToString();
         //         player.PlayerName = rdr["PlayerName"].ToString();
         //         player.PlayerType = rdr["PlayerType"].ToString()[0];
         //         player.FieldingString = rdr["FieldingString"].ToString();
         //         player.Year = (int)rdr["Year"];
         //         player.MlbTeam = rdr["MlbTeam"].ToString();
         //         player.MlbLeague = rdr["MlbLeague"].ToString();

         //         list.Add(player);
         //      }

         //   }
         //}
         //return list;

      }


      public List<UserPlayer> GetUserPlayerList(string user, string team) {
         // ---------------------------------------------------------------------
         var list = new List<UserPlayer>();
         string sql = $"EXEC GetPlayerList '{user}', '{team}'";
         using (var cmd = new SqlCommand(sql, con1)) {

            using (SqlDataReader rdr = cmd.ExecuteReader()) {
               while (rdr.Read()) {
                  var player = new UserPlayer();
                  player.PlayerId = rdr["PlayerId"].ToString();
                  //player.PlayerName = rdr["PlayerName"].ToString();
                  //player.PlayerType = rdr["PlayerType"].ToString()[0];
                  //player.FieldingString = rdr["FieldingString"].ToString();
                  //player.Year = (int)rdr["Year"];
                  //player.MlbTeam = rdr["MlbTeam"].ToString();
                  //player.MlbLeague = rdr["MlbLeague"].ToString();

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


      public List<MlbPlayer> SearchPlayers(string crit) {
         // ---------------------------------------------------------------------
         var list = new List<MlbPlayer>();
         string sql = $"EXEC SearchPlayers '{crit}'";
         using (var cmd = new SqlCommand(sql, con1)) {

            using (SqlDataReader rdr = cmd.ExecuteReader()) {
               while (rdr.Read()) {
                  var player = new MlbPlayer();
                  player.PlayerId = rdr["PlayerId"].ToString();
                  player.PlayerName = rdr["PlayerName"].ToString();
                  player.PlayerType = rdr["PlayerType"].ToString();
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


      public List<MlbPlayer> SearchPlayersMulti(string critName, string critTeam, string critYear, string critPosn) {
         // ---------------------------------------------------------------------
         var list = new List<MlbPlayer>();

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
                  var player = new MlbPlayer();
                  player.PlayerId = rdr["PlayerId"].ToString();
                  player.PlayerName = rdr["PlayerName"].ToString();
                  //player.PlayerType = rdr["PlayerType"].ToString()[0];
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


      //public void SetLineup(string user, string team, Models.UserPlayerListVM model) {
      //   // ---------------------------------------------------------------------------------
      //   // First, delete all existing lineup data...
      //   string sql = $"EXEC ClearLineup '{user}', '{team}'";
      //   using (var cmd = new SqlCommand(sql, con1)) {
      //      cmd.ExecuteNonQuery();
      //   }

      //   // Now, update with new lineup data...
      //   // NOTE: This should probably be a transaction!!!
      //   foreach (UserPlayer player in model.Players
      //      .Where(p => p.Slot_NoDH != 0 || p.Posn_NoDH != 0 || p.Slot_DH != 0 || p.Posn_DH != 0)) {
      //      sql = $"EXEC SetLineup '{user}', '{team}', '{player.PlayerId}'";
      //      using (SqlCommand cmd = new SqlCommand(sql, con1)) {
      //         cmd.ExecuteNonQuery();
      //      }  
      //   }
      //}

   }
}
