using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using BcxbTeamBldr.DataLayer;
using BcxbTeamBldr.ViewModels;

using DBAccess3;


namespace BcxbTeamBldr.Controllers {

   public class HomeController : Controller {

      DbInfo dbinfo; 

      public HomeController() {
         // -----------------------------------
         //if (TempData["dbinfo"] == null) {
         //   TempData["dbinfo"] = new DbInfo();
         //   //TempData.Keep("dbinfo");
         //}
         //dbinfo = TempData.Peek["dbinfo"] as DbInfo;
         //TempData.Keep("dbinfo");

         //dbinfo = new DbInfo();
      }


      // GET: Home
      public ActionResult Index() {
         // ---------------------------------------
         ViewBag.Title = "Team Bldr";
         return View();
      }


      public ActionResult TeamList(string user) {
         // ----------------------------------------
         var view = new TeamListVM(user);

         return View(view);

      }

      public ActionResult AddTeam(string user) {
      // ------------------------------------------
         ViewBag.Msg = "";
         return View(new DataLayer.CUserTeam { UserName = user });

      }

      [HttpPost]
      public ActionResult AddTeam(CUserTeam team) {
         // ------------------------------------------

         //if (!ModelState.IsValid) return View(team);

         try {
            ViewBag.Msg = "";
            if (team.TeamName == "" || team.TeamName == null) {
               ViewBag.Msg = "You must enter a team name";
            }
            else if (team.TeamName.Length > 30) {
               ViewBag.Msg = "Team name is too long (30 characters max)";
            }
            else {
               bool exists = DbInfoEF.TeamNameExists(team.UserName, team.TeamName);
               if (exists) ViewBag.Msg = "You already have a team named " + team.TeamName;
            }
            if (ViewBag.Msg != "") {
               return View(team);
            }
            else {
               DbInfoEF.AddNewTeam(team.UserName, team.TeamName, team.UsesDh);
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
            var already = dbinfo.GetPlayerList(user, team).Exists(p => p.PlayerId == pid);
            if (already) { //Player already on team
               ViewBag.Msg = "Player is already on " + team;
               return View("SearchPlayers", new CUserTeam() { UserName = user, TeamName = team });
            }
            else {
               dbinfo.AddPlayerToTeam(user, team, pid);
               var roster = new PlayerListVM(user, team, 1);
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
            var pids = DbInfoEF.GetPlayerIdList(user, team);
            foreach (var pid in aIdList) {
               bool already = pids.Exists(id => id == pid);
               if (!already) { //Player already on team
                  DbInfoEF.AddPlayerToTeam(user, team, pid);
               }
            }
            var roster = new UserTeamVM(user, team);
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
            dbinfo.RemovePlayerFromTeam(user, team, id);
            var roster = new PlayerListVM(user, team, 1);
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
         var roster = new UserTeamVM(user, team);

         return View(roster);
      }


      [HttpPost]
      public ActionResult EditTeam(UserTeamVM model) {
      // -----------------------------------------------------
         try {
            string team = model.UserTeam.TeamName;
            string user = model.UserTeam.UserName;

         // Loop over player list, 
         // If they are marked'Remove', shoot off a delete.
         // Otherwise, shoot off an update for each with lineups. 
            DbInfoEF.UpdateLineups(user, team, model.Players);

            ModelState.Clear();

            return View("EditTeam", new UserTeamVM(user, team));

         }
         catch (Exception ex) {
            ViewBag.ErrorMsg =
               $"There was an error updating the team in the database:\r\n{ex.Message}";
            return View("ErrorView");
         }

      }


      public ActionResult DeleteTeam(string userName, string teamName) {
         // --------------------------------------------------------------
         try {
            DbInfoEF.DeleteTeam(userName, teamName);
            return RedirectToAction("TeamList", new { userName = userName, teamName = teamName });
         }
         catch (Exception ex) {
            ViewBag.ErrorMsg =
               $"There was an error deleting the team in the database:\r\n{ex.Message}";
            return View("ErrorView");
         }


      }



      //[HttpPost]
      //public ActionResult EditLineup(UserPlayerListVM model) {
      //// -----------------------------------------------------
      //// User has edited the lineups. 
      //// So we must update the DB with the 4 lineup fields...
      //   var team = model.UserTeam;
      //   DbInfo.SetLineup(team.UserName, team.TeamName, model);

      //// And then return the EditTeam view...
      //   return View("EditTeam", new UserPlayerListVM(model.UserTeam.UserName, model.UserTeam.TeamName));
      //}




      public ActionResult EditLineup(string user, string team) {
         // -----------------------------------------------------
         var roster = new UserTeamVM(user, team);
         return View(roster);
      }

      //[HttpPost]
      //public ActionResult EditLineup(UserPlayerListVM model) {
      //// -----------------------------------------------------
      //// User has edited the lineups. 
      //// So we must update the DB with the 4 lineup fields...
      //   var team = model.UserTeam;
      //   DbInfo.SetLineup(team.UserName, team.TeamName, model);

      //// And then return the EditTeam view...
      //   return View("EditTeam", new UserPlayerListVM(model.UserTeam.UserName, model.UserTeam.TeamName));
      //}


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
         List<CMlbPlayer> py = dbinfo.SearchPlayers(crit);
         var s = Json(py, JsonRequestBehavior.AllowGet);
         return Json(py, JsonRequestBehavior.AllowGet);

      }

      public JsonResult searchMultiJson(string critName, string critTeam, string critYear, string critPosn) {
      // -------------------------------------------------------------------------------------
         List<CMlbPlayer> py = DbInfoEF.SearchPlayersMulti(critName, critTeam, critYear, critPosn);
         var s = Json(py, JsonRequestBehavior.AllowGet);
         return Json(py, JsonRequestBehavior.AllowGet);

      }



      public ContentResult VerMsgAction(string user, string team, string pid) {
      // --------------------------------------------------------------
         bool already = dbinfo.GetPlayerList(user, team).Exists(p => p.PlayerId == pid);
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
            int retval = DbInfoEF.AddNewUser(user, pwd);
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
            int retval = DbInfoEF.Login(user, pwd);
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
