namespace LifePath
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
            this.btnGenerate = new System.Windows.Forms.Button();
            this.btnFirstName = new System.Windows.Forms.Button();
            this.label1 = new System.Windows.Forms.Label();
            this.tbxFirstName = new System.Windows.Forms.TextBox();
            this.tbxLastName = new System.Windows.Forms.TextBox();
            this.label2 = new System.Windows.Forms.Label();
            this.btnLastName = new System.Windows.Forms.Button();
            this.panel1 = new System.Windows.Forms.Panel();
            this.btnSave = new System.Windows.Forms.Button();
            this.label4 = new System.Windows.Forms.Label();
            this.btnGenName = new System.Windows.Forms.Button();
            this.panel2 = new System.Windows.Forms.Panel();
            this.lbxEnemies = new System.Windows.Forms.ListBox();
            this.lbxFriends = new System.Windows.Forms.ListBox();
            this.lbxSiblings = new System.Windows.Forms.ListBox();
            this.label9 = new System.Windows.Forms.Label();
            this.tbxDisplaySelected = new System.Windows.Forms.TextBox();
            this.lblRelationshipStatus = new System.Windows.Forms.Label();
            this.lblRelInfo = new System.Windows.Forms.Label();
            this.lblLoverName = new System.Windows.Forms.Label();
            this.lblRelationshipReroll = new System.Windows.Forms.Label();
            this.lblEnemiesReroll = new System.Windows.Forms.Label();
            this.lblFriendsReroll = new System.Windows.Forms.Label();
            this.lblSiblingReroll = new System.Windows.Forms.Label();
            this.lblFamilyStatus = new System.Windows.Forms.Label();
            this.lblFamilyReroll = new System.Windows.Forms.Label();
            this.lblParentStatus = new System.Windows.Forms.Label();
            this.lblParent2 = new System.Windows.Forms.Label();
            this.lblParent1 = new System.Windows.Forms.Label();
            this.lblParentReroll = new System.Windows.Forms.Label();
            this.panel1.SuspendLayout();
            this.panel2.SuspendLayout();
            this.SuspendLayout();
            // 
            // btnGenerate
            // 
            this.btnGenerate.Location = new System.Drawing.Point(11, 6);
            this.btnGenerate.Name = "btnGenerate";
            this.btnGenerate.Size = new System.Drawing.Size(123, 23);
            this.btnGenerate.TabIndex = 0;
            this.btnGenerate.Text = "Generate Lifepath";
            this.btnGenerate.UseVisualStyleBackColor = true;
            this.btnGenerate.Click += new System.EventHandler(this.btnGenerate_Click);
            // 
            // btnFirstName
            // 
            this.btnFirstName.Location = new System.Drawing.Point(11, 51);
            this.btnFirstName.Name = "btnFirstName";
            this.btnFirstName.Size = new System.Drawing.Size(75, 23);
            this.btnFirstName.TabIndex = 1;
            this.btnFirstName.Text = "First Only";
            this.btnFirstName.UseVisualStyleBackColor = true;
            this.btnFirstName.Click += new System.EventHandler(this.btnFirstName_Click);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(12, 9);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(55, 13);
            this.label1.TabIndex = 2;
            this.label1.Text = "First name";
            // 
            // tbxFirstName
            // 
            this.tbxFirstName.Location = new System.Drawing.Point(11, 25);
            this.tbxFirstName.Name = "tbxFirstName";
            this.tbxFirstName.Size = new System.Drawing.Size(179, 20);
            this.tbxFirstName.TabIndex = 3;
            // 
            // tbxLastName
            // 
            this.tbxLastName.Location = new System.Drawing.Point(196, 25);
            this.tbxLastName.Name = "tbxLastName";
            this.tbxLastName.Size = new System.Drawing.Size(179, 20);
            this.tbxLastName.TabIndex = 6;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(197, 9);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(56, 13);
            this.label2.TabIndex = 5;
            this.label2.Text = "Last name";
            // 
            // btnLastName
            // 
            this.btnLastName.Location = new System.Drawing.Point(196, 51);
            this.btnLastName.Name = "btnLastName";
            this.btnLastName.Size = new System.Drawing.Size(75, 23);
            this.btnLastName.TabIndex = 4;
            this.btnLastName.Text = "Last Only";
            this.btnLastName.UseVisualStyleBackColor = true;
            this.btnLastName.Click += new System.EventHandler(this.btnLastName_Click);
            // 
            // panel1
            // 
            this.panel1.Controls.Add(this.btnSave);
            this.panel1.Controls.Add(this.label4);
            this.panel1.Controls.Add(this.btnGenName);
            this.panel1.Controls.Add(this.label1);
            this.panel1.Controls.Add(this.tbxLastName);
            this.panel1.Controls.Add(this.btnFirstName);
            this.panel1.Controls.Add(this.label2);
            this.panel1.Controls.Add(this.tbxFirstName);
            this.panel1.Controls.Add(this.btnLastName);
            this.panel1.Dock = System.Windows.Forms.DockStyle.Top;
            this.panel1.Location = new System.Drawing.Point(0, 0);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(758, 96);
            this.panel1.TabIndex = 7;
            // 
            // btnSave
            // 
            this.btnSave.Location = new System.Drawing.Point(381, 51);
            this.btnSave.Name = "btnSave";
            this.btnSave.Size = new System.Drawing.Size(114, 23);
            this.btnSave.TabIndex = 9;
            this.btnSave.Text = "Save to File";
            this.btnSave.UseVisualStyleBackColor = true;
            this.btnSave.Click += new System.EventHandler(this.btnSave_Click);
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(12, 77);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(247, 13);
            this.label4.TabIndex = 8;
            this.label4.Text = "LifePath table data copyright © R.Talsorian Games";
            // 
            // btnGenName
            // 
            this.btnGenName.Location = new System.Drawing.Point(381, 23);
            this.btnGenName.Name = "btnGenName";
            this.btnGenName.Size = new System.Drawing.Size(114, 23);
            this.btnGenName.TabIndex = 7;
            this.btnGenName.Text = "Generate Name";
            this.btnGenName.UseVisualStyleBackColor = true;
            this.btnGenName.Click += new System.EventHandler(this.btnGenName_Click);
            // 
            // panel2
            // 
            this.panel2.Controls.Add(this.lbxEnemies);
            this.panel2.Controls.Add(this.lbxFriends);
            this.panel2.Controls.Add(this.lbxSiblings);
            this.panel2.Controls.Add(this.label9);
            this.panel2.Controls.Add(this.tbxDisplaySelected);
            this.panel2.Controls.Add(this.lblRelationshipStatus);
            this.panel2.Controls.Add(this.lblRelInfo);
            this.panel2.Controls.Add(this.lblLoverName);
            this.panel2.Controls.Add(this.lblRelationshipReroll);
            this.panel2.Controls.Add(this.lblEnemiesReroll);
            this.panel2.Controls.Add(this.lblFriendsReroll);
            this.panel2.Controls.Add(this.lblSiblingReroll);
            this.panel2.Controls.Add(this.lblFamilyStatus);
            this.panel2.Controls.Add(this.lblFamilyReroll);
            this.panel2.Controls.Add(this.lblParentStatus);
            this.panel2.Controls.Add(this.lblParent2);
            this.panel2.Controls.Add(this.lblParent1);
            this.panel2.Controls.Add(this.lblParentReroll);
            this.panel2.Controls.Add(this.btnGenerate);
            this.panel2.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panel2.Location = new System.Drawing.Point(0, 96);
            this.panel2.Name = "panel2";
            this.panel2.Size = new System.Drawing.Size(758, 498);
            this.panel2.TabIndex = 8;
            // 
            // lbxEnemies
            // 
            this.lbxEnemies.FormattingEnabled = true;
            this.lbxEnemies.Location = new System.Drawing.Point(14, 412);
            this.lbxEnemies.Name = "lbxEnemies";
            this.lbxEnemies.Size = new System.Drawing.Size(257, 69);
            this.lbxEnemies.TabIndex = 21;
            this.lbxEnemies.SelectedIndexChanged += new System.EventHandler(this.lbxSiblings_SelectedIndexChanged);
            // 
            // lbxFriends
            // 
            this.lbxFriends.FormattingEnabled = true;
            this.lbxFriends.Location = new System.Drawing.Point(14, 319);
            this.lbxFriends.Name = "lbxFriends";
            this.lbxFriends.Size = new System.Drawing.Size(257, 69);
            this.lbxFriends.TabIndex = 20;
            this.lbxFriends.SelectedIndexChanged += new System.EventHandler(this.lbxSiblings_SelectedIndexChanged);
            // 
            // lbxSiblings
            // 
            this.lbxSiblings.FormattingEnabled = true;
            this.lbxSiblings.Location = new System.Drawing.Point(14, 127);
            this.lbxSiblings.Name = "lbxSiblings";
            this.lbxSiblings.Size = new System.Drawing.Size(257, 173);
            this.lbxSiblings.TabIndex = 19;
            this.lbxSiblings.SelectedIndexChanged += new System.EventHandler(this.lbxSiblings_SelectedIndexChanged);
            // 
            // label9
            // 
            this.label9.AutoSize = true;
            this.label9.Location = new System.Drawing.Point(274, 112);
            this.label9.Name = "label9";
            this.label9.Size = new System.Drawing.Size(52, 13);
            this.label9.TabIndex = 18;
            this.label9.Text = "Selected:";
            // 
            // tbxDisplaySelected
            // 
            this.tbxDisplaySelected.Font = new System.Drawing.Font("Courier New", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.tbxDisplaySelected.Location = new System.Drawing.Point(277, 128);
            this.tbxDisplaySelected.Multiline = true;
            this.tbxDisplaySelected.Name = "tbxDisplaySelected";
            this.tbxDisplaySelected.Size = new System.Drawing.Size(469, 172);
            this.tbxDisplaySelected.TabIndex = 17;
            // 
            // lblRelationshipStatus
            // 
            this.lblRelationshipStatus.AutoSize = true;
            this.lblRelationshipStatus.Location = new System.Drawing.Point(384, 319);
            this.lblRelationshipStatus.Name = "lblRelationshipStatus";
            this.lblRelationshipStatus.Size = new System.Drawing.Size(10, 13);
            this.lblRelationshipStatus.TabIndex = 16;
            this.lblRelationshipStatus.Text = " ";
            // 
            // lblRelInfo
            // 
            this.lblRelInfo.AutoSize = true;
            this.lblRelInfo.Location = new System.Drawing.Point(290, 353);
            this.lblRelInfo.Name = "lblRelInfo";
            this.lblRelInfo.Size = new System.Drawing.Size(10, 13);
            this.lblRelInfo.TabIndex = 15;
            this.lblRelInfo.Text = " ";
            // 
            // lblLoverName
            // 
            this.lblLoverName.AutoSize = true;
            this.lblLoverName.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblLoverName.Location = new System.Drawing.Point(290, 336);
            this.lblLoverName.Name = "lblLoverName";
            this.lblLoverName.Size = new System.Drawing.Size(11, 13);
            this.lblLoverName.TabIndex = 14;
            this.lblLoverName.Text = " ";
            this.lblLoverName.Click += new System.EventHandler(this.lblItemReroll_Click);
            // 
            // lblRelationshipReroll
            // 
            this.lblRelationshipReroll.AutoSize = true;
            this.lblRelationshipReroll.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblRelationshipReroll.Location = new System.Drawing.Point(277, 319);
            this.lblRelationshipReroll.Name = "lblRelationshipReroll";
            this.lblRelationshipReroll.Size = new System.Drawing.Size(121, 13);
            this.lblRelationshipReroll.TabIndex = 13;
            this.lblRelationshipReroll.Text = "Relationship Status:";
            this.lblRelationshipReroll.Click += new System.EventHandler(this.lblItemReroll_Click);
            // 
            // lblEnemiesReroll
            // 
            this.lblEnemiesReroll.AutoSize = true;
            this.lblEnemiesReroll.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblEnemiesReroll.Location = new System.Drawing.Point(12, 396);
            this.lblEnemiesReroll.Name = "lblEnemiesReroll";
            this.lblEnemiesReroll.Size = new System.Drawing.Size(58, 13);
            this.lblEnemiesReroll.TabIndex = 11;
            this.lblEnemiesReroll.Text = "Enemies:";
            this.lblEnemiesReroll.Click += new System.EventHandler(this.lblItemReroll_Click);
            // 
            // lblFriendsReroll
            // 
            this.lblFriendsReroll.AutoSize = true;
            this.lblFriendsReroll.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblFriendsReroll.Location = new System.Drawing.Point(12, 303);
            this.lblFriendsReroll.Name = "lblFriendsReroll";
            this.lblFriendsReroll.Size = new System.Drawing.Size(52, 13);
            this.lblFriendsReroll.TabIndex = 9;
            this.lblFriendsReroll.Text = "Friends:";
            this.lblFriendsReroll.Click += new System.EventHandler(this.lblItemReroll_Click);
            // 
            // lblSiblingReroll
            // 
            this.lblSiblingReroll.AutoSize = true;
            this.lblSiblingReroll.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblSiblingReroll.Location = new System.Drawing.Point(12, 111);
            this.lblSiblingReroll.Name = "lblSiblingReroll";
            this.lblSiblingReroll.Size = new System.Drawing.Size(55, 13);
            this.lblSiblingReroll.TabIndex = 7;
            this.lblSiblingReroll.Text = "Siblings:";
            this.lblSiblingReroll.Click += new System.EventHandler(this.lblItemReroll_Click);
            // 
            // lblFamilyStatus
            // 
            this.lblFamilyStatus.AutoSize = true;
            this.lblFamilyStatus.Location = new System.Drawing.Point(99, 93);
            this.lblFamilyStatus.Name = "lblFamilyStatus";
            this.lblFamilyStatus.Size = new System.Drawing.Size(10, 13);
            this.lblFamilyStatus.TabIndex = 6;
            this.lblFamilyStatus.Text = " ";
            // 
            // lblFamilyReroll
            // 
            this.lblFamilyReroll.AutoSize = true;
            this.lblFamilyReroll.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblFamilyReroll.Location = new System.Drawing.Point(12, 93);
            this.lblFamilyReroll.Name = "lblFamilyReroll";
            this.lblFamilyReroll.Size = new System.Drawing.Size(86, 13);
            this.lblFamilyReroll.TabIndex = 5;
            this.lblFamilyReroll.Text = "Family Status:";
            this.lblFamilyReroll.Click += new System.EventHandler(this.lblItemReroll_Click);
            // 
            // lblParentStatus
            // 
            this.lblParentStatus.AutoSize = true;
            this.lblParentStatus.Location = new System.Drawing.Point(99, 36);
            this.lblParentStatus.Name = "lblParentStatus";
            this.lblParentStatus.Size = new System.Drawing.Size(10, 13);
            this.lblParentStatus.TabIndex = 4;
            this.lblParentStatus.Text = " ";
            // 
            // lblParent2
            // 
            this.lblParent2.AutoSize = true;
            this.lblParent2.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblParent2.Location = new System.Drawing.Point(24, 70);
            this.lblParent2.Name = "lblParent2";
            this.lblParent2.Size = new System.Drawing.Size(11, 13);
            this.lblParent2.TabIndex = 3;
            this.lblParent2.Text = " ";
            this.lblParent2.Click += new System.EventHandler(this.lblItemReroll_Click);
            // 
            // lblParent1
            // 
            this.lblParent1.AutoSize = true;
            this.lblParent1.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblParent1.Location = new System.Drawing.Point(24, 53);
            this.lblParent1.Name = "lblParent1";
            this.lblParent1.Size = new System.Drawing.Size(11, 13);
            this.lblParent1.TabIndex = 2;
            this.lblParent1.Text = " ";
            this.lblParent1.Click += new System.EventHandler(this.lblItemReroll_Click);
            // 
            // lblParentReroll
            // 
            this.lblParentReroll.AutoSize = true;
            this.lblParentReroll.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblParentReroll.Location = new System.Drawing.Point(11, 36);
            this.lblParentReroll.Name = "lblParentReroll";
            this.lblParentReroll.Size = new System.Drawing.Size(88, 13);
            this.lblParentReroll.TabIndex = 1;
            this.lblParentReroll.Text = "Parent Status:";
            this.lblParentReroll.Click += new System.EventHandler(this.lblItemReroll_Click);
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(758, 594);
            this.Controls.Add(this.panel2);
            this.Controls.Add(this.panel1);
            this.Name = "Form1";
            this.Text = "LifePath Generator";
            this.Load += new System.EventHandler(this.Form1_Load);
            this.panel1.ResumeLayout(false);
            this.panel1.PerformLayout();
            this.panel2.ResumeLayout(false);
            this.panel2.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Button btnGenerate;
        private System.Windows.Forms.Button btnFirstName;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TextBox tbxFirstName;
        private System.Windows.Forms.TextBox tbxLastName;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Button btnLastName;
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.Panel panel2;
        private System.Windows.Forms.Button btnGenName;
        private System.Windows.Forms.Label lblParentReroll;
        private System.Windows.Forms.Button btnSave;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Label label9;
        private System.Windows.Forms.TextBox tbxDisplaySelected;
        private System.Windows.Forms.Label lblRelationshipStatus;
        private System.Windows.Forms.Label lblRelInfo;
        private System.Windows.Forms.Label lblLoverName;
        private System.Windows.Forms.Label lblRelationshipReroll;
        private System.Windows.Forms.Label lblEnemiesReroll;
        private System.Windows.Forms.Label lblFriendsReroll;
        private System.Windows.Forms.Label lblSiblingReroll;
        private System.Windows.Forms.Label lblFamilyStatus;
        private System.Windows.Forms.Label lblFamilyReroll;
        private System.Windows.Forms.Label lblParentStatus;
        private System.Windows.Forms.Label lblParent2;
        private System.Windows.Forms.Label lblParent1;
        private System.Windows.Forms.ListBox lbxEnemies;
        private System.Windows.Forms.ListBox lbxFriends;
        private System.Windows.Forms.ListBox lbxSiblings;
    }
}

