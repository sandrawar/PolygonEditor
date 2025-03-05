namespace Graphics_project1
{
    partial class PolygonEditor
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
            AddButton = new RadioButton();
            EditButton = new RadioButton();
            DeleteButton = new RadioButton();
            pictureBox1 = new PictureBox();
            radioButton1 = new RadioButton();
            independentPanel = new Panel();
            radioButton2 = new RadioButton();
            label1 = new Label();
            ((System.ComponentModel.ISupportInitialize)pictureBox1).BeginInit();
            independentPanel.SuspendLayout();
            SuspendLayout();
            // 
            // AddButton
            // 
            AddButton.AutoSize = true;
            AddButton.Location = new Point(8, 82);
            AddButton.Name = "AddButton";
            AddButton.Size = new Size(71, 29);
            AddButton.TabIndex = 0;
            AddButton.Text = "Add";
            AddButton.UseVisualStyleBackColor = true;
            AddButton.CheckedChanged += AddButton_CheckedChanged;
            // 
            // EditButton
            // 
            EditButton.AutoSize = true;
            EditButton.Location = new Point(8, 12);
            EditButton.Name = "EditButton";
            EditButton.Size = new Size(67, 29);
            EditButton.TabIndex = 1;
            EditButton.Text = "Edit";
            EditButton.UseVisualStyleBackColor = true;
            EditButton.CheckedChanged += EditButton_CheckedChanged;
            // 
            // DeleteButton
            // 
            DeleteButton.AutoSize = true;
            DeleteButton.Location = new Point(8, 47);
            DeleteButton.Name = "DeleteButton";
            DeleteButton.Size = new Size(87, 29);
            DeleteButton.TabIndex = 2;
            DeleteButton.Text = "Delete";
            DeleteButton.UseVisualStyleBackColor = true;
            DeleteButton.CheckedChanged += DeleteButton_CheckedChanged;
            // 
            // pictureBox1
            // 
            pictureBox1.Location = new Point(-1, 117);
            pictureBox1.Name = "pictureBox1";
            pictureBox1.Size = new Size(1192, 623);
            pictureBox1.TabIndex = 3;
            pictureBox1.TabStop = false;
            // 
            // radioButton1
            // 
            radioButton1.AutoSize = true;
            radioButton1.Location = new Point(4, 12);
            radioButton1.Name = "radioButton1";
            radioButton1.Size = new Size(314, 29);
            radioButton1.TabIndex = 4;
            radioButton1.Text = "line drawing - own implemantation";
            radioButton1.UseVisualStyleBackColor = true;
            radioButton1.CheckedChanged += radioButton1_CheckedChanged;
            // 
            // independentPanel
            // 
            independentPanel.Controls.Add(radioButton1);
            independentPanel.Controls.Add(radioButton2);
            independentPanel.Location = new Point(867, 35);
            independentPanel.Name = "independentPanel";
            independentPanel.Size = new Size(640, 76);
            independentPanel.TabIndex = 0;
            // 
            // radioButton2
            // 
            radioButton2.AutoSize = true;
            radioButton2.Location = new Point(3, 44);
            radioButton2.Name = "radioButton2";
            radioButton2.Size = new Size(269, 29);
            radioButton2.TabIndex = 5;
            radioButton2.Text = "line drawing - library function";
            radioButton2.UseVisualStyleBackColor = true;
            radioButton2.CheckedChanged += radioButton2_CheckedChanged;
            // 
            // label1
            // 
            label1.AutoSize = true;
            label1.Location = new Point(140, 12);
            label1.Name = "label1";
            label1.Size = new Size(590, 250);
            label1.TabIndex = 4;
            label1.Text = "Chose mode";
            // 
            // Form1
            // 
            AutoScaleDimensions = new SizeF(10F, 25F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(1193, 741);
            Controls.Add(label1);
            Controls.Add(independentPanel);
            Controls.Add(pictureBox1);
            Controls.Add(DeleteButton);
            Controls.Add(EditButton);
            Controls.Add(AddButton);
            Name = "Form1";
            Text = "Form1";
            ((System.ComponentModel.ISupportInitialize)pictureBox1).EndInit();
            independentPanel.ResumeLayout(false);
            independentPanel.PerformLayout();
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private RadioButton AddButton;
        private RadioButton EditButton;
        private RadioButton DeleteButton;
        private PictureBox pictureBox1;
        private RadioButton radioButton1;
        private Panel independentPanel;
        private RadioButton radioButton2;
        private Label label1;
    }
}
