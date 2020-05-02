using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using BcxbTeamBldr.DataLayer;

namespace BcxbTeamBldr.Models {

   public class TeamListVM {

      public string UserName;
      public List<CUserTeam> UserTeamList;

      public TeamListVM(string userName, DbInfo info) {
      // --------------------------------------------------------
      // Constructor...
         this.UserName = userName;
         this.UserTeamList = info.GetUserTeamList(userName);
      }



   }
}