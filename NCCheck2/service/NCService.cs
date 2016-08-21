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
      private static String STATUS_NONE = "None";
      private static String STATUS_SECTION_BEGIN = "Section Begin";

      public List<string> m_lines = new List<string>();
      public List<WorkSection> m_workSections = new List<WorkSection>();
      private String m_status = STATUS_NONE;
      private WorkSection m_currentSection;

      public CheckLineResult checkLine(String line)
      {
         CheckLineResult result = new CheckLineResult();
         result.Line = line;

         // Store line
         m_lines.Add(line);

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
               m_currentSection.Description = m_lines[m_currentSection.StartLine - 2];

               m_status = STATUS_SECTION_BEGIN;
               result.IsSectionHeader = true;
            }
         }
         else if (STATUS_SECTION_BEGIN.Equals(m_status))
         {
            Regex sectionDescriptionRegex = new Regex(REGEX_SECTION_DESCRIPTION);
            Match match = sectionDescriptionRegex.Match(line);
            if (match.Success || MARKER_FILE_END.Equals(line))
            {
               m_currentSection.EndLine = m_lines.Count - 1;

               m_workSections.Add(m_currentSection);

               m_currentSection = null;
               m_status = STATUS_NONE;
            }
         }
         
         return result;
      }
   }
}
