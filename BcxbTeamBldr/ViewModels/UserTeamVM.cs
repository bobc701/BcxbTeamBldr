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


      public string Ready {

         /* ------------------------------------------------------------
          * Rules:
          * Must have 10 to 15 'B' players
          * Must have 1 to 10 'P' player
          * Must have 25 or less total players
          * Slot_NoDH must have 1..9
          * Posn_NoDH must have 1..9, same as Slot_DH
          * Slot_DH must have 1..9
          * Posn_DH must have 1..10, 2 to 9 must match Slot_DH, 1 must not bat, 10 must not field
          * -----------------------------------------------------------
          * Return: "" if ok, else message
          * -----------------------------------------------------------*/
         get {

            int totB = Players.Count(p => p.PlayerType == "B");
            int totP = Players.Count(p => p.PlayerType == "P");
            int totAll = totB + totP;
            string msg = "";

            if (totB + totP < 11) return "Team must have at least 11 players (including 1 pitcher)";
            if (totB + totP > 25) return "Team can't have more than 25 players";
            if (totB < 10) return "Team must have at least 10 position players plus DH's";
            if (totP == 0) return "Team must have at least one pitcher";
            if (totP > 11) return "Team can't have more than 11 pitchers";

            int[] slots_NoDH = { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 };
            int[] posns_NoDH = { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 };
            int[] slots_DH =   { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 };
            int[] posns_DH =   { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0};

            string[] posnNames = { "", "p", "c", "1b", "2b", "3b", "ss", "lf", "cf", "rf", "dh" };

            foreach (var p in Players) {
               if (p.Slot_NoDH != 0 && p.Posn_NoDH == 0) return $"{p.PlayerName} has slot but no position"; 
               if (p.Slot_NoDH == 0 && p.Posn_NoDH != 0) return $"{p.PlayerName} has position but no slot";
               if (p.Posn_DH == 1 && p.Slot_DH > 0) return $"Pitcher, {p.PlayerName}, should not have position";
              
               slots_NoDH[p.Slot_NoDH]++;
               posns_NoDH[p.Posn_NoDH]++;
               slots_DH[p.Slot_DH]++;
               posns_DH[p.Posn_DH]++;
            }

            for (int i = 1; i < 9; i++) {
               if (slots_NoDH[i] == 0) return $"No batter in slot {i} in No-DH lineup";
               if (slots_DH[i] == 0) return $"No batter in slot {i} in DH lineup";
               if (slots_NoDH[i] > 1) return $"{slots_NoDH[i]} batters in slot {i} in No-DH lineup";
               if (slots_DH[i] > 1) return $"{slots_DH[i]} batters in slot {i} in No-DH lineup";
            }

            for (int i = 0; i <= 9; i++) {
               if (posns_NoDH[i] == 0) return $"No player at position: {posnNames[i]}, in No-DH lineup";
               if (posns_NoDH[i] > 1) return $"{posns_NoDH[i]} players at position {posnNames[i]}, in No-DH lineup";
            }
            for (int i = 0; i <= 10; i++) {
               if (posns_DH[i] == 0) return $"No player at position{posnNames[i]}, in DH lineup";
               if (posns_DH[i] > 1) return $"{posns_DH[i]} players at position {posnNames[i]}, in DH lineup";
            }
            return "";

         }
      }

   }

}