namespace Energo
{
    partial class MainForm
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

        #region Код, автоматически созданный конструктором форм Windows

        private void InitializeComponent()
        {
            this.dataGridViewClients = new System.Windows.Forms.DataGridView();
            this.dataGridViewAccounts = new System.Windows.Forms.DataGridView();
            this.buttonTransfer = new System.Windows.Forms.Button();
            this.buttonExport = new System.Windows.Forms.Button();
            this.buttonImport = new System.Windows.Forms.Button();
            this.dataGridViewAccountHistory = new System.Windows.Forms.DataGridView();
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridViewClients)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridViewAccounts)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridViewAccountHistory)).BeginInit();
            this.SuspendLayout();
            // 
            // dataGridViewClients
            // 
            this.dataGridViewClients.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dataGridViewClients.Location = new System.Drawing.Point(38, 44);
            this.dataGridViewClients.Name = "dataGridViewClients";
            this.dataGridViewClients.Size = new System.Drawing.Size(240, 150);
            this.dataGridViewClients.TabIndex = 0;
            this.dataGridViewClients.CellClick += new System.Windows.Forms.DataGridViewCellEventHandler(this.dataGridViewClients_CellClick);
            // 
            // dataGridViewAccounts
            // 
            this.dataGridViewAccounts.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dataGridViewAccounts.Location = new System.Drawing.Point(434, 44);
            this.dataGridViewAccounts.Name = "dataGridViewAccounts";
            this.dataGridViewAccounts.Size = new System.Drawing.Size(240, 150);
            this.dataGridViewAccounts.TabIndex = 1;
            this.dataGridViewAccounts.CellClick += new System.Windows.Forms.DataGridViewCellEventHandler(this.dataGridViewAccounts_CellClick);
            // 
            // buttonTransfer
            // 
            this.buttonTransfer.Location = new System.Drawing.Point(38, 226);
            this.buttonTransfer.Name = "buttonTransfer";
            this.buttonTransfer.Size = new System.Drawing.Size(240, 23);
            this.buttonTransfer.TabIndex = 2;
            this.buttonTransfer.Text = "Форма выполнения транзакции";
            this.buttonTransfer.UseVisualStyleBackColor = true;
            this.buttonTransfer.Click += new System.EventHandler(this.buttonTransfer_Click);
            // 
            // buttonExport
            // 
            this.buttonExport.Location = new System.Drawing.Point(38, 255);
            this.buttonExport.Name = "buttonExport";
            this.buttonExport.Size = new System.Drawing.Size(240, 23);
            this.buttonExport.TabIndex = 3;
            this.buttonExport.Text = "Экспорт данных в XML";
            this.buttonExport.UseVisualStyleBackColor = true;
            this.buttonExport.Click += new System.EventHandler(this.buttonExport_Click);
            // 
            // buttonImport
            // 
            this.buttonImport.Location = new System.Drawing.Point(38, 284);
            this.buttonImport.Name = "buttonImport";
            this.buttonImport.Size = new System.Drawing.Size(240, 23);
            this.buttonImport.TabIndex = 4;
            this.buttonImport.Text = "Импорт данных из XML";
            this.buttonImport.UseVisualStyleBackColor = true;
            this.buttonImport.Click += new System.EventHandler(this.buttonImport_Click);
            // 
            // dataGridViewAccountHistory
            // 
            this.dataGridViewAccountHistory.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dataGridViewAccountHistory.Location = new System.Drawing.Point(434, 226);
            this.dataGridViewAccountHistory.Name = "dataGridViewAccountHistory";
            this.dataGridViewAccountHistory.Size = new System.Drawing.Size(240, 150);
            this.dataGridViewAccountHistory.TabIndex = 5;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(35, 28);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(54, 13);
            this.label1.TabIndex = 6;
            this.label1.Text = "Клиенты:";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(431, 28);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(39, 13);
            this.label2.TabIndex = 7;
            this.label2.Text = "Счета:";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(431, 210);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(104, 13);
            this.label3.TabIndex = 8;
            this.label3.Text = "История операций:";
            // 
            // MainForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(800, 450);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.dataGridViewAccountHistory);
            this.Controls.Add(this.buttonImport);
            this.Controls.Add(this.buttonExport);
            this.Controls.Add(this.buttonTransfer);
            this.Controls.Add(this.dataGridViewAccounts);
            this.Controls.Add(this.dataGridViewClients);
            this.Name = "MainForm";
            this.Text = "Списки клиентов, счетов, истории операций";
            ((System.ComponentModel.ISupportInitialize)(this.dataGridViewClients)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridViewAccounts)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridViewAccountHistory)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.DataGridView dataGridViewClients;
        private System.Windows.Forms.DataGridView dataGridViewAccounts;
        private System.Windows.Forms.Button buttonTransfer;
        private System.Windows.Forms.Button buttonExport;
        private System.Windows.Forms.Button buttonImport;
        private System.Windows.Forms.DataGridView dataGridViewAccountHistory;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label3;
    }
}
