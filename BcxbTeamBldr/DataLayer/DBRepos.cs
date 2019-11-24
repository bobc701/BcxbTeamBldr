using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace BcxbTeamBldr.DataLayer {

   public static class DBRepos {

      // Thisi s an experimental class, where data is fetched from code itself,
      // (ie, a psuedo-database) rather than a database.
      // Probably a waster of time.

      // This class, DBInfo, is responsible for all intereaction with the psuedo-database.

      private static List<CMlbLeague> dbo_MlbLeagues;
      private static List<CMlbPlayer> dbo_MlbPlayers;
      private static List<CUserPlayer> dbo_UserPlayers;
      private static List<CUser> dbo_Users;
      private static List<CUserTeam> dbo_UserTeams;


      static DBRepos() {
         // -------------------------------------------
         dbo_MlbLeagues = new List<CMlbLeague> {
            new CMlbLeague {
               MlbLeague = "AL2018", Year = 2018, Ver="1.2",
               Stats = "01685E14316508710AF186B544EEE1032721DD51443EB4EB1D40FDD610064"
            },
            new CMlbLeague {
               MlbLeague = "AL2019", Year = 2019, Ver="1.2",
               Stats = "00C9A00B44D2D85093E0CB7582DAD08C16311490A220925C10B0907F10037"
            },
            new CMlbLeague {
               MlbLeague = "NL2018", Year = 2018, Ver = "1.2",
               Stats = "016AD5143224FB30F991C9A7D52092342611F7125D3974BF1EA0FFCD00064"
            },
            new CMlbLeague {
               MlbLeague = "NL2019", Year = 2019, Ver = "1.2",
               Stats = "00C9030B3682D1709190D57132DEC12A13710FC12122D22D0D908CB500037"
            }
          };
      }
   

      public static List<CUserTeam> GetUserTeamList(string user) {
         // --------------------------------------------------------------------
         // Purpose: Retrieve from database, a list of all teams for this user.

         var list = new List<CUserTeam>();
         //string sql = $"SELECT * FROM UserTeams WHERE UserName = '{user}'";
         return list;
      }


      public static void AddNewTeam(string user, string team, bool dh) {
         // ---------------------------------------------------------------------

      }


      public static void AddPlayerToTeam(string user, string team, int id) {
         // --------------------------------------------------------------------
      }


      public static void RemovePlayerFromTeam(string user, string team, int id) {
         // --------------------------------------------------------------------
      }


      public static bool TeamNameExists(string user, string team) {
         // ------------------------------------------------------------------
         return false;
      }


      public static List<CMlbPlayer> GetPlayerList(string user, string team) {
         // ---------------------------------------------------------------------
         var list = new List<CMlbPlayer>();
         string sql = $"EXEC GetPlayerList '{user}', '{team}'";
         
         return list;

      }


      public static List<CMlbPlayer> SearchPlayers(string crit) {
         // ---------------------------------------------------------------------
         var list = new List<CMlbPlayer>();

            
         
         return list;

      }

   }


}

