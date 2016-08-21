using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace NCCheck2
{
   class NCService
   {
      private static String MARKER_FILE_END = "%";
      private static String REGEX_SECTION_DESCRIPTION = @"^\(\s.+\s\)$";
      private static String REGEX_SECTION_HEADER = @"^T\d+ M6$";
      private static String REGEX_SECTION_END = @"^M5$";
      private static String[] REGEX_TOKEN_LIST;
      private static String STATUS_NONE = "None";
      private static String STATUS_SECTION_BEGIN = "Section Begin";

      public static String SECTION_START = "^";
      public static String SECTION_END = "$";

      public List<Line> m_lines = new List<Line>();
      public List<WorkSection> m_workSections = new List<WorkSection>();
      private String m_status = STATUS_NONE;
      private WorkSection m_currentSection;

      public NCService()
      {
         REGEX_TOKEN_LIST = new String[]
         {
            @"^D\d+$",
            @"^H\d+$"
         };
      }

      public Line prepareLine(String line)
      {
         Line result = new Line();
         result.Text = line;

         // Analyze line
         if (STATUS_NONE.Equals(m_status))
         {
            Regex sectionBeginRegex = new Regex(REGEX_SECTION_HEADER);
            Match match = sectionBeginRegex.Match(line);
            if (match.Success)
            {
               String[] sectionNameParts = line.Split(' ');
               m_currentSection = new WorkSection();
               m_currentSection.ID = Convert.ToInt32(sectionNameParts[0].Substring(1));
               m_currentSection.StartLine = m_lines.Count;
               m_currentSection.Name = sectionNameParts[0];
               m_currentSection.Description = m_lines[m_currentSection.StartLine - 2].Text;

               m_status = STATUS_SECTION_BEGIN;
               result.IsSectionHeader = true;
            }
         }
         else if (STATUS_SECTION_BEGIN.Equals(m_status))
         {
            Regex sectionEndRegex = new Regex(REGEX_SECTION_END);
            Match match = sectionEndRegex.Match(line);
            if (match.Success || MARKER_FILE_END.Equals(line))
            {
               m_currentSection.EndLine = m_lines.Count - 1;

               m_workSections.Add(m_currentSection);

               m_currentSection = null;
               m_status = STATUS_NONE;
               result.IsSectionEnd = true;
            }
         }

         m_lines.Add(result);
         return result;
      }

      public void checkFile()
      {
         foreach(Line line in m_lines)
         {
            line.TokenList = new List<Token>();

            String[] lineParts = line.Text.Split(' ');
            foreach (String tokenText in lineParts)
            {
               Token token = new Token();
               token.Text = tokenText;
               token.errorCode = Const.ERROR_NONE;

               // check
               foreach (String regex in REGEX_TOKEN_LIST)
               {
                  Regex regexToken = new Regex(regex);
                  Match match = regexToken.Match(token.Text);
                  if (match.Success)
                  {
                     //String tokenId = 
                  }
               }
               
            }
         }
      }
   }
}
