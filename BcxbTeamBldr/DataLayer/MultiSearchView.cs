using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace BcxbTeamBldr.DataLayer {

   public class MultiSearchView {

      const int fMin = 10;  // Min games at a posn to be included in FieldingString.
      const int pitMin = 5; // Min games as P to be type 'P'

      public string nameLast { get; set; }
      public int ZPlayerId { get; set; }
      public string playerID { get; set; }
      public int yearID { get; set; }
      public string teamID { get; set; }
      public string lgID { get; set; }
      public Nullable<int> PAEst { get; set; }
      public Nullable<int> BFP { get; set; }
      public Nullable<int> G_c { get; set; }
      public Nullable<int> G_1b { get; set; }
      public Nullable<int> G_2b { get; set; }
      public Nullable<int> G_3b { get; set; }
      public Nullable<int> G_ss { get; set; }
      public Nullable<int> G_lf { get; set; }
      public Nullable<int> G_cf { get; set; }
      public Nullable<int> G_rf { get; set; }
      public Nullable<int> G_of { get; set; }
      public Nullable<int> G_p { get; set; }
      public int G_all { get; set; }
      public string teamName { get; set; }

      public char PlayerType => (G_p ?? 0) >= pitMin ? 'P' : 'B';

      public string FieldingString {
         // ----------------------------------------------------
         get {
            string s = "";
            string del = "";
            if (G_p >= fMin) { s += del + "p"; del = ","; }
            if (G_c >= fMin) { s += del + "c"; del = ","; }
            if (G_1b >= fMin) { s += del + "1b"; del = ","; }
            if (G_2b >= fMin) { s += del + "2b"; del = ","; }
            if (G_3b >= fMin) { s += del + "3b"; del = ","; }
            if (G_ss >= fMin) { s += del + "ss"; del = ","; }
            if (G_lf >= fMin) { s += del + "lf"; del = ","; }
            if (G_cf >= fMin) { s += del + "cf"; del = ","; }
            if (G_rf >= fMin) { s += del + "rf"; del = ","; }

            return s;
         }

      }
   }
}