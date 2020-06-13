using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using BcxbTeamBldr.DataLayer;

namespace BcxbTeamBldr.ViewModels {

   public class UserTeamVM {

      public CUserTeam UserTeam { get; set; }
      public List<CUserPlayer> Players { get; set; }
      public bool ReadyStatus { get; set; }
      public string ReadyMsg { get; set; }

   // These are used for display values in the view...
      public string[] PosnToStr { get; } = { "-", "p", "c", "1b", "2b", "3b", "ss", "lf", "cf", "rf", "dh" };
      public string[] SlotToStr { get; } = { "-", "1", "2", "3", "4", "5", "6", "7", "8", "9" };
      

      public UserTeamVM(string user, string team) {
      // ---------------------------------------------
         UserTeam = new CUserTeam { UserName = user, TeamName = team };
         Players = DbInfoEF.GetUserPlayerList(user, team).OrderBy(p => p.PlayerType).ThenBy(p => p.PlayerName).ToList();
         (bool status, string msg) ready = GetReadyStatus();
         ReadyStatus = ready.status;
         ReadyMsg = ready.msg;
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


      public (bool status, string msg) GetReadyStatus() {

         /* ------------------------------------------------------------
          * Return: "" if ok, else message
          * -----------------------------------------------------------*/

            int totB = Players.Count(p => p.PlayerType == "B");
            int totP = Players.Count(p => p.PlayerType == "P");
            int totAll = totB + totP;
            string msg = "";

         // Check totals of players...
            if (totB + totP == 0) return (false, "No players yet!");
            if (totB + totP < 11) return (false, "Team must have at least 11 players (including 1 pitcher)");
            if (totB + totP > 25) return (false, "Team can't have more than 25 players");
            if (totB < 10) return (false, "Team must have at least 10 position players plus DH's");
            if (totP == 0) return (false, "Team must have at least one pitcher");
            if (totP > 11) return (false, "Team can't have more than 11 pitchers");

            int[] slots_NoDH = { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 };
            int[] posns_NoDH = { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 };
            int[] slots_DH = { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 };
            int[] posns_DH = { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 };

            string[] posnNames = { "", "p", "c", "1b", "2b", "3b", "ss", "lf", "cf", "rf", "dh" };

         // Cross-checks of slot vs posn..
            foreach (var p in Players) {
               if (p.Slot_NoDH != 0 && p.Posn_NoDH == 0) return (false, $"{p.PlayerName} has slot but no position in No-DH lineup");
               if (p.Slot_NoDH == 0 && p.Posn_NoDH != 0) return (false, $"{p.PlayerName} has position but no slot in No-DH lineup");
               if (p.Posn_DH >= 2 && p.Posn_DH <= 10 && p.Slot_DH == 0) return (false, $"{p.PlayerName} has position but no slot in DH lineup");
               if (p.Slot_DH != 0 && p.Posn_DH == 0) return (false, $"{p.PlayerName} has slot but no position in DH lineup");
               if (p.Posn_DH == 1 && p.Slot_DH > 0) return (false, $"Pitcher, {p.PlayerName}, should not have position in DH lneup");

               slots_NoDH[p.Slot_NoDH]++;
               posns_NoDH[p.Posn_NoDH]++;
               slots_DH[p.Slot_DH]++;
               posns_DH[p.Posn_DH]++;
            }

         // Check for 1 player in each slot...
            for (int i = 1; i <= 9; i++) {
               if (slots_NoDH[i] == 0) return (false, $"No batter in slot {i} in No-DH lineup");
               if (slots_DH[i] == 0) return (false, $"No batter in slot {i} in DH lineup");
               if (slots_NoDH[i] > 1) return (false, $"{slots_NoDH[i]} batters in slot {i} in No-DH lineup");
               if (slots_DH[i] > 1) return (false, $"{slots_DH[i]} batters in slot {i} in DH lineup");
            }

         // Check for 1 player at each posn...
            for (int i = 1; i <= 9; i++) {
               if (posns_NoDH[i] == 0) return (false, $"No player at position: {posnNames[i]}, in No-DH lineup");
               if (posns_NoDH[i] > 1) return (false, $"{posns_NoDH[i]} players at position {posnNames[i]}, in No-DH lineup");
            }
            for (int i = 1; i <= 10; i++) {
               if (posns_DH[i] == 0) return (false, $"No player at position {posnNames[i]}, in DH lineup");
               if (posns_DH[i] > 1) return (false, $"{posns_DH[i]} players at position {posnNames[i]}, in DH lineup");
            }
            return (true, "Ready!");

         }
      }

   }

