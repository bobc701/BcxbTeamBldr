using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace BcxbTeamBldr.DbObjects {

   public class UserTeam {

      public string UserName { get; set; }
      public string TeamName { get; set; }
      public Nullable<int> NumPos { get; set; }
      public Nullable<int> NumPit { get; set; }
      public bool UsesDh { get; set; }
      public bool IsComplete { get; set; }

   }

}