using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using BcxbTeamBldr.DataLayer;

namespace BcxbTeamBldr.Models {

   public class PlayerListVM {

      public CUserTeam UserTeam { get; set; }
      public List<CMlbPlayer> Players { get; set; }

      public PlayerListVM (string user, string team) {
         // ---------------------------------------------
         UserTeam = new CUserTeam { UserName = user, TeamName = team };
         Players = DbInfo.GetPlayerList(user, team);
      }


   }
}