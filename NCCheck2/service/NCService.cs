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

      public List<WorkSection> m_workSections = new List<WorkSection>();
      private String m_status;
      private WorkSection m_currentSection;

      public List<Line> Lines { get; set; }

      public NCService()
      {
         REGEX_TOKEN_LIST = new String[]
         {
            @"^D\d+$",
            @"^H\d+$"
         };

         m_currentSection = null;
         m_status = STATUS_NONE;
         Lines = new List<Line>();
      }

      public Line prepareLine(String lineText)
      {
         Line line = new Line();
         line.Text = lineText;

         // Analyze line
         if (STATUS_NONE.Equals(m_status))
         {
            Regex sectionBeginRegex = new Regex(REGEX_SECTION_HEADER);
            Match match = sectionBeginRegex.Match(lineText);
            if (match.Success)
            {
               String[] sectionNameParts = lineText.Split(' ');
               m_currentSection = new WorkSection();
               m_currentSection.Number = Convert.ToInt32(sectionNameParts[0].Substring(1));
               m_currentSection.StartLine = Lines.Count;
               m_currentSection.Name = sectionNameParts[0];
               m_currentSection.Description = Lines[m_currentSection.StartLine - 2].Text;

               m_status = STATUS_SECTION_BEGIN;
               line.IsSectionHeader = true;
            }
         }
         else if (STATUS_SECTION_BEGIN.Equals(m_status))
         {
            Regex sectionEndRegex = new Regex(REGEX_SECTION_END);
            Match match = sectionEndRegex.Match(lineText);
            if (match.Success || MARKER_FILE_END.Equals(lineText))
            {
               m_currentSection.EndLine = Lines.Count - 1;

               m_workSections.Add(m_currentSection);

               m_currentSection = null;
               m_status = STATUS_NONE;
               line.IsSectionFooter = true;
            }
         }

         if (m_currentSection != null)
         {
            line.Section = m_currentSection;
         }

         Lines.Add(line);
         return line;
      }

      public void checkFile()
      {
         foreach(Line line in Lines)
         {
            // Bypass line that is belong to a work section or header and footer of a section.
            if (line.Section == null
               || line.IsSectionHeader || line.IsSectionFooter)
            {       continue;
  
                  }

            // Parse words of the line into tokens.
            line.TokenList = new List<Token>();

            String[] lineParts = line.Text.Split(' ');
            foreach (String tokenText in lineParts)
            {
               Token token = new Token();
               token.Text = tokenText;
               token.ErrorCode = Const.ErrorCode.ERROR_CODE_NONE;

               // check
               foreach (String regex in REGEX_TOKEN_LIST)
               {
                  Regex regexToken = new Regex(regex);
                  Match match = regexToken.Match(token.Text);
                  if (match.Success)
                  {
                     // Predefined tokens with Number that mismatch Section Number is considered errornous.
                     int tokenNumber = Convert.ToInt32(token.Text.Substring(1));
                     if (tokenNumber != line.Section.Number)
                     {
                        token.ErrorCode = Const.ErrorCode.ERROR_CODE_SECTION_ID_MISMATCH;
                     }
                     else
                     {
                        token.ErrorCode = Const.ErrorCode.ERROR_CODE_OK;
                     }

                     break;
                  }
               }

               line.TokenList.Add(token);
            }
         }
      }
   }
}
