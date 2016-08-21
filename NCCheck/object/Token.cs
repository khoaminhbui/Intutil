using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NCCheck
{
   class Token
   {
      public String OriginalText { get; set; }
      public String FixedText { get; set; }
      public int ErrorCode { get; set; }
   }
}
