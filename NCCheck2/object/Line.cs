using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NCCheck2
{
   class Line
   {
      public Line()
      {
         TokenList = null;
         Section = null;
      }

      public int Position { get; set; }
      public String Text { get; set; }
      public bool IsSectionHeader { get; set; }
      public bool IsSectionFooter { get; set; }
      public bool IsLastLine { get; set; }

      public List<Token> TokenList { get; set; }
      public WorkSection Section { get; set; }
   }
}
