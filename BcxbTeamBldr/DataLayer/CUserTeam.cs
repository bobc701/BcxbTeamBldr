using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace BcxbTeamBldr.DataLayer {

   public class CUserTeam {

      public string TeamName { get; set; }
      public string UserName { get; set; }
      public int NumPos { get; set; }
      public int NumPit { get; set; }
      public bool UsesDh { get; set; }
      public bool IsComplete { get; set; }
      public string StatusMsg { get; set; } = "Status msg goes here";


   }
}