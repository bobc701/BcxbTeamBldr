using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using BcxbTeamBldr.DataLayer;

namespace BcxbTeamBldr.ViewModels {

   public class UserTeamVM {

      public CUserTeam UserTeam { get; set; }
      public List<CUserPlayer> Players { get; set; }

   // These are used for display values in the view...
      public string[] PosnToStr { get; } = { "-", "p", "c", "1b", "2b", "3b", "ss", "lf", "cf", "rf", "dh" };
      public string[] SlotToStr { get; } = { "-", "1", "2", "3", "4", "5", "6", "7", "8", "9" };
      

      public UserTeamVM(string user, string team) {
      // ---------------------------------------------
         UserTeam = new CUserTeam { UserName = user, TeamName = team };
         Players = DbInfoEF.GetUserPlayerList(user, team).OrderBy(p => p.PlayerType).ThenBy(p => p.PlayerName).ToList();
      }


      //public UserPlayerListVM(string user, string team, DbInfo info, int option) {
      //// ---------------------------------------------
      //   UserTeam = new CUserTeam { UserName = user, TeamName = team };
      //   Players = option switch
      //   {
      //      0 => null,
      //      _ => info.GetUserPlayerList(user, team).OrderBy(p => p.PlayerType).ThenBy(p => p.PlayerName).ToList()
      //   };
      //}


      public UserTeamVM() {
         // ---------------------------------------------
         // I think you need a parameterless constructor
         // to use this as arg to [HttpPost] action.
         // ---------------------------------------------
         var n = 10;
      }


      public bool Ready {

         /* ------------------------------------------------------------
          * Rules:
          * Must have 10 to 25 total players
          * Must have 1 to 11 'P' player
          * Slot_NoDH must have 1..9
          * Posn_NoDH must have 1..9, same as Slot_DH
          * Slot_DH must have 1..9
          * Posn_DH must have 1..10, 2 to 9 must match Slot_DH, 1 must not bat, 10 must not field
          * -----------------------------------------------------------*/

         get {
            int totB = Players.Count(p => p.PlayerType == "B");
            int totP = Players.Count(p => p.PlayerType == "P");
            int totAll = totB + totP;
            string msg = "";

            if (totB + totP == 0) msg = "Team has no players";
            if (totB + totP > 25) msg = "Team must have 25 or fewer oalyers");
            if (totP > 11) msg = "Team must have 11 or fewer pitchers";

            if (msg != "") return msg;

            int[] slot_NoDH = { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 };
            int[] posn_NoDH = { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 };
            int[] slot_DH =   { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 };
            int[] posn_DH =   { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 };

            foreach (var p in Players) {
               if (p.Slot_NoDH != 0 && p.Posn_NoDH == 0) { msg = $"{p.PlayerName} has slot but no position"; break; }
               if (p.Slot_NoDH == 0 && p.Posn_NoDH != 0) { msg = $"{p.PlayerName} has position but no slot"; break; }
               if(p.Slot_DH == )

               slot[p.Slot_NoDH]++;
               posn[p.Posn_NoDH]++;
               slot[p.Slot_DH]++;
               posn[p.Posn_DH]++;


            }

            for (int i = 1; i++; i<=9) if ((slot_NoDH[i] == 0)

            return true;

         }
      }

   }

}