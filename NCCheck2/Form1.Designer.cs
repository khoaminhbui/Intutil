namespace NCCheck2
{
   partial class NCCheck
   {
      /// <summary>
      /// Required designer variable.
      /// </summary>
      private System.ComponentModel.IContainer components = null;

      /// <summary>
      /// Clean up any resources being used.
      /// </summary>
      /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
      protected override void Dispose(bool disposing)
      {
         if (disposing && (components != null))
         {
            components.Dispose();
         }
         base.Dispose(disposing);
      }

      #region Windows Form Designer generated code

      /// <summary>
      /// Required method for Designer support - do not modify
      /// the contents of this method with the code editor.
      /// </summary>
      private void InitializeComponent()
      {
         System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(NCCheck));
         this.m_rtNC = new System.Windows.Forms.RichTextBox();
         this.toolStrip1 = new System.Windows.Forms.ToolStrip();
         this.m_openFile = new System.Windows.Forms.ToolStripButton();
         this.m_checkFile = new System.Windows.Forms.ToolStripButton();
         this.toolStrip1.SuspendLayout();
         this.SuspendLayout();
         // 
         // m_rtNC
         // 
         this.m_rtNC.Location = new System.Drawing.Point(12, 39);
         this.m_rtNC.Name = "m_rtNC";
         this.m_rtNC.Size = new System.Drawing.Size(777, 451);
         this.m_rtNC.TabIndex = 0;
         this.m_rtNC.Text = "";
         // 
         // toolStrip1
         // 
         this.toolStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.m_openFile,
            this.m_checkFile});
         this.toolStrip1.Location = new System.Drawing.Point(0, 0);
         this.toolStrip1.Name = "toolStrip1";
         this.toolStrip1.Size = new System.Drawing.Size(801, 25);
         this.toolStrip1.TabIndex = 1;
         this.toolStrip1.Text = "toolStrip1";
         // 
         // m_openFile
         // 
         this.m_openFile.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
         this.m_openFile.Image = ((System.Drawing.Image)(resources.GetObject("m_openFile.Image")));
         this.m_openFile.ImageTransparentColor = System.Drawing.Color.Magenta;
         this.m_openFile.Name = "m_openFile";
         this.m_openFile.Size = new System.Drawing.Size(23, 22);
         this.m_openFile.Text = "Open";
         this.m_openFile.Click += new System.EventHandler(this.openFile_Click);
         // 
         // m_checkFile
         // 
         this.m_checkFile.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
         this.m_checkFile.Image = ((System.Drawing.Image)(resources.GetObject("m_checkFile.Image")));
         this.m_checkFile.ImageTransparentColor = System.Drawing.Color.Magenta;
         this.m_checkFile.Name = "m_checkFile";
         this.m_checkFile.Size = new System.Drawing.Size(23, 22);
         this.m_checkFile.Text = "Check";
         this.m_checkFile.Click += new System.EventHandler(this.checkFile_Click);
         // 
         // NCCheck
         // 
         this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
         this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
         this.ClientSize = new System.Drawing.Size(801, 502);
         this.Controls.Add(this.toolStrip1);
         this.Controls.Add(this.m_rtNC);
         this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
         this.Name = "NCCheck";
         this.Text = "NCCheck";
         this.toolStrip1.ResumeLayout(false);
         this.toolStrip1.PerformLayout();
         this.ResumeLayout(false);
         this.PerformLayout();

      }

      #endregion

      private System.Windows.Forms.RichTextBox m_rtNC;
      private System.Windows.Forms.ToolStrip toolStrip1;
      private System.Windows.Forms.ToolStripButton m_openFile;
      private System.Windows.Forms.ToolStripButton m_checkFile;
   }
}

