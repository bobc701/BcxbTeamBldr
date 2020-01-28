using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using BcxbTeamBldr.DataLayer;
using BcxbTeamBldr.Models;


namespace BcxbTeamBldr.Controllers {

   public class HomeController : Controller {
      // GET: Home
      public ActionResult Index() {
         // ---------------------------------------
         return View();
      }


      public ActionResult TeamList(string user) {
         // ----------------------------------------
         var view = new TeamListVM(user);

         return View(view);

      }

      public ActionResult AddTeam(string user) {
      // ------------------------------------------
         ViewBag.UserName = user;
         return View(new DataLayer.CUserTeam() { UserName = user });

      }

      [HttpPost]
      public ActionResult AddTeam(CUserTeam team) {
      // ------------------------------------------
         try {
            ViewBag.Msg = "";
            if (team.TeamName == "" || team.TeamName == null) {
               ViewBag.Msg = "You must enter a team name";
            }
            else if (team.TeamName.Length > 30) {
               ViewBag.Msg = "Team name is too long (30 characters max)";
            }
            else {
               bool exists = DbInfo.TeamNameExists(team.UserName, team.TeamName);
               if (exists) ViewBag.Msg = "You already have a team named " + team.TeamName;
            }
            if (ViewBag.Msg != "") {
               return View(team);
            }
            else {
               DbInfo.AddNewTeam(team.UserName, team.TeamName, team.UsesDh);
               var view = new TeamListVM(team.UserName);
               return View("TeamList", view);
            }

         }
         catch (Exception ex) {
            string msg =
               "An error occurred adding the new team to the database:\r\n" +
               ex.Message;
            ViewBag.ErrorMsg = msg;
            return View("ErrorView");
         }

      }


      public ActionResult AddPlayerToTeam(string user, string team, string pid) {
         // ------------------------------------------------------------
         try {
            ViewBag.Msg = "";
            var already = DbInfo.GetPlayerList(user, team).Exists(p => p.PlayerId == pid);
            if (already) { //Player already on team
               ViewBag.Msg = "Player is already on " + team;
               return View("SearchPlayers", new CUserTeam() { UserName = user, TeamName = team });
            }
            else {
               DbInfo.AddPlayerToTeam(user, team, pid);
               var roster = new PlayerListVM(user, team);
               return View("EditTeam", roster);
            }
         }
         catch (Exception ex) {
            string msg =
               "An error occurred adding the new team to the database:\r\n" +
               ex.Message;
            ViewBag.ErrorMsg = msg;
            return View("ErrorView");
         }

      }


      public ActionResult AddPlayerToTeamMulti(string user, string team, string idList) {
         // ------------------------------------------------------------
         try {
            ViewBag.Msg = "";
            var aIdList = idList.Split(',');
            foreach (var pid in aIdList) {
               var already = DbInfo.GetPlayerList(user, team).Exists(p => p.PlayerId == pid);
               if (!already) { //Player already on team
                  DbInfo.AddPlayerToTeam(user, team, pid);
               }
            }
            var roster = new PlayerListVM(user, team);
            return View("EditTeam", roster);
         }
         catch (Exception ex) {
            string msg =
               "An error occurred adding players to the team:\r\n" +
               ex.Message;
            ViewBag.ErrorMsg = msg;
            return View("ErrorView");
         }

      }



      public ActionResult RemovePlayerFromTeam(string user, string team, string id) {
         // ------------------------------------------------------------
         try {
            ViewBag.Msg = "";
            DbInfo.RemovePlayerFromTeam(user, team, id);
            var roster = new PlayerListVM(user, team);
            return View("EditTeam", roster);
         }
         catch (Exception ex) {
            string msg =
               "An error occurred adding the new team to the database:\r\n" +
               ex.Message;
            ViewBag.ErrorMsg = msg;
            return View("ErrorView");
         }

      }


      public ActionResult EditTeam(string user, string team) {
      // -----------------------------------------------------
         var roster = new UserPlayerListVM(user, team);

         Session["Roster"] = roster;
         return View(roster);
      }

      [HttpPost]
      public ActionResult EditTeam(UserPlayerListVM model) {
         // -----------------------------------------------------
         // This must be changed to operate on the static UserPlayerListVM object
         var team = model.UserTeam;
      // Logic here to return to TeamList.
         return View(new TeamListVM(team.TeamName));
      }


      public ActionResult EditLineup(string user, string team) {
         // -----------------------------------------------------
         var roster = new UserPlayerListVM(user, team);
         return View(roster);
      }

      [HttpPost]
      public ActionResult EditLineup(UserPlayerListVM model) {
      // -----------------------------------------------------
      // User has edited the lineups. 
      // So we must update the DB with the 4 lineup fields...
         var team = model.UserTeam;
         DbInfo.SetLineup(team.UserName, team.TeamName, model);

      // And then return the EditTeam view...
         return View("EditTeam", new UserPlayerListVM(model.UserTeam.UserName, model.UserTeam.TeamName));
      }


      public ActionResult SearchPlayers(string user, string team) {
      // -----------------------------------------------------
         var model = new CUserTeam() { UserName = user, TeamName = team };
         return View(model);

      }


      public ActionResult SearchMulti(string user, string team) {
         //  ------------------------------------------
         var model = new PlayerListVM(user, team, 0);
         return View(model);
      }


      public JsonResult detailasjson(string crit) {
      // ----------------------------------------------
         List<CMlbPlayer> py = DbInfo.SearchPlayers(crit);
         var s = Json(py, JsonRequestBehavior.AllowGet);
         return Json(py, JsonRequestBehavior.AllowGet);

      }

      public JsonResult searchMultiJson(string critName, string critTeam, string critYear, string critPosn) {
      // -------------------------------------------------------------------------------------
         List<CMlbPlayer> py = DbInfo.SearchPlayersMulti(critName, critTeam, critYear, critPosn);
         var s = Json(py, JsonRequestBehavior.AllowGet);
         return Json(py, JsonRequestBehavior.AllowGet);

      }



      public ContentResult VerMsgAction(string user, string team, string pid) {
      // --------------------------------------------------------------
         bool already = DbInfo.GetPlayerList(user, team).Exists(p => p.PlayerId == pid);
         if (already) { //Player already on team
            return Content("Player is already on " + team);
         }
         else {
            return Content("ok");
         }

      }


      public ContentResult AddNewUser(string user, string pwd) {
         // --------------------------------------------------------------
         try {
            int retval = DbInfo.AddNewUser(user, pwd);
            switch (retval) {
               case 0: return Content("ok");
               case 1: return Content($"'{user}' is already in use");
               default: return Content("Error updating the database");
            };
         }
         catch (Exception ex) {
            string msg = $"An error occurred adding the new user:\r\n{ex.Message}";
            return Content(msg);
         }

      }

      public ContentResult LogIn(string user, string pwd) {
         // --------------------------------------------------------------
         try {
            int retval = DbInfo.Login(user, pwd);
            switch (retval) {
               case 0: return Content("ok");
               case 1: return Content("User name or password is not valid");
               default: return Content("Error verifying credentials");
            };
         }
         catch (Exception ex) {
            string msg = $"An error occurred validating the credentials:\r\n{ex.Message}";
            return Content(msg);
         }

      }


   }

}
