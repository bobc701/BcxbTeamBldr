using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace BcxbTeamBldr.DbObjects {

   public class UserPlayer {

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

   }
}