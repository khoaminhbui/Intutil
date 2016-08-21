using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.IO;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Runtime.InteropServices;

namespace NCCheck
{
   public enum ScrollBarType : uint
   {
      SbHorz = 0,
      SbVert = 1,
      SbCtl = 2,
      SbBoth = 3
   }

   public enum Message : uint
   {
      WM_VSCROLL = 0x0115
   }

   public enum ScrollBarCommands : uint
   {
      SB_THUMBPOSITION = 4
   }

   public partial class NCCheck : Form
   {
      private NCService m_ncService;

      [DllImport("User32.dll")]
      public extern static int GetScrollPos(IntPtr hWnd, int nBar);
      [DllImport("User32.dll")]
      public extern static int SendMessage(IntPtr hWnd, uint msg, IntPtr wParam, IntPtr lParam);

      public NCCheck()
      {
         InitializeComponent();
      }

      private void openFile_Click(object sender, EventArgs e)
      {
         resetView();

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
                  //formatNormalLine(line, m_rtNCOriginal);
               }
            }
            fileStream.Close();

            // check
            m_ncService.checkFile();
            updateStatistic(m_ncService.ErrorCount);
            
            foreach (Line line in m_ncService.Lines)
            {
               displayCheckedLine(line, m_rtNCOriginal);
            }
         }
      }

      private void checkFile_Click(object sender, EventArgs e)
      {
         
      }

      private void resetView()
      {
         this.m_rtNCOriginal.Text = "";
         this.m_rtNCResult.Text = "";
         this.m_lblErrorCount.Text = "";
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
            textbox.SelectionBackColor = Color.White;
         }

         if (!line.IsLastLine)
         {
            textbox.AppendText(Environment.NewLine);
         }
      }

      private void formatLineNumber(Line line, RichTextBox textbox)
      {
         int startColorPos = textbox.TextLength;
         int endColorPos = 5;
         String numberMarker = "  ";
         if (line.IsSectionHeader)
         {
            numberMarker = " " + NCService.SECTION_START;
         }
         else if (line.IsSectionFooter)
         {
            numberMarker = " " + NCService.SECTION_END;
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
            textbox.AppendText(token.Text);
            textbox.Select(startColorPos, endColorPos);
            if (Const.ErrorCode.ERROR_CODE_SECTION_ID_MISMATCH.Equals(token.ErrorCode))
            {
               textbox.SelectionColor = Color.White;
               textbox.SelectionBackColor = Color.Red;
            }
            else if (Const.ErrorCode.ERROR_CODE_OK.Equals(token.ErrorCode))
            {
               textbox.SelectionColor = Color.White;
               textbox.SelectionBackColor = Color.Green;
            }
            else
            {
               textbox.SelectionColor = Color.Black;
               textbox.SelectionBackColor = Color.White;
            }

            // space
            textbox.AppendText(" ");
            textbox.Select(textbox.TextLength - 1, 1);
            textbox.SelectionColor = Color.Black;
            textbox.SelectionBackColor = Color.White;
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

      private void updateStatistic(int errorCount)
      {
         if (errorCount > 1)
         {
            m_lblErrorCount.Text = errorCount.ToString() + " errors";
            m_lblErrorCount.ForeColor = Color.Red;
         }
         else if (errorCount == 1)
         {
            m_lblErrorCount.Text = errorCount.ToString() + " error";
            m_lblErrorCount.ForeColor = Color.Red;
         }
         else
         {
            m_lblErrorCount.Text = "No error";
            m_lblErrorCount.ForeColor = Color.Green;
         }
      }
   }
}
