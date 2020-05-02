using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using BcxbTeamBldr.DataLayer;

namespace BcxbTeamBldr.Models {

   public class PlayerListVM {

      public CUserTeam UserTeam { get; set; }
      public List<CMlbPlayer> Players { get; set; }

      public PlayerListVM (string user, string team, DbInfo info) {
      // ---------------------------------------------
         UserTeam = new CUserTeam { UserName = user, TeamName = team };
         Players = info.GetPlayerList(user, team);
      }

      public PlayerListVM(string user, string team, DbInfo info, int option) {
      // ---------------------------------------------
         UserTeam = new CUserTeam { UserName = user, TeamName = team };
         Players = option switch {
            0 => null,
            _ => info.GetPlayerList(user, team)
         };
      }

   }
}