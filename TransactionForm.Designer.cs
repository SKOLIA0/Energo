namespace Energo
{
    partial class TransactionForm
    {
        private System.ComponentModel.IContainer components = null;

        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        private void InitializeComponent()
        {
            this.comboBoxSenderClient = new System.Windows.Forms.ComboBox();
            this.comboBoxSenderAccount = new System.Windows.Forms.ComboBox();
            this.comboBoxRecipientClient = new System.Windows.Forms.ComboBox();
            this.comboBoxRecipientAccount = new System.Windows.Forms.ComboBox();
            this.textBoxAmount = new System.Windows.Forms.TextBox();
            this.buttonSubmit = new System.Windows.Forms.Button();
            this.labelSenderBalance = new System.Windows.Forms.Label();
            this.labelRecipientBalance = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.label5 = new System.Windows.Forms.Label();
            this.label6 = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // comboBoxSenderClient
            // 
            this.comboBoxSenderClient.FormattingEnabled = true;
            this.comboBoxSenderClient.Location = new System.Drawing.Point(26, 68);
            this.comboBoxSenderClient.Name = "comboBoxSenderClient";
            this.comboBoxSenderClient.Size = new System.Drawing.Size(121, 21);
            this.comboBoxSenderClient.TabIndex = 0;
            this.comboBoxSenderClient.SelectedIndexChanged += new System.EventHandler(this.comboBoxSenderClient_SelectedIndexChanged);
            // 
            // comboBoxSenderAccount
            // 
            this.comboBoxSenderAccount.FormattingEnabled = true;
            this.comboBoxSenderAccount.Location = new System.Drawing.Point(196, 68);
            this.comboBoxSenderAccount.Name = "comboBoxSenderAccount";
            this.comboBoxSenderAccount.Size = new System.Drawing.Size(121, 21);
            this.comboBoxSenderAccount.TabIndex = 1;
            this.comboBoxSenderAccount.SelectedIndexChanged += new System.EventHandler(this.comboBoxSenderAccount_SelectedIndexChanged);
            // 
            // comboBoxRecipientClient
            // 
            this.comboBoxRecipientClient.FormattingEnabled = true;
            this.comboBoxRecipientClient.Location = new System.Drawing.Point(445, 68);
            this.comboBoxRecipientClient.Name = "comboBoxRecipientClient";
            this.comboBoxRecipientClient.Size = new System.Drawing.Size(121, 21);
            this.comboBoxRecipientClient.TabIndex = 2;
            this.comboBoxRecipientClient.SelectedIndexChanged += new System.EventHandler(this.comboBoxRecipientClient_SelectedIndexChanged);
            // 
            // comboBoxRecipientAccount
            // 
            this.comboBoxRecipientAccount.FormattingEnabled = true;
            this.comboBoxRecipientAccount.Location = new System.Drawing.Point(618, 68);
            this.comboBoxRecipientAccount.Name = "comboBoxRecipientAccount";
            this.comboBoxRecipientAccount.Size = new System.Drawing.Size(121, 21);
            this.comboBoxRecipientAccount.TabIndex = 3;
            this.comboBoxRecipientAccount.SelectedIndexChanged += new System.EventHandler(this.comboBoxRecipientAccount_SelectedIndexChanged);
            // 
            // textBoxAmount
            // 
            this.textBoxAmount.Location = new System.Drawing.Point(316, 141);
            this.textBoxAmount.Name = "textBoxAmount";
            this.textBoxAmount.Size = new System.Drawing.Size(128, 20);
            this.textBoxAmount.TabIndex = 4;
            // 
            // buttonSubmit
            // 
            this.buttonSubmit.Location = new System.Drawing.Point(316, 215);
            this.buttonSubmit.Name = "buttonSubmit";
            this.buttonSubmit.Size = new System.Drawing.Size(128, 23);
            this.buttonSubmit.TabIndex = 5;
            this.buttonSubmit.Text = "Подтвердить перевод";
            this.buttonSubmit.UseVisualStyleBackColor = true;
            this.buttonSubmit.Click += new System.EventHandler(this.buttonSubmit_Click);
            // 
            // labelSenderBalance
            // 
            this.labelSenderBalance.AutoSize = true;
            this.labelSenderBalance.Location = new System.Drawing.Point(152, 141);
            this.labelSenderBalance.Name = "labelSenderBalance";
            this.labelSenderBalance.Size = new System.Drawing.Size(47, 13);
            this.labelSenderBalance.TabIndex = 6;
            this.labelSenderBalance.Text = "Баланс:";
            // 
            // labelRecipientBalance
            // 
            this.labelRecipientBalance.AutoSize = true;
            this.labelRecipientBalance.Location = new System.Drawing.Point(577, 141);
            this.labelRecipientBalance.Name = "labelRecipientBalance";
            this.labelRecipientBalance.Size = new System.Drawing.Size(47, 13);
            this.labelRecipientBalance.TabIndex = 7;
            this.labelRecipientBalance.Text = "Баланс:";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(23, 52);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(73, 13);
            this.label1.TabIndex = 8;
            this.label1.Text = "Отправитель";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(193, 52);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(98, 13);
            this.label2.TabIndex = 9;
            this.label2.Text = "Счет отправления";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(442, 52);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(66, 13);
            this.label3.TabIndex = 10;
            this.label3.Text = "Получатель";
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(615, 52);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(90, 13);
            this.label4.TabIndex = 11;
            this.label4.Text = "Счет получателя";
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(152, 22);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(43, 13);
            this.label5.TabIndex = 12;
            this.label5.Text = "Кредит";
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(577, 22);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(39, 13);
            this.label6.TabIndex = 13;
            this.label6.Text = "Дебет";
            // 
            // TransactionForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(800, 450);
            this.Controls.Add(this.label6);
            this.Controls.Add(this.label5);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.labelRecipientBalance);
            this.Controls.Add(this.labelSenderBalance);
            this.Controls.Add(this.buttonSubmit);
            this.Controls.Add(this.textBoxAmount);
            this.Controls.Add(this.comboBoxRecipientAccount);
            this.Controls.Add(this.comboBoxRecipientClient);
            this.Controls.Add(this.comboBoxSenderAccount);
            this.Controls.Add(this.comboBoxSenderClient);
            this.Name = "TransactionForm";
            this.Text = "Перевод (проводка между счетами клиентов)";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.ComboBox comboBoxSenderClient;
        private System.Windows.Forms.ComboBox comboBoxSenderAccount;
        private System.Windows.Forms.ComboBox comboBoxRecipientClient;
        private System.Windows.Forms.ComboBox comboBoxRecipientAccount;
        private System.Windows.Forms.TextBox textBoxAmount;
        private System.Windows.Forms.Button buttonSubmit;
        private System.Windows.Forms.Label labelSenderBalance;
        private System.Windows.Forms.Label labelRecipientBalance;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.Label label6;
    }
}
