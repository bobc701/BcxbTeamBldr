using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;


namespace BcxbTeamBldr.DataLayer {

   public class CUserTeam {

      [Required(ErrorMessage = "You must specify team name.")]
      [StringLength(30, ErrorMessage = "Team name is too nong (max is 30).")]
      public string TeamName { get; set; }

      public string UserName { get; set; }
      public int NumPos { get; set; }
      public int NumPit { get; set; }
      public bool UsesDh { get; set; }

   }
}