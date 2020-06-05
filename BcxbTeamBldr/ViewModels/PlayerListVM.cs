using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using BcxbTeamBldr.DataLayer;

namespace BcxbTeamBldr.ViewModels {

   public class PlayerListVM {

      public CUserTeam UserTeam { get; set; }
      public List<CMlbPlayer> Players { get; set; }

      //public PlayerListVM (string user, string team) {
      //// ---------------------------------------------
      //   UserTeam = new CUserTeam { UserName = user, TeamName = team };
      //   Players = DbInfoEF.GetPlayerList(user, team);
      //}

      public PlayerListVM(string user, string team, int option = 0) {
      // ---------------------------------------------
         UserTeam = new CUserTeam { UserName = user, TeamName = team };
         Players = option switch {
            0 => null,
            _ => DbInfoEF.GetPlayerList(user, team)
         };
      }

   }
}