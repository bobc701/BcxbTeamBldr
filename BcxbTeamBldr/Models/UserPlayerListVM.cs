using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using BcxbTeamBldr.DataLayer;

namespace BcxbTeamBldr.Models {

   public class UserPlayerListVM {

      public CUserTeam UserTeam { get; set; }
      public List<CUserPlayer> Players { get; set; }

   // These are used for display values in the view...
      public string[] PosnToStr { get; } = { "-", "p", "c", "1b", "2b", "3b", "ss", "lf", "cf", "rf", "dh" };
      public string[] SlotToStr { get; } = { "-", "1", "2", "3", "4", "5", "6", "7", "8", "9" };
      

      public UserPlayerListVM(string user, string team, DbInfo info) {
      // ---------------------------------------------
         UserTeam = new CUserTeam { UserName = user, TeamName = team };
         Players = DbInfoEF.GetUserPlayerList(user, team).OrderBy(p => p.PlayerType).ThenBy(p => p.PlayerName).ToList();
      }


      public UserPlayerListVM(string user, string team, DbInfo info, int option) {
      // ---------------------------------------------
         UserTeam = new CUserTeam { UserName = user, TeamName = team };
         Players = option switch
         {
            0 => null,
            _ => info.GetUserPlayerList(user, team).OrderBy(p => p.PlayerType).ThenBy(p => p.PlayerName).ToList()
         };
      }


      public UserPlayerListVM() {
         // ---------------------------------------------
         // I think you need a parameterless constructor
         // to use this as arg to [HttpPost] action.
         // ---------------------------------------------
         var n = 10;
      }

   }
}