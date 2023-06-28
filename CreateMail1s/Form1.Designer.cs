namespace CreateMail1s
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
            Take = new Button();
            Copy = new Button();
            txtMail = new TextBox();
            txtNumber = new TextBox();
            SuspendLayout();
            // 
            // Take
            // 
            Take.Location = new Point(258, 30);
            Take.Name = "Take";
            Take.Size = new Size(75, 23);
            Take.TabIndex = 0;
            Take.Text = "Take&Copy";
            Take.UseVisualStyleBackColor = true;
            Take.Click += Take_Click;
            // 
            // Copy
            // 
            Copy.Location = new Point(258, 71);
            Copy.Name = "Copy";
            Copy.Size = new Size(75, 23);
            Copy.TabIndex = 1;
            Copy.Text = "Copy";
            Copy.UseVisualStyleBackColor = true;
            // 
            // txtMail
            // 
            txtMail.Location = new Point(70, 30);
            txtMail.Name = "txtMail";
            txtMail.Size = new Size(160, 23);
            txtMail.TabIndex = 2;
            // 
            // txtNumber
            // 
            txtNumber.Location = new Point(12, 31);
            txtNumber.Name = "txtNumber";
            txtNumber.Size = new Size(38, 23);
            txtNumber.TabIndex = 3;
            // 
            // Form1
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(800, 450);
            Controls.Add(txtNumber);
            Controls.Add(txtMail);
            Controls.Add(Copy);
            Controls.Add(Take);
            Name = "Form1";
            Text = "Form1";
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private Button Take;
        private Button Copy;
        private TextBox txtMail;
        private TextBox txtNumber;
    }
}