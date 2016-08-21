using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.IO;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace NCCheck2
{
   public partial class NCCheck : Form
   {
      private NCService m_ncService;

      public NCCheck()
      {
         InitializeComponent();
      }

      private void openFile_Click(object sender, EventArgs e)
      {
         m_ncService = new NCService();

         // Create an instance of the open file dialog box.
         OpenFileDialog openFileDialog1 = new OpenFileDialog();

         // Set filter options and filter index.
         openFileDialog1.Filter = "CNC Files (.NC)|*.NC";
         openFileDialog1.FilterIndex = 1;
         openFileDialog1.Multiselect = false;

         // Call the ShowDialog method to show the dialog box.
         DialogResult result = openFileDialog1.ShowDialog();

         // Process input if the user clicked OK.
         if (DialogResult.OK.Equals(result))
         {
            // Open the selected file to read.
            Stream fileStream = openFileDialog1.OpenFile();

            using (StreamReader reader = new StreamReader(fileStream))
            {
               int linePos = 0;
               while (reader.Peek() >= 0)
               {
                  Line line = m_ncService.prepareLine(reader.ReadLine());
                  line.Position = ++linePos;
                  formatNormalLine(line, m_rtNCOriginal);
               }
            }
            fileStream.Close();
         }
      }

      private void checkFile_Click(object sender, EventArgs e)
      {
         m_ncService.checkFile();
         foreach (Line line in m_ncService.Lines)
         {
            displayCheckedLine(line, m_rtNCResult);
         }
      }

      private void formatNormalLine(Line line, RichTextBox textbox)
      {
         // Line number
         formatLineNumber(line, textbox);

         // Text
         int startColorPos = textbox.TextLength;
         int endColorPos = line.Text.Length;
         textbox.AppendText(line.Text);
         textbox.Select(startColorPos, endColorPos);
         if (line.IsSectionHeader || line.IsSectionFooter)
         {
            textbox.SelectionColor = Color.Green;
            textbox.SelectionBackColor = Color.Yellow;
         }
         else
         {
            textbox.SelectionColor = Color.Black;
         }

         textbox.AppendText(Environment.NewLine);
      }

      private void formatLineNumber(Line line, RichTextBox textbox)
      {
         int startColorPos = textbox.TextLength;
         int endColorPos = 6;
         String numberMarker = "    ";
         if (line.IsSectionHeader)
         {
            numberMarker = " " + NCService.SECTION_START + " ";
         }
         else if (line.IsSectionFooter)
         {
            numberMarker = " " + NCService.SECTION_END + " ";
         }
         textbox.AppendText(line.Position.ToString().PadLeft(3, ' ') + numberMarker);
         textbox.Select(startColorPos, endColorPos);
         textbox.SelectionColor = Color.Black;
         textbox.SelectionBackColor = Color.LightGray;
      }

      private void formatSectionLine(Line line, RichTextBox textbox)
      {
         // Line number
         formatLineNumber(line, textbox);

         // Checked Text
         foreach (Token token in line.TokenList)
         {
            int startColorPos = textbox.TextLength;
            int endColorPos = token.Text.Length;
            textbox.AppendText(token.Text + " ");
            textbox.Select(startColorPos, endColorPos);
            if (Const.ERROR_SECTION_ID_MISMATCH.Equals(token.errorCode))
            {
               textbox.SelectionColor = Color.White;
               textbox.SelectionBackColor = Color.Red;
            }
            else
            {
               textbox.SelectionColor = Color.White;
               textbox.SelectionBackColor = Color.Green;
            }
         }

         textbox.AppendText(Environment.NewLine);
      }

      private void displayCheckedLine(Line line, RichTextBox textbox)
      {
         if (line.TokenList == null)
         {
            formatNormalLine(line, textbox);
         }
         else
         {
            formatSectionLine(line, textbox);
         }
      }
   }
}
