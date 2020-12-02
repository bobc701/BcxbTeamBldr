using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using BcxbTeamBldr.DataLayer;
using BcxbTeamBldr.Models;


namespace BcxbTeamBldr.Controllers {

   public class HomeController : Controller {

      DbInfo info; 

      public HomeController() {
         // -----------------------------------
         //if (TempData["dbinfo"] == null) {
         //   TempData["dbinfo"] = new DbInfo();
         //   //TempData.Keep("dbinfo");
         //}
         //dbinfo = TempData.Peek["dbinfo"] as DbInfo;
         //TempData.Keep("dbinfo");

         info = new DbInfo();
      }


      // GET: Home
      public ActionResult Index() {
         // ---------------------------------------
         ViewBag.Title = "Login";
         return View();
      }


      public ActionResult TeamList(string user) {
         // ----------------------------------------
         ViewBag.Title = "Team List";
         var view = new TeamListVM(user, info);

         return View(view);

      }

      [Route("home/mvc/addteam/{user}")]
      public ActionResult AddTeam(string user) {
         // ------------------------------------------
         ViewBag.Title = "Add Team";
         ViewBag.UserName = user;
         return View(new DataLayer.CUserTeamSpecs() { UserName = user });

      }

      [Route("home/mvc/addteam/{user}")]
      [HttpPost]
      public ActionResult AddTeam(CUserTeamSpecs team) {
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
               bool exists = info.TeamNameExists(team.UserName, team.TeamName);
               if (exists) ViewBag.Msg = "You already have a team named " + team.TeamName;
            }
            if (ViewBag.Msg != "") {
               return View(team);
            }
            else {
               info.AddNewTeam(team.UserName, team.TeamName, team.UsesDh);
               var view = new TeamListVM(team.UserName, info);
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


      public ActionResult DeleteTeam(string user, int teamID) {
      // -------------------------------------------------------
         try {
            ViewBag.Msg = "";
            info.DeleteTeam(user, teamID);
            
            //var view = new TeamListVM(user, info);
            return RedirectToAction("TeamList", new { user = user });

         }
         catch (Exception ex) {
            string msg =
               "An error occurred attempting to delete the team from the database:\r\n" +
               ex.Message;
            ViewBag.ErrorMsg = msg;
            return View("ErrorView");
            //var view = new TeamListVM(user, info);
            //return RedirectToAction("TeamList", new { user = user });

         }

      }


      //public ActionResult AddPlayerToTeam(string user, string team, string pid) {
      //   // ------------------------------------------------------------
      //   // This appears to be NOT used 11'20
      //   try {
      //      ViewBag.Msg = "";
      //      var already = dbinfo.GetUserPlayerList(user, team).Exists(p => p.playerID == pid);
      //      if (already) { //Player already on team
      //         ViewBag.Msg = "Player is already on " + team;
      //         return View("SearchPlayers", new CUserTeam() { UserName = user, TeamName = team });
      //      }
      //      else {
      //         dbinfo.AddPlayerToTeam(user, team, pid);
      //         var roster = new CUserTeamDetail(user, team, dbinfo);
      //         return View("EditTeam", roster);
      //      }
      //   }
      //   catch (Exception ex) {
      //      string msg =
      //         "An error occurred adding the new team to the database:\r\n" +
      //         ex.Message;
      //      ViewBag.ErrorMsg = msg;
      //      return View("ErrorView");
      //   }

      //}


      public ActionResult AddPlayerToTeamMulti(string user, int teamID, string pidList, string tagList, string yearList) {
      // ------------------------------------------------------------
      // Sample idList: "2019|NYY|judgear01,2020|DET|jonesbi01,1901|BRO|stengca01"
         try{ 
            ViewBag.Msg = "";
            var aPidList = pidList.Split(',');
            var aTagList = tagList.Split(',');
            var aYearList = yearList.Split(',').Select(y => int.Parse(y)).ToArray();
            for (int i=0; i<aPidList.Length; i++) {
               var already = info.GetUserTeam(user, teamID).Roster
                  .Exists(p => p.pid == aPidList[i] && aTagList[i] == aTagList[i] && p.year == aYearList[i]);
               if (!already) { //Player already on team
                  info.AddPlayerToTeam(user, teamID, aPidList[i], aTagList[i], aYearList[i]);
               }
            }
            //var roster = new PlayerListVM(user, team);
            var roster = info.GetUserTeam(user, teamID); //new CUserTeam(user, teamID, info);
            return View("EditTeam", roster);
         }
         catch (Exception ex) {
            string msg =
               "An error occurred adding players to the team:\r\n" +
               ex.Message;
            ViewBag.Title = "Error";
            ViewBag.ErrorMsg = msg;
            return View("ErrorView");
         }

      }


      [Route("home/mvc/removeplayerfromteam/{user}/{team}/{yearID}/{teamID}/{playerID}")]
      public ActionResult RemovePlayerFromTeam(string user, int teamID, int yearID, string teamTag, string playerID) {
         // ------------------------------------------------------------
         try {
            ViewBag.Msg = ""; 
            info.RemovePlayerFromTeam(user, teamID, (playerID, teamTag, yearID));
            //info.ValidateTeam(user, team);
            var roster = info.GetUserTeam(user, teamID); //new CUserTeam(user, teamID, info);
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

      //[Route("home/mvc/editteam/{user}/{team}")]
      public ActionResult EditTeam(string user, int teamID) {
         // -----------------------------------------------------
         ViewBag.Title = "Edit Team";
         var roster = info.GetUserTeam(user, teamID); //new CUserTeam(user, teamID, info);

         return View(roster);
      }

      //[Route("home/mvc/editteam/{user}/{team}")]
      [HttpPost]
      public ActionResult EditTeam(CUserTeam model) {
      // -----------------------------------------------------
         try {
            int teamID = model.TeamSpecs.UserTeamID;
            string team = model.TeamSpecs.TeamName;
            string user = model.TeamSpecs.UserName;

         // Loop over player list, 
         // If they are marked'Remove', shoot off a delete.
         // Otherwise, shoot off an update for each with lineups. 
            info.UpdateLineups(user, teamID, model.Roster);

            return View("EditTeam", info.GetUserTeam(user, teamID)); //new CUserTeam(user, teamID, info));

         }
         catch (Exception ex) {
            ViewBag.ErrorMsg =
               $"There was an error updating the team in the database:\r\n{ex.Message}";
            return View("ErrorView");
         }

      }


      //[HttpPost]
      //public ActionResult EditLineup(CUserTeamDetail model) {
      //// -----------------------------------------------------
      //// User has edited the lineups. 
      //// So we must update the DB with the 4 lineup fields...
      //   var team = model.UserTeam;
      //   DbInfo.SetLineup(team.UserName, team.TeamName, model);

      //// And then return the EditTeam view...
      //   return View("EditTeam", new CUserTeamDetail(model.UserTeam.UserName, model.UserTeam.TeamName));
      //}




      public ActionResult EditLineup(string user, int teamID) {
         // -----------------------------------------------------
         var roster = info.GetUserTeam(user, teamID); //new CUserTeam(user, teamID, info);
         return View(roster);
      }

      //[HttpPost]
      //public ActionResult EditLineup(CUserTeamDetail model) {
      //// -----------------------------------------------------
      //// User has edited the lineups. 
      //// So we must update the DB with the 4 lineup fields...
      //   var team = model.UserTeam;
      //   DbInfo.SetLineup(team.UserName, team.TeamName, model);

      //// And then return the EditTeam view...
      //   return View("EditTeam", new CUserTeamDetail(model.UserTeam.UserName, model.UserTeam.TeamName));
      //}


      public ActionResult SearchPlayers(string user, string team) {
      // -----------------------------------------------------
         var model = new CUserTeamSpecs() { UserName = user, TeamName = team };
         return View(model);

      }

      [Route("home/mvc/searchmulti/{user}/{teamID}")]
      public ActionResult SearchMulti(string user, string teamID) {
         //  ------------------------------------------
         //var model = new PlayerListVM(user, team, dbinfo, 0);
         ViewBag.Title = "Search Players";
         ViewData["user"] = user;
         ViewData["teamID"] = teamID;
         return View();
      }


      //public JsonResult detailasjson(string crit) {
      //// ----------------------------------------------
      //   List<CMlbPlayer> py = dbinfo.SearchPlayers(crit);
      //   var s = Json(py, JsonRequestBehavior.AllowGet);
      //   return Json(py, JsonRequestBehavior.AllowGet);

      //}


      public JsonResult GetTeamsForYear(int yr) {
         // --------------------------------------------------------------------------------------
         //var ret = new[] { 
         //   new { teamTag = "NYY", teamName = "NYY Yankees" },
         //   new { teamTag = "BOS", teamName = "BOS Red Sox" }
         //};
         //var ret = new List<object>();
         //ret.Add(new { teamTag = "NYY", teamName = "NYY Yankees" });
         //ret.Add(new { teamTag = "BOS", teamName = "BOS Red Sox" });
         //ret.Add(new { teamTag = "PHI", teamName = "PHI Phillies" });

         try {
            var ret = info.GetTeamsForYear(yr);

            string s = Json(ret, JsonRequestBehavior.AllowGet).ToString();
            return Json(ret, JsonRequestBehavior.AllowGet);

         }
         catch (Exception ex) {
            string msg =
               $"An error occurred retrieving available teams for {yr}:\r\n" +
               ex.Message;
            return Json(msg, JsonRequestBehavior.AllowGet);
         }


      }

      public JsonResult searchMultiJson(string critName, string critTeam, string critYear, string critPosn) {
      // -------------------------------------------------------------------------------------
         List<MultiSearchView> py = info.SearchPlayersMulti(critName, critTeam, critYear, critPosn);
         var s = Json(py, JsonRequestBehavior.AllowGet);
         return Json(py, JsonRequestBehavior.AllowGet);

      }



      public ContentResult VerMsgAction(string user, int teamID ,string pid, string teamTag, int year) {
      // --------------------------------------------------------------
         bool already = info.GetUserTeam(user, teamID).Roster.Exists(p => p.pid == pid && p.teamTag == teamTag && p.year == year);
         if (already) { //Player already on team
            return Content("Player is already on " + teamID);
         }
         else {
            return Content("ok");
         }

      }


      public ContentResult AddNewUser(string user, string pwd) {
         // --------------------------------------------------------------
         try {
            int retval = info.AddNewUser(user, pwd);
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
            int retval = info.Login(user, pwd);
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
