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

using DBAccess3;
using System.Data.Entity;
using System.Web.ModelBinding;



namespace BcxbTeamBldr.DataLayer {

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


      public static void AddNewTeam(string user, string team, bool dh) {
      // ---------------------------------------------------------------------

         var uTeam = new UserTeam { UserName = user, TeamName = team, UsesDh = dh };
         using (var ctx = new DBAccess3.DB_133455_bcxbteambldrEntities()) {
            ctx.UserTeams.Add(uTeam);
            ctx.SaveChanges();
         }

         // -------------------------------------------------------------------------- orig
         //string sql = $"EXEC AddNewTeam '{user}', '{team}', {(dh ? '1' : '0')}";
         //using (var cmd = new SqlCommand(sql, con1)) {
         //   cmd.ExecuteNonQuery();
         //}
         ////GetUserTeamList(user);

      }


      public static int AddNewUser(string user, string pwd) {
      // ----------------------------------------------------------

         using (var ctx = new DBAccess3.DB_133455_bcxbteambldrEntities()) {
            bool exists = ctx.Users.Count(u => u.UserName == user) > 0;
            if (exists)
               return 1;
            else {
               var newUser = new User { UserName = user, pwd = pwd };
               ctx.Users.Add(newUser);
               ctx.SaveChanges();
               return 0;
            }
         }

         // ------------------------------------------------------------------------------- orig
         //using (SqlCommand cmd = new SqlCommand("u196491_BcxbUser.AddNewUser", con1)) {
         //   cmd.CommandType = CommandType.StoredProcedure;

         //   // Add parameter for return value...
         //   SqlParameter returnValue = new SqlParameter();
         //   returnValue.Direction = ParameterDirection.ReturnValue;
         //   cmd.Parameters.Add(returnValue);

         //   // Add input parameters...
         //   cmd.Parameters.AddWithValue("@user", user);
         //   cmd.Parameters.AddWithValue("@pwd", pwd);

         //   cmd.ExecuteNonQuery();
         //   n = (int)returnValue.Value;
         //}
         // return n;

      }

      public static int Login(string user, string pwd) {
      // ----------------------------------------------------------
         bool exists;
         using (var ctx = new DBAccess3.DB_133455_bcxbteambldrEntities()) {
            exists = ctx.Users.Count(u => u.UserName == user && u.pwd == pwd) > 0;
         }
         return exists ? 0 : 1;


         // ------------------------------------------------------------- orig
         //using (SqlCommand cmd = new SqlCommand("Login", con1)) {
         //   cmd.CommandType = CommandType.StoredProcedure;

         //   // Add parameter for return value...
         //   SqlParameter loginResult = new SqlParameter();
         //   loginResult.Direction = ParameterDirection.ReturnValue;
         //   cmd.Parameters.Add(loginResult);

         //   // Add input parameters...
         //   cmd.Parameters.AddWithValue("@user", user);
         //   cmd.Parameters.AddWithValue("@pwd", pwd);

         //   cmd.ExecuteNonQuery();
         //   n = (int)loginResult.Value;
         //}
         //return n;

      }



      public static void AddPlayerToTeam(string user, string team, string id) {
      // --------------------------------------------------------------------
      // Task: Add one new record to UserPlayer
      // Note: We've already validated hat player is not already on team.

         using (var ctx = new DBAccess3.DB_133455_bcxbteambldrEntities()) {

            var userPlayer = new UserPlayer { PlayerId = id, UserName = user, TeamName = team };
            ctx.UserPlayers.Add(userPlayer);
            ctx.SaveChanges();
         }


         // ----------------------------------------------------------------- orig
         //string sql = $"EXEC AddPlayerToTeam '{user}', '{team}', '{id}'";
         //using (var cmd = new SqlCommand(sql, con1)) {
         //   cmd.ExecuteNonQuery();
         //}

      }


