
namespace CodeFileTools
{
    partial class Form1
    {
        /// <summary>
        ///  Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        ///  Clean up any resources being used.
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
        ///  Required method for Designer support - do not modify
        ///  the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.TxtDirPath = new System.Windows.Forms.TextBox();
            this.LbDirPath = new System.Windows.Forms.Label();
            this.BtnStartCreate = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // TxtDirPath
            // 
            this.TxtDirPath.Location = new System.Drawing.Point(288, 155);
            this.TxtDirPath.Name = "TxtDirPath";
            this.TxtDirPath.Size = new System.Drawing.Size(162, 23);
            this.TxtDirPath.TabIndex = 0;
            this.TxtDirPath.Text = "D:/代码生成工具";
            // 
            // LbDirPath
            // 
            this.LbDirPath.AutoSize = true;
            this.LbDirPath.Location = new System.Drawing.Point(200, 158);
            this.LbDirPath.Name = "LbDirPath";
            this.LbDirPath.Size = new System.Drawing.Size(80, 17);
            this.LbDirPath.TabIndex = 2;
            this.LbDirPath.Text = "文件夹路径：";
            // 
            // BtnStartCreate
            // 
            this.BtnStartCreate.Location = new System.Drawing.Point(309, 205);
            this.BtnStartCreate.Name = "BtnStartCreate";
            this.BtnStartCreate.Size = new System.Drawing.Size(75, 23);
            this.BtnStartCreate.TabIndex = 20;
            this.BtnStartCreate.Text = "开始生成";
            this.BtnStartCreate.UseVisualStyleBackColor = true;
            this.BtnStartCreate.Click += new System.EventHandler(this.BtnStartCreate_Click);
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 17F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(748, 515);
            this.Controls.Add(this.BtnStartCreate);
            this.Controls.Add(this.LbDirPath);
            this.Controls.Add(this.TxtDirPath);
            this.Name = "Form1";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Form1";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        private System.Windows.Forms.TextBox TxtDirPath;
        private System.Windows.Forms.Label LbDirPath;

        #endregion

        private System.Windows.Forms.Button BtnStartCreate;
    }
}

