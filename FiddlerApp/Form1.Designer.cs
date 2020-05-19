namespace FiddlerApp
{
    partial class Form1
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
            this.txtlog = new System.Windows.Forms.TextBox();
            this.btnstart = new System.Windows.Forms.Button();
            this.btnout = new System.Windows.Forms.Button();
            this.btn_clear = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // txtlog
            // 
            this.txtlog.Location = new System.Drawing.Point(12, 124);
            this.txtlog.Multiline = true;
            this.txtlog.Name = "txtlog";
            this.txtlog.ScrollBars = System.Windows.Forms.ScrollBars.Both;
            this.txtlog.Size = new System.Drawing.Size(776, 314);
            this.txtlog.TabIndex = 0;
            // 
            // btnstart
            // 
            this.btnstart.Location = new System.Drawing.Point(214, 51);
            this.btnstart.Name = "btnstart";
            this.btnstart.Size = new System.Drawing.Size(110, 40);
            this.btnstart.TabIndex = 1;
            this.btnstart.Text = "开始";
            this.btnstart.UseVisualStyleBackColor = true;
            this.btnstart.Click += new System.EventHandler(this.btnstart_Click);
            // 
            // btnout
            // 
            this.btnout.Location = new System.Drawing.Point(362, 51);
            this.btnout.Name = "btnout";
            this.btnout.Size = new System.Drawing.Size(101, 40);
            this.btnout.TabIndex = 2;
            this.btnout.Text = "退出代理";
            this.btnout.UseVisualStyleBackColor = true;
            this.btnout.Click += new System.EventHandler(this.btnout_Click);
            // 
            // btn_clear
            // 
            this.btn_clear.Location = new System.Drawing.Point(588, 79);
            this.btn_clear.Name = "btn_clear";
            this.btn_clear.Size = new System.Drawing.Size(75, 23);
            this.btn_clear.TabIndex = 3;
            this.btn_clear.Text = "清除日志";
            this.btn_clear.UseVisualStyleBackColor = true;
            this.btn_clear.Click += new System.EventHandler(this.btn_clear_Click);
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(800, 450);
            this.Controls.Add(this.btn_clear);
            this.Controls.Add(this.btnout);
            this.Controls.Add(this.btnstart);
            this.Controls.Add(this.txtlog);
            this.Name = "Form1";
            this.Text = "Form1";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.TextBox txtlog;
        private System.Windows.Forms.Button btnstart;
        private System.Windows.Forms.Button btnout;
        private System.Windows.Forms.Button btn_clear;
    }
}

