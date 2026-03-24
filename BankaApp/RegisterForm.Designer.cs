
namespace BankaApp
{
    partial class RegisterForm
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(RegisterForm));
            this.panel1 = new System.Windows.Forms.Panel();
            this.button1 = new System.Windows.Forms.Button();
            this.crtAccount = new System.Windows.Forms.Button();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.city = new System.Windows.Forms.TextBox();
            this.streetId = new System.Windows.Forms.TextBox();
            this.EGN = new System.Windows.Forms.TextBox();
            this.adress = new System.Windows.Forms.TextBox();
            this.cmbCountry = new System.Windows.Forms.ComboBox();
            this.label3 = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.age = new System.Windows.Forms.TextBox();
            this.label2 = new System.Windows.Forms.Label();
            this.radioBtnWoman = new System.Windows.Forms.RadioButton();
            this.radioBtnMan = new System.Windows.Forms.RadioButton();
            this.pass = new System.Windows.Forms.TextBox();
            this.phoneNum = new System.Windows.Forms.TextBox();
            this.email = new System.Windows.Forms.TextBox();
            this.username = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.panel2 = new System.Windows.Forms.Panel();
            this.panel1.SuspendLayout();
            this.groupBox1.SuspendLayout();
            this.SuspendLayout();
            // 
            // panel1
            // 
            this.panel1.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.panel1.BackColor = System.Drawing.SystemColors.ButtonHighlight;
            this.panel1.Controls.Add(this.button1);
            this.panel1.Controls.Add(this.crtAccount);
            this.panel1.Controls.Add(this.groupBox1);
            this.panel1.Controls.Add(this.label1);
            this.panel1.Location = new System.Drawing.Point(1, 0);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(626, 535);
            this.panel1.TabIndex = 1;
            // 
            // button1
            // 
            this.button1.BackColor = System.Drawing.Color.Lavender;
            this.button1.Font = new System.Drawing.Font("Arial Narrow", 13.8F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point);
            this.button1.Location = new System.Drawing.Point(261, 471);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(175, 48);
            this.button1.TabIndex = 8;
            this.button1.Text = "Cancel";
            this.button1.UseVisualStyleBackColor = false;
            this.button1.Click += new System.EventHandler(this.button1_Click);
            // 
            // crtAccount
            // 
            this.crtAccount.BackColor = System.Drawing.Color.SpringGreen;
            this.crtAccount.Font = new System.Drawing.Font("Segoe UI", 16.2F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point);
            this.crtAccount.ForeColor = System.Drawing.SystemColors.ActiveCaptionText;
            this.crtAccount.Location = new System.Drawing.Point(191, 404);
            this.crtAccount.Name = "crtAccount";
            this.crtAccount.Size = new System.Drawing.Size(283, 48);
            this.crtAccount.TabIndex = 7;
            this.crtAccount.Text = "CREATE ACCOUNT";
            this.crtAccount.TextAlign = System.Drawing.ContentAlignment.TopCenter;
            this.crtAccount.UseVisualStyleBackColor = false;
            this.crtAccount.Click += new System.EventHandler(this.crtAccount_Click);
            // 
            // groupBox1
            // 
            this.groupBox1.BackColor = System.Drawing.SystemColors.ControlLightLight;
            this.groupBox1.Controls.Add(this.city);
            this.groupBox1.Controls.Add(this.streetId);
            this.groupBox1.Controls.Add(this.EGN);
            this.groupBox1.Controls.Add(this.adress);
            this.groupBox1.Controls.Add(this.cmbCountry);
            this.groupBox1.Controls.Add(this.label3);
            this.groupBox1.Controls.Add(this.label4);
            this.groupBox1.Controls.Add(this.age);
            this.groupBox1.Controls.Add(this.label2);
            this.groupBox1.Controls.Add(this.radioBtnWoman);
            this.groupBox1.Controls.Add(this.radioBtnMan);
            this.groupBox1.Controls.Add(this.pass);
            this.groupBox1.Controls.Add(this.phoneNum);
            this.groupBox1.Controls.Add(this.email);
            this.groupBox1.Controls.Add(this.username);
            this.groupBox1.Location = new System.Drawing.Point(23, 50);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(595, 348);
            this.groupBox1.TabIndex = 6;
            this.groupBox1.TabStop = false;
            this.groupBox1.Enter += new System.EventHandler(this.groupBox1_Enter);
            // 
            // city
            // 
            this.city.Location = new System.Drawing.Point(250, 129);
            this.city.Multiline = true;
            this.city.Name = "city";
            this.city.PlaceholderText = "City";
            this.city.Size = new System.Drawing.Size(178, 30);
            this.city.TabIndex = 16;
            this.city.TextChanged += new System.EventHandler(this.city_TextChanged);
            // 
            // streetId
            // 
            this.streetId.Location = new System.Drawing.Point(250, 48);
            this.streetId.Multiline = true;
            this.streetId.Name = "streetId";
            this.streetId.PlaceholderText = "street";
            this.streetId.Size = new System.Drawing.Size(178, 30);
            this.streetId.TabIndex = 15;
            this.streetId.TextChanged += new System.EventHandler(this.streetId_TextChanged);
            // 
            // EGN
            // 
            this.EGN.BackColor = System.Drawing.Color.Aquamarine;
            this.EGN.Location = new System.Drawing.Point(20, 303);
            this.EGN.Multiline = true;
            this.EGN.Name = "EGN";
            this.EGN.PlaceholderText = "EGN";
            this.EGN.Size = new System.Drawing.Size(166, 39);
            this.EGN.TabIndex = 14;
            this.EGN.TextChanged += new System.EventHandler(this.EGN_TextChanged);
            // 
            // adress
            // 
            this.adress.BackColor = System.Drawing.Color.Aquamarine;
            this.adress.Location = new System.Drawing.Point(250, 89);
            this.adress.Multiline = true;
            this.adress.Name = "adress";
            this.adress.PlaceholderText = "adress";
            this.adress.Size = new System.Drawing.Size(178, 30);
            this.adress.TabIndex = 13;
            this.adress.TextChanged += new System.EventHandler(this.adress_TextChanged);
            // 
            // cmbCountry
            // 
            this.cmbCountry.BackColor = System.Drawing.SystemColors.Info;
            this.cmbCountry.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbCountry.ForeColor = System.Drawing.SystemColors.InactiveCaption;
            this.cmbCountry.FormattingEnabled = true;
            this.cmbCountry.Location = new System.Drawing.Point(238, 215);
            this.cmbCountry.Name = "cmbCountry";
            this.cmbCountry.Size = new System.Drawing.Size(178, 28);
            this.cmbCountry.TabIndex = 9;
            this.cmbCountry.SelectedIndexChanged += new System.EventHandler(this.cmbCountry_SelectedIndexChanged);
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Font = new System.Drawing.Font("Segoe UI", 10.8F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point);
            this.label3.Location = new System.Drawing.Point(238, 187);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(94, 25);
            this.label3.TabIndex = 10;
            this.label3.Text = "Country *";
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.ForeColor = System.Drawing.Color.Red;
            this.label4.Location = new System.Drawing.Point(250, 284);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(0, 20);
            this.label4.TabIndex = 12;
            this.label4.Click += new System.EventHandler(this.label4_Click);
            // 
            // age
            // 
            this.age.Location = new System.Drawing.Point(20, 249);
            this.age.Multiline = true;
            this.age.Name = "age";
            this.age.PlaceholderText = "Year of Birth";
            this.age.Size = new System.Drawing.Size(166, 39);
            this.age.TabIndex = 11;
            this.age.TextChanged += new System.EventHandler(this.age_TextChanged);
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Font = new System.Drawing.Font("Segoe UI Semibold", 10.8F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point);
            this.label2.Location = new System.Drawing.Point(478, 149);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(87, 25);
            this.label2.TabIndex = 8;
            this.label2.Text = "Gender *";
            // 
            // radioBtnWoman
            // 
            this.radioBtnWoman.AutoSize = true;
            this.radioBtnWoman.Font = new System.Drawing.Font("Segoe UI", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point);
            this.radioBtnWoman.Location = new System.Drawing.Point(478, 216);
            this.radioBtnWoman.Name = "radioBtnWoman";
            this.radioBtnWoman.Size = new System.Drawing.Size(105, 32);
            this.radioBtnWoman.TabIndex = 7;
            this.radioBtnWoman.TabStop = true;
            this.radioBtnWoman.Text = "Woman";
            this.radioBtnWoman.UseVisualStyleBackColor = true;
            this.radioBtnWoman.CheckedChanged += new System.EventHandler(this.radioBtnWoman_CheckedChanged);
            // 
            // radioBtnMan
            // 
            this.radioBtnMan.AutoSize = true;
            this.radioBtnMan.Font = new System.Drawing.Font("Segoe UI", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point);
            this.radioBtnMan.Location = new System.Drawing.Point(478, 178);
            this.radioBtnMan.Name = "radioBtnMan";
            this.radioBtnMan.Size = new System.Drawing.Size(75, 32);
            this.radioBtnMan.TabIndex = 6;
            this.radioBtnMan.TabStop = true;
            this.radioBtnMan.Text = "Man";
            this.radioBtnMan.UseVisualStyleBackColor = true;
            this.radioBtnMan.CheckedChanged += new System.EventHandler(this.radioBtnMan_CheckedChanged);
            // 
            // pass
            // 
            this.pass.Location = new System.Drawing.Point(20, 195);
            this.pass.Multiline = true;
            this.pass.Name = "pass";
            this.pass.PlaceholderText = "password";
            this.pass.Size = new System.Drawing.Size(166, 39);
            this.pass.TabIndex = 3;
            this.pass.TextChanged += new System.EventHandler(this.pass_TextChanged);
            // 
            // phoneNum
            // 
            this.phoneNum.BackColor = System.Drawing.Color.Aquamarine;
            this.phoneNum.Location = new System.Drawing.Point(20, 150);
            this.phoneNum.Multiline = true;
            this.phoneNum.Name = "phoneNum";
            this.phoneNum.PlaceholderText = "phone";
            this.phoneNum.Size = new System.Drawing.Size(166, 39);
            this.phoneNum.TabIndex = 2;
            this.phoneNum.TextChanged += new System.EventHandler(this.phoneNum_TextChanged);
            // 
            // email
            // 
            this.email.Location = new System.Drawing.Point(20, 99);
            this.email.Multiline = true;
            this.email.Name = "email";
            this.email.PlaceholderText = "email adress";
            this.email.Size = new System.Drawing.Size(166, 39);
            this.email.TabIndex = 1;
            this.email.TextChanged += new System.EventHandler(this.email_TextChanged);
            // 
            // username
            // 
            this.username.BackColor = System.Drawing.Color.Aquamarine;
            this.username.Location = new System.Drawing.Point(20, 39);
            this.username.Multiline = true;
            this.username.Name = "username";
            this.username.PlaceholderText = "Full Name";
            this.username.Size = new System.Drawing.Size(166, 39);
            this.username.TabIndex = 0;
            this.username.TextChanged += new System.EventHandler(this.username_TextChanged);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Font = new System.Drawing.Font("Sitka Display", 18F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point);
            this.label1.Location = new System.Drawing.Point(223, 9);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(251, 43);
            this.label1.TabIndex = 4;
            this.label1.Text = "Registration Form";
            this.label1.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // panel2
            // 
            this.panel2.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.panel2.BackColor = System.Drawing.SystemColors.Highlight;
            this.panel2.ForeColor = System.Drawing.SystemColors.Highlight;
            this.panel2.Location = new System.Drawing.Point(625, 0);
            this.panel2.Name = "panel2";
            this.panel2.Size = new System.Drawing.Size(408, 548);
            this.panel2.TabIndex = 2;
            // 
            // RegisterForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 20F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1030, 531);
            this.Controls.Add(this.panel2);
            this.Controls.Add(this.panel1);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "RegisterForm";
            this.Text = "RegisterForm";
            this.Load += new System.EventHandler(this.RegisterForm_Load);
            this.panel1.ResumeLayout(false);
            this.panel1.PerformLayout();
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.PictureBox pictureBox1;
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.TextBox pass;
        private System.Windows.Forms.TextBox phoneNum;
        private System.Windows.Forms.TextBox email;
        private System.Windows.Forms.TextBox username;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Button crtAccount;
        private System.Windows.Forms.ComboBox cmbCountry;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.RadioButton radioBtnWoman;
        private System.Windows.Forms.RadioButton radioBtnMan;
        private System.Windows.Forms.TextBox age;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.TextBox EGN;
        private System.Windows.Forms.TextBox adress;
        private System.Windows.Forms.TextBox streetId;
        private System.Windows.Forms.TextBox city;
        private System.Windows.Forms.Panel panel2;
        private System.Windows.Forms.Button button1;
    }
}