using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace BcxbTeamBldr.DataLayer {

   public class CUserPlayer {

      public string UserName { get; set; } //Source table: UserPlayer
      public string TeamName { get; set; } //Source table: UserPlayer
      public string playerId { get; set; } //Source table: UserPlayer & MlbPlayer
      public string ZPlayerID { get; set; }

      public string PlayerName { get; set; }  //Source table: MlbPlayer
      public string PlayerType { get; set; }    //Source table: MlbPlayer Note: Linq to Entities can't deal w/ char.
      public string FieldingString { get; set; }  //Source table: MlbPlayer
      public int yearID { get; set; }               //Source table: MlbPlayer
      public string teamID { get; set; }
      public string MlbTeam { get; set; }         //Source table: MlbPlayer
      public string MlbLeague { get; set; }       //Source table: MlbPlayer

      public int Slot_NoDH { get; set; }  //Source table: UserPlayer
      public int Slot_DH { get; set; }    //Source table: UserPlayer
      public int Posn_NoDH { get; set; }  //Source table: UserPlayer
      public int Posn_DH { get; set; }    //Source table: UserPlayer

      public bool Selected { get; set; } = false;  // For use with SearchMulti view
      public bool RemoveFromTeam { get; set; } = false;   // For use with EditTeam

      //public string PosnString {
      //   // --------------------------------
      //   get {
      //      string[] posNames = { "p", "c", "1b", "2b", "3b", "ss", "lf", "cf", "rf" };
      //      var t = FieldingString.Zip(posNames, (skill, posName) => skill != '-' ? posName : "-").Where(p => p != "-");
      //      return String.Join(",", t);

      //   }
      //}
   }
}


/*
      public string UserName { get; set; }
      public string TeamName { get; set; }

      public int ZPlayerID { get; set; }
      public int yearID { get; set; }
      public string teamID { get; set; }
      public string playerID { get; set; }

      public int Slot_NoDH { get; set; }
      public int Slot_DH { get; set; }
      public int Posn_NoDH { get; set; }
      public int Posn_DH { get; set; }

 */
