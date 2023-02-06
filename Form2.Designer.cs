
namespace WindowsInterface
{
    partial class Form2_CreateAccaunt
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Form2_CreateAccaunt));
            this.btn_Start = new System.Windows.Forms.Button();
            this.btn_ClearTextBox = new System.Windows.Forms.Button();
            this.btn_Cancel = new System.Windows.Forms.Button();
            this.InputName = new System.Windows.Forms.TextBox();
            this.btn_Label = new System.Windows.Forms.Button();
            this.label1 = new System.Windows.Forms.Label();
            this.ss_NationName = new System.Windows.Forms.StatusStrip();
            this.toolStripStatusLabel1 = new System.Windows.Forms.ToolStripStatusLabel();
            this.nUD_Width = new System.Windows.Forms.NumericUpDown();
            this.label2 = new System.Windows.Forms.Label();
            this.nUD_Height = new System.Windows.Forms.NumericUpDown();
            this.label3 = new System.Windows.Forms.Label();
            this.nUD_CountOfBots = new System.Windows.Forms.NumericUpDown();
            this.label4 = new System.Windows.Forms.Label();
            this.label5 = new System.Windows.Forms.Label();
            this.label6 = new System.Windows.Forms.Label();
            this.label7 = new System.Windows.Forms.Label();
            this.label8 = new System.Windows.Forms.Label();
            this.ss_NationName.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.nUD_Width)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.nUD_Height)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.nUD_CountOfBots)).BeginInit();
            this.SuspendLayout();
            // 
            // btn_Start
            // 
            this.btn_Start.BackColor = System.Drawing.Color.Black;
            this.btn_Start.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btn_Start.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.btn_Start.ForeColor = System.Drawing.Color.Red;
            this.btn_Start.Location = new System.Drawing.Point(90, 385);
            this.btn_Start.Name = "btn_Start";
            this.btn_Start.Size = new System.Drawing.Size(360, 40);
            this.btn_Start.TabIndex = 2;
            this.btn_Start.Text = "СОЗДАТЬ МИР TRAVIAN";
            this.btn_Start.UseVisualStyleBackColor = false;
            this.btn_Start.Click += new System.EventHandler(this.btn_Start_Click);
            this.btn_Start.Paint += new System.Windows.Forms.PaintEventHandler(this.btn_Label_Paint);
            // 
            // btn_ClearTextBox
            // 
            this.btn_ClearTextBox.BackColor = System.Drawing.Color.Black;
            this.btn_ClearTextBox.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.btn_ClearTextBox.ForeColor = System.Drawing.Color.White;
            this.btn_ClearTextBox.Location = new System.Drawing.Point(90, 340);
            this.btn_ClearTextBox.Name = "btn_ClearTextBox";
            this.btn_ClearTextBox.Size = new System.Drawing.Size(360, 40);
            this.btn_ClearTextBox.TabIndex = 3;
            this.btn_ClearTextBox.Text = "ОЧИСТИТЬ ВВОД";
            this.btn_ClearTextBox.UseVisualStyleBackColor = false;
            this.btn_ClearTextBox.Click += new System.EventHandler(this.btn_ClearTextBox_Click);
            this.btn_ClearTextBox.Paint += new System.Windows.Forms.PaintEventHandler(this.btn_Label_Paint);
            // 
            // btn_Cancel
            // 
            this.btn_Cancel.BackColor = System.Drawing.Color.Black;
            this.btn_Cancel.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.btn_Cancel.ForeColor = System.Drawing.Color.White;
            this.btn_Cancel.Location = new System.Drawing.Point(90, 430);
            this.btn_Cancel.Name = "btn_Cancel";
            this.btn_Cancel.Size = new System.Drawing.Size(360, 40);
            this.btn_Cancel.TabIndex = 4;
            this.btn_Cancel.Text = "Отмена";
            this.btn_Cancel.UseVisualStyleBackColor = false;
            this.btn_Cancel.Click += new System.EventHandler(this.btn_Cancel_Click);
            this.btn_Cancel.Paint += new System.Windows.Forms.PaintEventHandler(this.btn_Label_Paint);
            // 
            // InputName
            // 
            this.InputName.Font = new System.Drawing.Font("Microsoft Sans Serif", 14F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.InputName.Location = new System.Drawing.Point(15, 140);
            this.InputName.Name = "InputName";
            this.InputName.Size = new System.Drawing.Size(490, 34);
            this.InputName.TabIndex = 6;
            this.InputName.TextChanged += new System.EventHandler(this.InputName_TextChanged);
            // 
            // btn_Label
            // 
            this.btn_Label.Font = new System.Drawing.Font("Microsoft Sans Serif", 24F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.btn_Label.Location = new System.Drawing.Point(0, 0);
            this.btn_Label.Name = "btn_Label";
            this.btn_Label.Size = new System.Drawing.Size(522, 83);
            this.btn_Label.TabIndex = 7;
            this.btn_Label.Text = "РЕГИСТРАЦИЯ";
            this.btn_Label.UseVisualStyleBackColor = true;
            this.btn_Label.Paint += new System.Windows.Forms.PaintEventHandler(this.btn_Label_Paint);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.BackColor = System.Drawing.Color.Transparent;
            this.label1.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.label1.ForeColor = System.Drawing.Color.Black;
            this.label1.Location = new System.Drawing.Point(10, 115);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(407, 20);
            this.label1.TabIndex = 8;
            this.label1.Text = "ВВЕДИТЕ ИГРОВОЙ НИК (до 30 символов):";
            // 
            // ss_NationName
            // 
            this.ss_NationName.BackColor = System.Drawing.SystemColors.ButtonFace;
            this.ss_NationName.Font = new System.Drawing.Font("Segoe UI", 10F, System.Drawing.FontStyle.Bold);
            this.ss_NationName.ImageScalingSize = new System.Drawing.Size(20, 20);
            this.ss_NationName.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.toolStripStatusLabel1});
            this.ss_NationName.Location = new System.Drawing.Point(0, 514);
            this.ss_NationName.Name = "ss_NationName";
            this.ss_NationName.Size = new System.Drawing.Size(522, 29);
            this.ss_NationName.TabIndex = 9;
            this.ss_NationName.Text = "statusStrip1";
            // 
            // toolStripStatusLabel1
            // 
            this.toolStripStatusLabel1.ForeColor = System.Drawing.Color.Black;
            this.toolStripStatusLabel1.Name = "toolStripStatusLabel1";
            this.toolStripStatusLabel1.Size = new System.Drawing.Size(111, 23);
            this.toolStripStatusLabel1.Text = "NationName";
            // 
            // nUD_Width
            // 
            this.nUD_Width.Font = new System.Drawing.Font("Microsoft Sans Serif", 14F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.nUD_Width.ForeColor = System.Drawing.Color.Blue;
            this.nUD_Width.Location = new System.Drawing.Point(15, 215);
            this.nUD_Width.Maximum = new decimal(new int[] {
            400,
            0,
            0,
            0});
            this.nUD_Width.Minimum = new decimal(new int[] {
            10,
            0,
            0,
            0});
            this.nUD_Width.Name = "nUD_Width";
            this.nUD_Width.Size = new System.Drawing.Size(100, 34);
            this.nUD_Width.TabIndex = 10;
            this.nUD_Width.Value = new decimal(new int[] {
            10,
            0,
            0,
            0});
            this.nUD_Width.ValueChanged += new System.EventHandler(this.nUD_Width_ValueChanged);
            this.nUD_Width.Click += new System.EventHandler(this.nUD_Width_ValueChanged);
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.BackColor = System.Drawing.Color.Transparent;
            this.label2.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.label2.ForeColor = System.Drawing.Color.Black;
            this.label2.Location = new System.Drawing.Point(10, 190);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(162, 20);
            this.label2.TabIndex = 11;
            this.label2.Text = "РАЗМЕР КАРТЫ:";
            // 
            // nUD_Height
            // 
            this.nUD_Height.Font = new System.Drawing.Font("Microsoft Sans Serif", 14F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.nUD_Height.ForeColor = System.Drawing.Color.Blue;
            this.nUD_Height.Location = new System.Drawing.Point(130, 215);
            this.nUD_Height.Maximum = new decimal(new int[] {
            400,
            0,
            0,
            0});
            this.nUD_Height.Minimum = new decimal(new int[] {
            10,
            0,
            0,
            0});
            this.nUD_Height.Name = "nUD_Height";
            this.nUD_Height.Size = new System.Drawing.Size(100, 34);
            this.nUD_Height.TabIndex = 12;
            this.nUD_Height.Value = new decimal(new int[] {
            10,
            0,
            0,
            0});
            this.nUD_Height.ValueChanged += new System.EventHandler(this.nUD_Width_ValueChanged);
            this.nUD_Height.Click += new System.EventHandler(this.nUD_Width_ValueChanged);
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.BackColor = System.Drawing.Color.Transparent;
            this.label3.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.label3.ForeColor = System.Drawing.Color.Black;
            this.label3.Location = new System.Drawing.Point(10, 265);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(159, 20);
            this.label3.TabIndex = 13;
            this.label3.Text = "КОЛ-ВО БОТОВ:";
            // 
            // nUD_CountOfBots
            // 
            this.nUD_CountOfBots.Font = new System.Drawing.Font("Microsoft Sans Serif", 14F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.nUD_CountOfBots.ForeColor = System.Drawing.Color.Blue;
            this.nUD_CountOfBots.Location = new System.Drawing.Point(15, 290);
            this.nUD_CountOfBots.Maximum = new decimal(new int[] {
            1000,
            0,
            0,
            0});
            this.nUD_CountOfBots.Name = "nUD_CountOfBots";
            this.nUD_CountOfBots.Size = new System.Drawing.Size(154, 34);
            this.nUD_CountOfBots.TabIndex = 14;
            this.nUD_CountOfBots.Value = new decimal(new int[] {
            10,
            0,
            0,
            0});
            this.nUD_CountOfBots.ValueChanged += new System.EventHandler(this.nUD_Width_ValueChanged);
            this.nUD_CountOfBots.Click += new System.EventHandler(this.nUD_Width_ValueChanged);
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.label4.ForeColor = System.Drawing.Color.Black;
            this.label4.Location = new System.Drawing.Point(262, 202);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(155, 20);
            this.label4.TabIndex = 15;
            this.label4.Text = "X: [-10..0; 0..+10]";
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.label5.ForeColor = System.Drawing.Color.Black;
            this.label5.Location = new System.Drawing.Point(262, 222);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(154, 20);
            this.label5.TabIndex = 16;
            this.label5.Text = "Y: [-10..0; 0..+10]";
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.label6.ForeColor = System.Drawing.Color.Black;
            this.label6.Location = new System.Drawing.Point(262, 242);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(193, 20);
            this.label6.TabIndex = 17;
            this.label6.Text = "Массив: [0..20][0..20]";
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.label7.ForeColor = System.Drawing.Color.Black;
            this.label7.Location = new System.Drawing.Point(262, 295);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(98, 20);
            this.label7.TabIndex = 18;
            this.label7.Text = "Ботов: 10";
            // 
            // label8
            // 
            this.label8.AutoSize = true;
            this.label8.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.label8.ForeColor = System.Drawing.Color.Gray;
            this.label8.Location = new System.Drawing.Point(10, 480);
            this.label8.Name = "label8";
            this.label8.Size = new System.Drawing.Size(495, 20);
            this.label8.TabIndex = 19;
            this.label8.Text = "Двойной клик заполняет поля ввода по умолчанию.";
            // 
            // Form2_CreateAccaunt
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(9F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.SystemColors.ButtonFace;
            this.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.ClientSize = new System.Drawing.Size(522, 543);
            this.ControlBox = false;
            this.Controls.Add(this.label8);
            this.Controls.Add(this.label7);
            this.Controls.Add(this.label6);
            this.Controls.Add(this.label5);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.nUD_CountOfBots);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.nUD_Height);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.nUD_Width);
            this.Controls.Add(this.ss_NationName);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.btn_Label);
            this.Controls.Add(this.InputName);
            this.Controls.Add(this.btn_Cancel);
            this.Controls.Add(this.btn_ClearTextBox);
            this.Controls.Add(this.btn_Start);
            this.Font = new System.Drawing.Font("Microsoft Sans Serif", 7.8F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.ForeColor = System.Drawing.Color.Blue;
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.KeyPreview = true;
            this.Name = "Form2_CreateAccaunt";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Создать аккаунт";
            this.Load += new System.EventHandler(this.Form2_CreateAccaunt_Load);
            this.Click += new System.EventHandler(this.Form2_CreateAccaunt_Click);
            this.DoubleClick += new System.EventHandler(this.Form2_CreateAccaunt_DoubleClick);
            this.ss_NationName.ResumeLayout(false);
            this.ss_NationName.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.nUD_Width)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.nUD_Height)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.nUD_CountOfBots)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion
        public System.Windows.Forms.Label label1;
        public System.Windows.Forms.Button btn_Label;
        public System.Windows.Forms.Button btn_Start;
        public System.Windows.Forms.TextBox InputName;
        public System.Windows.Forms.StatusStrip ss_NationName;
        private System.Windows.Forms.ToolStripStatusLabel toolStripStatusLabel1;
        public System.Windows.Forms.NumericUpDown nUD_Width;
        public System.Windows.Forms.Label label2;
        public System.Windows.Forms.NumericUpDown nUD_Height;
        public System.Windows.Forms.NumericUpDown nUD_CountOfBots;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Label label5;
        public System.Windows.Forms.Label label6;
        public System.Windows.Forms.Label label7;
        public System.Windows.Forms.Label label3;
        public System.Windows.Forms.Button btn_ClearTextBox;
        public System.Windows.Forms.Button btn_Cancel;
        public System.Windows.Forms.Label label8;
    }
}