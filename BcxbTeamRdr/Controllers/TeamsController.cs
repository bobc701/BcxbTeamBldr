using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

using BcxbData.DataLayer;


namespace BcxbTeamRdr.Controllers {

   public class TeamsController : ApiController {


      public IEnumerable<CUserTeam> GetTeamList(string userName) {
         // -----------------------------------------------
         List<CUserTeam> list = DbInfo.GetUserTeamList(userName);
         return list;
      }

      public IEnumerable<string> GetAll() {
         // -----------------------------------------------
         return new List<string>() { "hello", "goodbye" };
      }


      //public IHttpActionResult GetTeam(string userName, string teamName) {
      //   // ------------------------------------------
      //   var product = products.FirstOrDefault((p) => p.Id == id);
      //   if (product == null) {
      //      return NotFound();
      //   }
      //   return Ok(product);
      //}


   }
}
