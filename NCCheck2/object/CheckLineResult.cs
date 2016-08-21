using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NCCheck2
{
   class CheckLineResult
   {
      public int LinePos { get; set; }
      public String Line { get; set; }
      public bool IsSectionHeader { get; set; }
   }
}