      public static void UpdateLineups(string user, string team, List<CUserPlayer> roster) {
         // ----------------------------------------------------------------------------
         // Task: Foreach in roster, if flagged for delete (in player.remove), then delete it from the 
         // UserPlayers. For the others, update their lineup fields (even if no change - we don't know!)
         // ----------------------------------------------------------------------------

         using var ctx = new DBAccess3.DB_133455_bcxbteambldrEntities();
         UserPlayer userPlayer;
         foreach (CUserPlayer player in roster) {

            if (player.RemoveFromTeam) {
               // Delete record from UserTeams... 
               userPlayer = ctx.UserPlayers.Find(user, team, player.PlayerId);
               ctx.UserPlayers.Remove(userPlayer);
            }
            else {
               // Update lineup data in UserPlayers...
               userPlayer = new UserPlayer {
                  PlayerId = player.PlayerId,
                  UserName = user,
                  TeamName = team,
                  Slot_NoDH = player.Slot_NoDH,
                  Slot_DH = player.Slot_DH,
                  Posn_NoDH = player.Posn_NoDH,
                  Posn_DH = player.Posn_DH
               };
               ctx.Entry(userPlayer).State = EntityState.Modified;
            }

         }
         ctx.SaveChanges();



         // -------------------------------------------- orig

         //SqlTransaction sqlTran = null;
         //try {

         //   sqlTran = con1.BeginTransaction();

         //   using SqlCommand cmd1 = new SqlCommand("RemovePlayerFromTeam", con1);
         //   cmd1.CommandType = CommandType.StoredProcedure;
         //   cmd1.Transaction = sqlTran;

         //   using SqlCommand cmd2 = new SqlCommand("UpdateLineups", con1);
         //   cmd2.CommandType = CommandType.StoredProcedure;
         //   cmd2.Transaction = sqlTran;

         //   foreach (UserPlayer player in roster) {
         //      if (true) { //if (player.Remove) {
         //         // Player is flagged for removal...
         //         cmd1.Parameters.Clear();
         //         cmd1.Parameters.AddWithValue("@user", user);
         //         cmd1.Parameters.AddWithValue("@team", team);
         //         cmd1.Parameters.AddWithValue("@pid", player.PlayerId);
         //         cmd1.ExecuteNonQuery();
         //      }
         //      else {
         //         // Player not flagged, so update lineups (NoDh & Dh)...
         //         cmd2.Parameters.Clear();
         //         cmd2.Parameters.AddWithValue("@user", user);
         //         cmd2.Parameters.AddWithValue("@team", team);
         //         cmd2.Parameters.AddWithValue("@pid", player.PlayerId);
         //         cmd2.Parameters.AddWithValue("@slotNoDh", player.Slot_NoDH);
         //         cmd2.Parameters.AddWithValue("@posnNoDh", player.Posn_NoDH);
         //         cmd2.Parameters.AddWithValue("@slotDh", player.Slot_DH);
         //         cmd2.Parameters.AddWithValue("@posnDh", player.Posn_DH);
         //         cmd2.ExecuteNonQuery();
         //      }
         //   }
         //   sqlTran.Commit();
         //   Debug.WriteLine("Both records were written to database.");
         //}
         //catch (Exception ex) {
         //   Debug.WriteLine(ex.Message);

         //   try {
         //      sqlTran.Rollback();
         //      throw ex;
         //   }
         //   catch (Exception exRollback) {
         //      // Throws an InvalidOperationException if the connection 
         //      // is closed or the transaction has already been rolled 
         //      // back on the server.
         //      Console.WriteLine(exRollback.Message);
         //      throw exRollback;

         //   }
         //}

      }


