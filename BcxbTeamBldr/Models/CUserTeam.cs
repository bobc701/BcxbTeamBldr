using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using BcxbTeamBldr.DataLayer;

namespace BcxbTeamBldr.Models {

   public class CUserTeam {

      public CUserTeamSpecs TeamSpecs { get; set; }
      public List<CUserPlayer> Roster { get; set; }

   // These are used for display values in the view...
      public string[] PosnToStr { get; } = { "-", "p", "c", "1b", "2b", "3b", "ss", "lf", "cf", "rf", "dh" };
      public string[] SlotToStr { get; } = { "-", "1", "2", "3", "4", "5", "6", "7", "8", "9" };
      

      //public CUserTeam(string user, int teamID, DbInfo info) {
      //// ---------------------------------------------
      //   CUserTeam tm = info.GetUserTeam(user, teamID);
      //   TeamSpecs = tm.TeamSpecs;
      //   Roster = tm.Roster.OrderBy(p => p.PlayerType).ThenBy(p => p.PlayerName).ToList();
      //}


      //public CUserTeamDetail(string user, int teamID, DbInfo info, int option) {
      //// ---------------------------------------------
      //   UserTeam = new CUserTeam { UserName = user, UserTeamID = teamID };
      //   Roster = option switch
      //   {
      //      0 => null,
      //      _ => info.GetUserPlayerList(user, teamID).Roster.OrderBy(p => p.PlayerType).ThenBy(p => p.PlayerName).ToList()
      //   };
      //}


      public CUserTeam() {
         // ---------------------------------------------
         // I think you need a parameterless constructor
         // to use this as arg to [HttpPost] action.
         // ---------------------------------------------
         var n = 10;
      }

   }
}