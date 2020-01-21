using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using BcxbTeamBldr.DataLayer;

namespace BcxbTeamBldr.Models {

   public class UserPlayerListVM {

      public CUserTeam UserTeam { get; set; }
      public List<CUserPlayer> Players { get; set; }

      public UserPlayerListVM(string user, string team) {
      // ---------------------------------------------
         UserTeam = new CUserTeam { UserName = user, TeamName = team };
         Players = DbInfo.GetUserPlayerList(user, team);
      }

      public UserPlayerListVM(string user, string team, int option) {
      // ---------------------------------------------
         UserTeam = new CUserTeam { UserName = user, TeamName = team };
         Players = option switch
         {
            0 => null,
            _ => DbInfo.GetUserPlayerList(user, team)
         };
      }

      public UserPlayerListVM() {
         // ---------------------------------------------
         // I think you need a parameterless constructor
         // to use this as arg to [HttpPost/ action.
         // ---------------------------------------------
         var n = 10;
      }

   }
}