      public static void DeleteTeam(string userName, string teamName) {
      // ----------------------------------------------------------
      // Task: Delete all records for this user & team, 
      // from 1) UserTeams table, 2) UserPlayers table.
      // ----------------------------------------------------------
         using var ctx = new DB_133455_bcxbteambldrEntities();

         UserTeam userTeam = ctx.UserTeams.Find(userName, teamName);
         ctx.UserTeams.Remove(userTeam);

         List<UserPlayer> roster = ctx.UserPlayers.Where(p => p.UserName == userName && p.TeamName == teamName).ToList();
         foreach (var userPlayer in roster) {
            ctx.UserPlayers.Remove(userPlayer);
         }

         ctx.SaveChanges();

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


      public static bool TeamNameExists(string user, string team) {
         // ------------------------------------------------------------------
         using (var ctx = new DB_133455_bcxbteambldrEntities()) {
            UserTeam team1 = ctx.UserTeams.FirstOrDefault(t => t.UserName == user && t.TeamName == team);
            return (team1 != null);

         // ------------------------------------------------------------- orig
            //string sql = $"EXEC TeamNameExists '{user}', '{team}'";
            //using (var cmd = new SqlCommand(sql, con1)) {
            //return (int)cmd.ExecuteScalar() == 1 ? true : false;
         }

      }


      public static List<string> GetPlayerIdList(string user, string team) {
         // ---------------------------------------------------------------------
         // Task: Just return a list of Player ID's for players on team

         List<string> pids;
         using (var ctx = new DB_133455_bcxbteambldrEntities()) {
            pids = ctx.UserPlayers
               .Where(p => p.UserName == user && p.TeamName == team)
               .Select(p => p.PlayerId)
               .ToList();
         }
         return pids;
         
      }


      public static List<CMlbPlayer> GetPlayerList(string user, string team) {
         // ---------------------------------------------------------------------

         //  Orig sql...
         //  SELECT up.PlayerId, mp.PlayerName, mp.MlbTeam, mp.MlbLeague, mp.Year, mp.FieldingString, mp.PlayerType,
         //  mp.Stats, mp.LgStats, up.Slot_NoDH, up.Posn_NoDH, up.Slot_DH, up.Posn_DH
         //  FROM UserPlayers up
         //  JOIN MlbPlayers mp ON mp.PlayerId = up.PlayerId
         //  WHERE up.UserName = @user AND up.TeamName = @team

         using (var ctx = new DB_133455_bcxbteambldrEntities()) {

            List<CMlbPlayer> mps2 =
              (from mp in ctx.MlbPlayers
               join up in ctx.UserPlayers.Where(up => up.UserName == user && up.TeamName == team)
               on mp.PlayerId equals up.PlayerId
               select new CMlbPlayer {
                  PlayerId = mp.PlayerId,
                  PlayerName = mp.PlayerName,
                  PlayerType = mp.PlayerType[0],
                  FieldingString = mp.FieldingString,
                  Year = (int)mp.Year,
                  MlbTeam = mp.MlbTeam,
                  MlbLeague = mp.MlbLeague
               }
            ).ToList();

            //List<CMlbPlayer> mps = ctx.MlbPlayers
            //   .Join(
            //      ctx.UserPlayers.Where(up => up.UserName == user && up.TeamName == team),
            //      mp => mp.PlayerId,
            //      up => up.PlayerId,
            //      (mp, up) => MapMlbPlayer(mp))
            //   .ToList();

            return mps2;
         }

         // ------------------------------------------------------------- orig
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


      public static CMlbPlayer MapMlbPlayer(MlbPlayer p1) {
         // -----------------------------------------------------------------
         // This is an object mapping function.
         // Unfortuneatly such a function can't be used in Linq-to-Entities!

         var p2 = new CMlbPlayer {
            PlayerId = p1.PlayerId,
            PlayerName = p1.PlayerName,
            PlayerType = p1.PlayerType[0],
            FieldingString = p1.FieldingString,
            Year = (int)p1.Year,
            MlbTeam = p1.MlbTeam,
            MlbLeague = p1.MlbLeague
         };
         return p2;
      }

      public static List<CUserPlayer> GetUserPlayerList(string user, string team) {
      // ---------------------------------------------------------------------

         // This is the SQL that was used in SP version, for reference...
         //SELECT up.PlayerId, mp.PlayerName, mp.MlbTeam, mp.MlbLeague, mp.Year, mp.FieldingString, mp.PlayerType,
         // mp.Stats, mp.LgStats, up.Slot_NoDH, up.Posn_NoDH, up.Slot_DH, up.Posn_DH

         using (var ctx = new DB_133455_bcxbteambldrEntities()) {
            List<CUserPlayer> mps2 =
              (from mp in ctx.MlbPlayers
               join up in ctx.UserPlayers.Where(up => up.UserName == user && up.TeamName == team)
               on mp.PlayerId equals up.PlayerId
               select new CUserPlayer {
                  PlayerId = mp.PlayerId,
                  UserName = user,
                  TeamName = team,
                  PlayerName = mp.PlayerName,
                  PlayerType = mp.PlayerType,
                  FieldingString = mp.FieldingString,
                  Year = (int)mp.Year,
                  MlbTeam = mp.MlbTeam,
                  MlbLeague = mp.MlbLeague,
                  Slot_NoDH = up.Slot_NoDH,
                  Posn_NoDH = up.Posn_NoDH,
                  Slot_DH = up.Slot_DH,
                  Posn_DH = up.Posn_DH
               }).ToList();

            return mps2;
         }


         //var list = new List<CUserPlayer>();
         //string sql = $"EXEC GetPlayerList '{user}', '{team}'";
         //using (var cmd = new SqlCommand(sql, con1)) {

         //   using (SqlDataReader rdr = cmd.ExecuteReader()) {
         //      while (rdr.Read()) {
         //         var player = new UserPlayer();
         //         player.PlayerId = rdr["PlayerId"].ToString();
         //         //player.PlayerName = rdr["PlayerName"].ToString();
         //         //player.PlayerType = rdr["PlayerType"].ToString()[0];
         //         //player.FieldingString = rdr["FieldingString"].ToString();
         //         //player.Year = (int)rdr["Year"];
         //         //player.MlbTeam = rdr["MlbTeam"].ToString();
         //         //player.MlbLeague = rdr["MlbLeague"].ToString();

         //         player.Slot_NoDH = (int)rdr["Slot_NoDH"];
         //         player.Posn_NoDH = (int)rdr["Posn_NoDH"];
         //         player.Slot_DH = (int)rdr["Slot_DH"];
         //         player.Posn_DH = (int)rdr["Posn_DH"];

         //         list.Add(player);
         //      }

         //   }
         //}
         //return list;

      }


      public List<MlbPlayer> SearchPlayers(string crit) {
         // ---------------------------------------------------------------------
         // NOTE: I don't think this is ever used.

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


      public static List<CMlbPlayer> SearchPlayersMulti(string critName, string critTeam, string critYear, string critPosn) {
         // ---------------------------------------------------------------------

         var list2 = new List<CMlbPlayer>();
         var list1 = new List<MlbPlayer>();

         int ix = 0;
         var s = new string[] { "All", "p", "c", "1b", "2b", "3b", "ss", "lf", "cf", "rf", "of" };
         for (int i=0; i<=s.Length-1; i++) if (s[i] == critPosn) { ix = i; break; }
         int? year = critYear == "All" ? 0 : int.Parse(critYear);

      // Check for no selection, return empty list if so...
         if (ix == 0 && year == 0 && critName == "All" && critTeam == "All") return list2;

         using (var ctx = new DBAccess3.DB_133455_bcxbteambldrEntities()) {

            if (ix > 0 && ix < 10) // Filtereing on position 1..9...
               list1 =
                  ctx.MlbPlayers
                     .Where(p => critName == "All" || p.PlayerName.Contains(critName))
                     .Where(p => critTeam == "All" || p.MlbTeam.Contains(critTeam))
                     .Where(p => year == 0 || p.Year == year)
                     .Where(p => p.FieldingString.Substring(ix-1, 1) != "-") //Can't use index [] with LinqToEntities
                     .ToList();

            else if (ix == 10) //Filtering on 'of' = 7,8,9...
               list1 =
                  ctx.MlbPlayers
                     .Where(p => critName == "All" || p.PlayerName.Contains(critName))
                     .Where(p => critTeam == "All" || p.MlbTeam.Contains(critTeam))
                     .Where(p => year == 0 || p.Year == year)
                     .Where(p =>
                        p.FieldingString.Substring(6, 1) != "-" ||
                        p.FieldingString.Substring(7, 1) != "-" ||
                        p.FieldingString.Substring(8, 1) != "-")
                     .ToList();

            else // Not filtering on posn...
               list1 =
                  ctx.MlbPlayers
                     .Where(p => critName == "All" || p.PlayerName.Contains(critName))
                     .Where(p => critTeam == "All" || p.MlbTeam.Contains(critTeam))
                     .Where(p => year == 0 || p.Year == year)
                     .ToList();

            // Map result from MlbPlayer to CMlbPlayer...
            list2 =
               list1
                  .Select(p1 => new CMlbPlayer {
                     PlayerId = p1.PlayerId,
                     PlayerName = p1.PlayerName,
                     PlayerType = p1.PlayerType[0],
                     FieldingString = p1.FieldingString,
                     Year = (int)p1.Year,
                     MlbTeam = p1.MlbTeam,
                     MlbLeague = p1.MlbLeague
                  }).ToList();

         }

         return list2;


         //   // Build search string for SQL select...
         //   string crit = "", delim = "";
         //if (critName != "All") { crit = $"PlayerName LIKE '%{critName}%'"; delim = " AND "; }
         //if (critTeam != "All") { crit += delim + $"MlbTeam LIKE '%{critTeam}%'"; delim = " AND "; } // EG: 'NYA2019' LIKE '%NYA%'
         //if (critYear != "All") { crit += delim + $"Year = '{critYear}'"; delim = " AND "; }

         //switch (critPosn) {
         //   case "p": crit += delim + "SUBSTRING(FieldingString,1,1) != '-'"; break;
         //   case "c": crit += delim + "SUBSTRING(FieldingString,2,1) != '-'"; break;
         //   case "1b": crit += delim + "SUBSTRING(FieldingString,3,1) != '-'"; break;
         //   case "2b": crit += delim + "SUBSTRING(FieldingString,4,1) != '-'"; break;
         //   case "3b": crit += delim + "SUBSTRING(FieldingString,5,1) != '-'"; break;
         //   case "ss": crit += delim + "SUBSTRING(FieldingString,6,1) != '-'"; break;
         //   case "lf": crit += delim + "SUBSTRING(FieldingString,7,1) != '-'"; break;
         //   case "cf": crit += delim + "SUBSTRING(FieldingString,8,1) != '-'"; break;
         //   case "rf": crit += delim + "SUBSTRING(FieldingString,9,1) != '-'"; break;
         //   case "of":
         //      crit += delim +
         //         "(SUBSTRING(FieldingString,7,1) != '-' OR SUBSTRING(FieldingString,8,1) != '-' OR SUBSTRING(FieldingString,9,1) != '-')";
         //      break;
         //}

         //string sql = $"SELECT * FROM MlbPlayers WHERE {crit}";
         ////string sql = $"EXEC SearchPlayers '{crit}'";
         //using (var cmd = new SqlCommand(sql, con1)) {

         //   using (SqlDataReader rdr = cmd.ExecuteReader()) {
         //      while (rdr.Read()) {
         //         var player = new MlbPlayer();
         //         player.PlayerId = rdr["PlayerId"].ToString();
         //         player.PlayerName = rdr["PlayerName"].ToString();
         //         //player.PlayerType = rdr["PlayerType"].ToString()[0];
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

         static bool CritSingle(MlbPlayer player, string critName, string critTeam, string critYear, string critPosn) {
         // --------------------------------------------------------------------------------
            bool ok1 = true, ok2 = true, ok3 = true, ok4 = true;
            ok1 = critName == "All" || player.PlayerName.Contains(critName);
            if (ok1) ok2 = (critTeam == "All" || player.MlbTeam == critTeam);
            if (ok2) ok3 = (critYear == "All" || player.Year == int.Parse(critYear));
            if (ok3) ok4 = critPosn switch
            {
               "p" => player.FieldingString[0] != '-',
               "c" => player.FieldingString[1] != '-',
               "1b" => player.FieldingString[2] != '-',
               "2b" => player.FieldingString[3] != '-',
               "3b" => player.FieldingString[4] != '-',
               "ss" => player.FieldingString[5] != '-',
               "lf" => player.FieldingString[6] != '-',
               "cf" => player.FieldingString[7] != '-',
               "rf" => player.FieldingString[8] != '-',
               "of" =>
                  player.FieldingString[6] != '-' || player.FieldingString[7] != '-' || player.FieldingString[8] != '-'
            };
            return ok1 && ok2 && ok3 && ok4;
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
