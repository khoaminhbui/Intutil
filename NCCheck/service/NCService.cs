using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace NCCheck
{
   class NCService
   {
      private static String MARKER_FILE_END = "%";
      private static String REGEX_SECTION_HEADER = @"^T\d+\s*M6$";
      private static String REGEX_SECTION_END = @"^(M5|M9)$";
      private static String REGEX_SECTION_END_SECOND_1 = @"^G91\s*G28\s*Z0\.\s*M9$";
      private static String REGEX_SECTION_END_SECOND_2 = @"^G91\s*G28\s*Z0\.\s*M5$";
      private static String REGEX_SECTION_COMMENT = @"^\(.*\)$";
      private static String REGEX_TOKEN = @"[A-Z][\.|\d|-]+";
      private static String REGEX_SPACE = @"\s+";
      private static String[] REGEX_TOKEN_CHECK_LIST;
      private static String STATUS_NONE = "None";
      private static String STATUS_SECTION_BEGIN = "Section Begin";

      public static String SECTION_START = "^";
      public static String SECTION_END = "$";
      public static String SECTION_END_1 = "M5";
      public static String SECTION_END_2 = "M9";

      public List<WorkSection> m_workSections;
      private String m_status;
      private WorkSection m_currentSection;

      public List<Line> Lines { get; set; }
      public int ErrorCount { get; set; }

      public NCService()
      {
         REGEX_TOKEN_CHECK_LIST = new String[]
         {
            @"^D\d+$",
            @"^H\d+$"
         };

         m_workSections = new List<WorkSection>();
         m_currentSection = null;
         m_status = STATUS_NONE;
         Lines = new List<Line>();
         ErrorCount = 0;
      }

      public Line prepareLine(String lineText)
      {
         Line line = new Line();
         line.Text = lineText;

         // skip comment, not processing.
         Regex sectionCommentRegex = new Regex(REGEX_SECTION_COMMENT);
         Match matchComment = sectionCommentRegex.Match(lineText);
         if (matchComment.Success)
         {
            line.IsCommentLine = true;
            Lines.Add(line);
            return line;
         }

         // Analyze line
         if (STATUS_NONE.Equals(m_status))
         {
            Regex sectionBeginRegex = new Regex(REGEX_SECTION_HEADER);
            Match match = sectionBeginRegex.Match(lineText);
            if (match.Success)
            {
               List<Token> sectionNameParts = splitTokens(lineText);
               m_currentSection = new WorkSection();
               m_currentSection.Number = Convert.ToInt32(sectionNameParts[0].OriginalText.Substring(1));
               m_currentSection.StartLine = Lines.Count;
               m_currentSection.Name = sectionNameParts[0].OriginalText;
               m_currentSection.Description = Lines[m_currentSection.StartLine - 2].Text;

               m_status = STATUS_SECTION_BEGIN;
               line.IsSectionHeader = true;
            }
         }
         else if (STATUS_SECTION_BEGIN.Equals(m_status))
         {
            bool foundSectionEnd = false;
            Regex sectionEndRegex = new Regex(REGEX_SECTION_END);
            Match match = sectionEndRegex.Match(lineText);
            if (match.Success)
            {
               line.IsSectionFooter = true;

               foundSectionEnd = true;
            }
            else
            {
               Regex sectionEndSecondRegex1 = new Regex(REGEX_SECTION_END_SECOND_1);
               Regex sectionEndSecondRegex2 = new Regex(REGEX_SECTION_END_SECOND_2);
               Match match1 = sectionEndSecondRegex1.Match(lineText);
               Match match2 = sectionEndSecondRegex2.Match(lineText);
               // M5 or M9 missing.
               if (match1.Success)
               {
                  Line endLine = new Line();
                  endLine.Text = SECTION_END_1;
                  endLine.IsSectionFooter = true;
                  endLine.IsMissingLine = true;
                  Lines.Add(endLine);

                  foundSectionEnd = true;
               }
               else if (match2.Success)
               {
                  Line endLine = new Line();
                  endLine.Text = SECTION_END_2;
                  endLine.IsSectionFooter = true;
                  endLine.IsMissingLine = true;
                  Lines.Add(endLine);

                  foundSectionEnd = true;
               }
            }

            if (foundSectionEnd)
            {
               m_currentSection.EndLine = Lines.Count - 1;

               m_workSections.Add(m_currentSection);

               m_currentSection = null;
               m_status = STATUS_NONE;
            }
         }

         if (m_currentSection != null)
         {
            line.Section = m_currentSection;
         }

         if (line.Position > 1 && MARKER_FILE_END.Equals(lineText))
         {
            line.IsLastLine = true;
         }

         Lines.Add(line);
         return line;
      }

      public void checkFile()
      {
         foreach (Line line in Lines)
         {
            // Bypass line that is belong to a work section or header and footer of a section.
            if (line.Section == null
               || line.IsCommentLine || line.IsSectionHeader || line.IsSectionFooter || line.IsMissingLine)
            {
               continue;
            }

            // Parse words of the line into tokens.
            line.TokenList = splitTokens(line.Text);

            // check token error
            foreach (Token token in line.TokenList)
            {
               token.ErrorCode = Const.ErrorCode.ERROR_CODE_NONE;

               foreach (String regex in REGEX_TOKEN_CHECK_LIST)
               {
                  Regex regexToken = new Regex(regex);
                  Match match = regexToken.Match(token.OriginalText);
                  if (match.Success)
                  {
                     // Predefined tokens with Number that mismatch Section Number is considered errornous.
                     int tokenNumber = Convert.ToInt32(token.OriginalText.Substring(1));
                     if (tokenNumber != line.Section.Number)
                     {
                        token.ErrorCode = Const.ErrorCode.ERROR_CODE_SECTION_ID_MISMATCH;
                        this.ErrorCount++;

                        token.FixedText = token.OriginalText.Substring(0, 1) + line.Section.Number;
                     }
                     else
                     {
                        token.ErrorCode = Const.ErrorCode.ERROR_CODE_OK;
                     }

                     break;
                  }
               }
            }
         }
      }

      public List<String> getFixedText()
      {
         List<String> fixedLines = new List<String>();
         foreach (Line line in Lines)
         {
            // Bypass line that is belong to a work section or header and footer of a section.
            if (line.Section == null
               || line.IsSectionHeader || line.IsSectionFooter)
            {
               fixedLines.Add(line.Text);
               continue;
            }

            // Get fixed text from token
            String lineText = "";
            foreach (Token token in line.TokenList)
            {
               if (token.ErrorCode == Const.ErrorCode.ERROR_CODE_SECTION_ID_MISMATCH)
               {
                  lineText += token.FixedText + token.Trailer;
               }
               else
               {
                  lineText += token.OriginalText + token.Trailer;
               }
            }

            lineText.Trim();
            fixedLines.Add(lineText);
         }

         return fixedLines;
      }

      private List<Token> splitTokens(String lineText)
      {
         List<Token> tokenList = new List<Token>();
         Regex regexToken = new Regex(REGEX_TOKEN, RegexOptions.IgnoreCase);
         Match matchResults = regexToken.Match(lineText);

         int previousMatchIndex = 0;
         Token previousToken = null;
         while (matchResults.Success)
         {
            Token token = new Token();
            token.OriginalText = matchResults.Value;
            token.Trailer = "";
            tokenList.Add(token);

            // Calculate trailer for previous token
            if (previousToken != null)
            {
               int previousTokenEndPos = previousMatchIndex + previousToken.OriginalText.Length;
               if (previousTokenEndPos < matchResults.Index)
               {
                  previousToken.Trailer = lineText.Substring(previousTokenEndPos, matchResults.Index - previousTokenEndPos);
               }
               else
               {
                  previousToken.Trailer = "";
               }
            }

            previousMatchIndex = matchResults.Index;
            previousToken = token;

            matchResults = matchResults.NextMatch();
         }

         return tokenList;
      }
   }
}
