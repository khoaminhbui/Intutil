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
               while (reader.Peek() >= 0)
               {
                  String processedLine = m_ncService.checkLine(reader.ReadLine());
                  showLine(processedLine);
               }
            }
            fileStream.Close();
         }
      }

      private void checkFile_Click(object sender, EventArgs e)
      {

      }

      private void showLine(String line)
      {
         m_rtNC.AppendText(line);
         m_rtNC.AppendText(Environment.NewLine);
      }
   }
}
