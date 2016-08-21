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
                  showLine(line);
               }
            }
            fileStream.Close();
         }
      }

      private void checkFile_Click(object sender, EventArgs e)
      {
         m_ncService.checkFile();
      }

      private void showLine(Line line)
      {
         // Line number
         int startColorPos = m_rtNCOriginal.TextLength;
         int endColorPos = 6;
         String numberMarker = "    ";
         if (line.IsSectionHeader)
         {
            numberMarker = " " + NCService.SECTION_START + " ";
         }
         else if (line.IsSectionEnd)
         {
            numberMarker = " " + NCService.SECTION_END + " ";
         }
         m_rtNCOriginal.AppendText(line.Position.ToString().PadLeft(3, ' ') + numberMarker);
         m_rtNCOriginal.Select(startColorPos, endColorPos);
         m_rtNCOriginal.SelectionColor = Color.Black;
         m_rtNCOriginal.SelectionBackColor = Color.LightGray;

         // Text
         startColorPos = m_rtNCOriginal.TextLength;
         endColorPos = line.Text.Length;
         m_rtNCOriginal.AppendText(line.Text);
         m_rtNCOriginal.Select(startColorPos, endColorPos);
         if (line.IsSectionHeader || line.IsSectionEnd)
         {
            m_rtNCOriginal.SelectionColor = Color.Green;
            m_rtNCOriginal.SelectionBackColor = Color.Yellow;
         }
         else
         {
            m_rtNCOriginal.SelectionColor = Color.Black;
         }

         m_rtNCOriginal.AppendText(Environment.NewLine);
      }
   }
}
