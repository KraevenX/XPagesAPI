using System.Windows.Forms;

namespace TestDLL
{
    partial class Form2
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
            this.btnConnect = new System.Windows.Forms.Button();
            this.Label1 = new System.Windows.Forms.Label();
            this.Label2 = new System.Windows.Forms.Label();
            this.txtPass = new System.Windows.Forms.MaskedTextBox();
            this.txtUser = new System.Windows.Forms.TextBox();
            this.Label3 = new System.Windows.Forms.Label();
            this.txtServer = new System.Windows.Forms.TextBox();
            this.Label4 = new System.Windows.Forms.Label();
            this.txtWebServiceURL = new System.Windows.Forms.TextBox();
            this.label5 = new System.Windows.Forms.Label();
            this.label6 = new System.Windows.Forms.Label();
            this.label7 = new System.Windows.Forms.Label();
            this.txtDbServer = new System.Windows.Forms.TextBox();
            this.txtDbFilePath = new System.Windows.Forms.TextBox();
            this.txtDbRepId = new System.Windows.Forms.TextBox();
            this.label8 = new System.Windows.Forms.Label();
            this.txtUnid = new System.Windows.Forms.TextBox();
            this.label9 = new System.Windows.Forms.Label();
            this.txtFields = new System.Windows.Forms.TextBox();
            this.txtExportFile = new System.Windows.Forms.TextBox();
            this.label10 = new System.Windows.Forms.Label();
            this.label11 = new System.Windows.Forms.Label();
            this.label12 = new System.Windows.Forms.Label();
            this.txtSearchField = new System.Windows.Forms.TextBox();
            this.txtSearchValue = new System.Windows.Forms.TextBox();
            this.txtSearchFormula = new System.Windows.Forms.TextBox();
            this.label13 = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // btnConnect
            // 
            this.btnConnect.Location = new System.Drawing.Point(15, 366);
            this.btnConnect.Name = "btnConnect";
            this.btnConnect.Size = new System.Drawing.Size(75, 23);
            this.btnConnect.TabIndex = 0;
            this.btnConnect.Text = "Connect";
            this.btnConnect.UseVisualStyleBackColor = true;
            this.btnConnect.Click += new System.EventHandler(this.btnConnect_Click);
            // 
            // Label1
            // 
            this.Label1.AutoSize = true;
            this.Label1.Location = new System.Drawing.Point(12, 13);
            this.Label1.Name = "Label1";
            this.Label1.Size = new System.Drawing.Size(57, 13);
            this.Label1.TabIndex = 1;
            this.Label1.Text = "UserName";
            // 
            // Label2
            // 
            this.Label2.AutoSize = true;
            this.Label2.Location = new System.Drawing.Point(12, 38);
            this.Label2.Name = "Label2";
            this.Label2.Size = new System.Drawing.Size(53, 13);
            this.Label2.TabIndex = 2;
            this.Label2.Text = "Password";
            // 
            // txtPass
            // 
            this.txtPass.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.txtPass.ForeColor = System.Drawing.Color.LightSlateGray;
            this.txtPass.Location = new System.Drawing.Point(138, 38);
            this.txtPass.Name = "txtPass";
            this.txtPass.PasswordChar = '■';
            this.txtPass.Size = new System.Drawing.Size(517, 20);
            this.txtPass.TabIndex = 1;
            this.txtPass.Text = "Je022018";
            // 
            // txtUser
            // 
            this.txtUser.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.txtUser.Location = new System.Drawing.Point(138, 13);
            this.txtUser.Name = "txtUser";
            this.txtUser.Size = new System.Drawing.Size(517, 20);
            this.txtUser.TabIndex = 0;
            this.txtUser.Text = "Kim Acket";
            // 
            // Label3
            // 
            this.Label3.AutoSize = true;
            this.Label3.Location = new System.Drawing.Point(12, 65);
            this.Label3.Name = "Label3";
            this.Label3.Size = new System.Drawing.Size(63, 13);
            this.Label3.TabIndex = 5;
            this.Label3.Text = "Server URL";
            // 
            // txtServer
            // 
            this.txtServer.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.txtServer.Location = new System.Drawing.Point(138, 65);
            this.txtServer.Name = "txtServer";
            this.txtServer.Size = new System.Drawing.Size(517, 20);
            this.txtServer.TabIndex = 2;
            this.txtServer.Text = "http://antln-test.europe.jacobs.com";
            // 
            // Label4
            // 
            this.Label4.AutoSize = true;
            this.Label4.Location = new System.Drawing.Point(12, 92);
            this.Label4.Name = "Label4";
            this.Label4.Size = new System.Drawing.Size(69, 13);
            this.Label4.TabIndex = 7;
            this.Label4.Text = "XPages URL";
            // 
            // txtWebServiceURL
            // 
            this.txtWebServiceURL.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.txtWebServiceURL.Location = new System.Drawing.Point(138, 92);
            this.txtWebServiceURL.Name = "txtWebServiceURL";
            this.txtWebServiceURL.Size = new System.Drawing.Size(517, 20);
            this.txtWebServiceURL.TabIndex = 3;
            this.txtWebServiceURL.Text = "http://antln-test.europe.jacobs.com/projects/jpix/XPageDev1300_XP.nsf";
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(12, 125);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(87, 13);
            this.label5.TabIndex = 8;
            this.label5.Text = "Database Server";
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(12, 153);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(94, 13);
            this.label6.TabIndex = 9;
            this.label6.Text = "Database FilePath";
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.Location = new System.Drawing.Point(12, 179);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(120, 13);
            this.label7.TabIndex = 10;
            this.label7.Text = "Database ReplicationID";
            // 
            // txtDbServer
            // 
            this.txtDbServer.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.txtDbServer.Location = new System.Drawing.Point(138, 122);
            this.txtDbServer.Name = "txtDbServer";
            this.txtDbServer.Size = new System.Drawing.Size(517, 20);
            this.txtDbServer.TabIndex = 11;
            this.txtDbServer.Text = "ANTLN-TEST/ANTWERPEN/JACOBSENGINEERING";
            // 
            // txtDbFilePath
            // 
            this.txtDbFilePath.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.txtDbFilePath.Location = new System.Drawing.Point(138, 150);
            this.txtDbFilePath.Name = "txtDbFilePath";
            this.txtDbFilePath.Size = new System.Drawing.Size(517, 20);
            this.txtDbFilePath.TabIndex = 12;
            this.txtDbFilePath.Text = "projects\\jpi4\\XP2015_jp.nsf";
            // 
            // txtDbRepId
            // 
            this.txtDbRepId.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.txtDbRepId.Location = new System.Drawing.Point(138, 176);
            this.txtDbRepId.Name = "txtDbRepId";
            this.txtDbRepId.Size = new System.Drawing.Size(517, 20);
            this.txtDbRepId.TabIndex = 13;
            // 
            // label8
            // 
            this.label8.AutoSize = true;
            this.label8.Location = new System.Drawing.Point(12, 211);
            this.label8.Name = "label8";
            this.label8.Size = new System.Drawing.Size(109, 13);
            this.label8.TabIndex = 14;
            this.label8.Text = "Document UniveralID";
            // 
            // txtUnid
            // 
            this.txtUnid.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.txtUnid.Location = new System.Drawing.Point(138, 208);
            this.txtUnid.Name = "txtUnid";
            this.txtUnid.Size = new System.Drawing.Size(517, 20);
            this.txtUnid.TabIndex = 15;
            // 
            // label9
            // 
            this.label9.AutoSize = true;
            this.label9.Location = new System.Drawing.Point(13, 317);
            this.label9.Name = "label9";
            this.label9.Size = new System.Drawing.Size(86, 13);
            this.label9.TabIndex = 16;
            this.label9.Text = "Document Fields";
            // 
            // txtFields
            // 
            this.txtFields.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.txtFields.Location = new System.Drawing.Point(138, 317);
            this.txtFields.Name = "txtFields";
            this.txtFields.Size = new System.Drawing.Size(517, 20);
            this.txtFields.TabIndex = 17;
            this.txtFields.Text = "StatusCode;jeNumber;jeProjectNumber;jeDisciplineCode;wfRevisionCode;jeTypeCode;je" +
    "Title1";
            // 
            // txtExportFile
            // 
            this.txtExportFile.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.txtExportFile.Location = new System.Drawing.Point(138, 343);
            this.txtExportFile.Name = "txtExportFile";
            this.txtExportFile.Size = new System.Drawing.Size(517, 20);
            this.txtExportFile.TabIndex = 18;
            this.txtExportFile.Text = "C:\\DocumentFields";
            // 
            // label10
            // 
            this.label10.AutoSize = true;
            this.label10.Location = new System.Drawing.Point(13, 342);
            this.label10.Name = "label10";
            this.label10.Size = new System.Drawing.Size(78, 13);
            this.label10.TabIndex = 19;
            this.label10.Text = "Export FilePath";
            // 
            // label11
            // 
            this.label11.AutoSize = true;
            this.label11.Location = new System.Drawing.Point(12, 237);
            this.label11.Name = "label11";
            this.label11.Size = new System.Drawing.Size(66, 13);
            this.label11.TabIndex = 20;
            this.label11.Text = "Search Field";
            // 
            // label12
            // 
            this.label12.AutoSize = true;
            this.label12.Location = new System.Drawing.Point(12, 264);
            this.label12.Name = "label12";
            this.label12.Size = new System.Drawing.Size(71, 13);
            this.label12.TabIndex = 21;
            this.label12.Text = "Search Value";
            // 
            // txtSearchField
            // 
            this.txtSearchField.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.txtSearchField.Location = new System.Drawing.Point(138, 237);
            this.txtSearchField.Name = "txtSearchField";
            this.txtSearchField.Size = new System.Drawing.Size(517, 20);
            this.txtSearchField.TabIndex = 22;
            // 
            // txtSearchValue
            // 
            this.txtSearchValue.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.txtSearchValue.Location = new System.Drawing.Point(138, 264);
            this.txtSearchValue.Name = "txtSearchValue";
            this.txtSearchValue.Size = new System.Drawing.Size(517, 20);
            this.txtSearchValue.TabIndex = 23;
            // 
            // txtSearchFormula
            // 
            this.txtSearchFormula.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.txtSearchFormula.Location = new System.Drawing.Point(138, 290);
            this.txtSearchFormula.Name = "txtSearchFormula";
            this.txtSearchFormula.Size = new System.Drawing.Size(517, 20);
            this.txtSearchFormula.TabIndex = 25;
            this.txtSearchFormula.Text = " StatusCode != 80 & @IsAvailable(jeSopIndexNumber) &  @Contains(form; \"Doc\")";
            // 
            // label13
            // 
            this.label13.AutoSize = true;
            this.label13.Location = new System.Drawing.Point(12, 290);
            this.label13.Name = "label13";
            this.label13.Size = new System.Drawing.Size(81, 13);
            this.label13.TabIndex = 24;
            this.label13.Text = "Search Formula";
            // 
            // Form2
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(667, 447);
            this.Controls.Add(this.txtSearchFormula);
            this.Controls.Add(this.label13);
            this.Controls.Add(this.txtSearchValue);
            this.Controls.Add(this.txtSearchField);
            this.Controls.Add(this.label12);
            this.Controls.Add(this.label11);
            this.Controls.Add(this.label10);
            this.Controls.Add(this.txtExportFile);
            this.Controls.Add(this.txtFields);
            this.Controls.Add(this.label9);
            this.Controls.Add(this.txtUnid);
            this.Controls.Add(this.label8);
            this.Controls.Add(this.txtDbRepId);
            this.Controls.Add(this.txtDbFilePath);
            this.Controls.Add(this.txtDbServer);
            this.Controls.Add(this.label7);
            this.Controls.Add(this.label6);
            this.Controls.Add(this.label5);
            this.Controls.Add(this.txtWebServiceURL);
            this.Controls.Add(this.Label4);
            this.Controls.Add(this.txtServer);
            this.Controls.Add(this.Label3);
            this.Controls.Add(this.txtUser);
            this.Controls.Add(this.txtPass);
            this.Controls.Add(this.Label2);
            this.Controls.Add(this.Label1);
            this.Controls.Add(this.btnConnect);
            this.Name = "Form2";
            this.Text = "Test Connector";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        internal Button btnConnect;
        internal Label Label1;
        internal Label Label2;
        internal MaskedTextBox txtPass;
        internal TextBox txtUser;
        internal Label Label3;
        internal TextBox txtServer;
        internal Label Label4;
        internal TextBox txtWebServiceURL;
        internal Label label5;
        internal Label label6;
        internal Label label7;
        internal TextBox txtDbServer;
        internal TextBox txtDbFilePath;
        internal TextBox txtDbRepId;
        internal Label label8;
        internal TextBox txtUnid;
        internal Label label9;
        internal TextBox txtFields;
        internal TextBox txtExportFile;
        internal Label label10;
        internal Label label11;
        internal Label label12;
        internal TextBox txtSearchField;
        internal TextBox txtSearchValue;
        internal TextBox txtSearchFormula;
        internal Label label13;
    }

        #endregion
    }
