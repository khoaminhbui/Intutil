using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NCCheck2
{
   class Line
   {
      public int Position { get; set; }
      public String Text { get; set; }
      public bool IsSectionHeader { get; set; }
   }
}
