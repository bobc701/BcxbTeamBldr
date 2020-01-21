using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace BcxbTeamBldr.DataLayer {

   public class CUserPlayer {

      public string UserName { get; set; } //Source table: UserPlayer
      public string TeamName { get; set; } //Source table: UserPlayer
      public string PlayerId { get; set; } //Source table: UserPlayer & MlbPlayer

      public string PlayerName { get; set; }  //Source table: MlbPlayer
      public char PlayerType { get; set; }    //Source table: MlbPlayer
      public string FieldingString { get; set; }  //Source table: MlbPlayer
      public int Year { get; set; }               //Source table: MlbPlayer
      public string MlbTeam { get; set; }         //Source table: MlbPlayer
      public string MlbLeague { get; set; }       //Source table: MlbPlayer

      public int Slot_NoDH { get; set; }  //Source table: UserPlayer
      public int Slot_DH { get; set; }    //Source table: UserPlayer
      public int Posn_NoDH { get; set; }  //Source table: UserPlayer
      public int Posn_DH { get; set; }    //Source table: UserPlayer

      public bool Selected { get; set; } = false;  // For use with SearchMulti view

      public string PosnString {
         // --------------------------------
         get {
            string[] posNames = { "p", "c", "1b", "2b", "3b", "ss", "lf", "cf", "rf" };
            var t = FieldingString.Zip(posNames, (skill, posName) => skill != '-' ? posName : "-").Where(p => p != "-");
            return String.Join(",", t);

         }
      }
   }
}