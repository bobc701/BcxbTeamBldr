using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Text;

namespace BcxbTeamBldr.DataLayer {

   public class CMlbPlayer {

      public int PlayerId{ get; set; }
      public string PlayerName { get; set; }
      public string MlbTeam { get; set; }
      public string MlbLeague { get; set; }
      public int Year { get; set; }
      public string FieldingString { get; set; }
      public char PlayerType { get; set; }
      public string Ver { get; set; }
      public string Stats { get; set; }

      public string PosnString {
      // --------------------------------
      // Decided not to include skills here, but left in the logic.
         get {
            //string[] posName = { "", "p", "c", "1b", "2b", "3b", "ss", "lf", "cf", "rf" };
            //var res = new StringBuilder();
            //string pos;
            //for (int i=0; i<=8; i++) {
            //   if (FieldingString[i] != '-') {
            //      pos = posName[i+1];
            //      //char skill = FieldingString[i];
            //      if (res.Length != 0) res.Append(',');
            //      //res.Append(pos + ":" + skill);
            //      res.Append(pos);
            //   }
            //}
            //return res.ToString();


            /* ----------------------------------------------------------
             * Here's how to do it (mostly) with Linq
             * It uses .Zip to match the 2 arrays elt-by-elt.
             * Note that FieldingString, though a string, is automatically
             * treated as a char array.
             * ----------------------------------------------------------*/

            string[] posName = { "p", "c", "1b", "2b", "3b", "ss", "lf", "cf", "rf" };
            var t = FieldingString.Zip(posName, (s, n) => s != '-' ? n : "-").Where(e => e != "-");
            return String.Join(",", t);


         }       
      }
   }
}
