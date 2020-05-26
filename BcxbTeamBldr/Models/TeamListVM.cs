using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using BcxbTeamBldr.DataLayer;

using DBAccess3;

namespace BcxbTeamBldr.Models {

   public class TeamListVM {

      public string UserName;
      public List<UserTeam> UserTeamList;

      public TeamListVM(string userName) {
      // --------------------------------------------------------
      // Constructor...
         this.UserName = userName;
         this.UserTeamList = DbInfoEF.GetUserTeamList(userName);
      }



   }
}