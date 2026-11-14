namespace LaundryCustomerAndOrderTracking
{
    partial class Dashboard
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
            this.dgvRecentOrders = new System.Windows.Forms.DataGridView();
            this.panel1 = new System.Windows.Forms.Panel();
            this.tableLayoutPanel1 = new System.Windows.Forms.TableLayoutPanel();
            this.btnReports = new System.Windows.Forms.Button();
            this.btnPayment = new System.Windows.Forms.Button();
            this.btnOrderTracking = new System.Windows.Forms.Button();
            this.btnService = new System.Windows.Forms.Button();
            this.btnCustomer = new System.Windows.Forms.Button();
            this.btnButton = new System.Windows.Forms.Button();
            this.panel2 = new System.Windows.Forms.Panel();
            this.label1 = new System.Windows.Forms.Label();
            this.panel3 = new System.Windows.Forms.Panel();
            this.lblTotalIncome = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.panel4 = new System.Windows.Forms.Panel();
            this.lblPendingOrder = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.panel5 = new System.Windows.Forms.Panel();
            this.lblCompletedOrders = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.lblTotalCustomer = new System.Windows.Forms.Label();
            ((System.ComponentModel.ISupportInitialize)(this.dgvRecentOrders)).BeginInit();
            this.panel1.SuspendLayout();
            this.tableLayoutPanel1.SuspendLayout();
            this.panel2.SuspendLayout();
            this.panel3.SuspendLayout();
            this.panel4.SuspendLayout();
            this.panel5.SuspendLayout();
            this.SuspendLayout();
            // 
            // dgvRecentOrders
            // 
            this.dgvRecentOrders.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dgvRecentOrders.Location = new System.Drawing.Point(214, 185);
            this.dgvRecentOrders.Name = "dgvRecentOrders";
            this.dgvRecentOrders.RowHeadersWidth = 62;
            this.dgvRecentOrders.RowTemplate.Height = 28;
            this.dgvRecentOrders.Size = new System.Drawing.Size(692, 213);
            this.dgvRecentOrders.TabIndex = 0;
            // 
            // panel1
            // 
            this.panel1.Controls.Add(this.tableLayoutPanel1);
            this.panel1.Dock = System.Windows.Forms.DockStyle.Left;
            this.panel1.Location = new System.Drawing.Point(0, 0);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(158, 398);
            this.panel1.TabIndex = 2;
            // 
            // tableLayoutPanel1
            // 
            this.tableLayoutPanel1.ColumnCount = 1;
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel1.Controls.Add(this.btnReports, 0, 5);
            this.tableLayoutPanel1.Controls.Add(this.btnPayment, 0, 4);
            this.tableLayoutPanel1.Controls.Add(this.btnOrderTracking, 0, 3);
            this.tableLayoutPanel1.Controls.Add(this.btnService, 0, 2);
            this.tableLayoutPanel1.Controls.Add(this.btnCustomer, 0, 1);
            this.tableLayoutPanel1.Controls.Add(this.btnButton, 0, 0);
            this.tableLayoutPanel1.Location = new System.Drawing.Point(0, 0);
            this.tableLayoutPanel1.Name = "tableLayoutPanel1";
            this.tableLayoutPanel1.RowCount = 6;
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 16.80441F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 16.80441F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 16.80441F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 16.52892F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 16.52892F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 16.52892F));
            this.tableLayoutPanel1.Size = new System.Drawing.Size(158, 309);
            this.tableLayoutPanel1.TabIndex = 3;
            // 
            // btnReports
            // 
            this.btnReports.BackColor = System.Drawing.SystemColors.MenuBar;
            this.btnReports.Location = new System.Drawing.Point(3, 258);
            this.btnReports.Name = "btnReports";
            this.btnReports.Size = new System.Drawing.Size(152, 45);
            this.btnReports.TabIndex = 5;
            this.btnReports.Text = "Reports";
            this.btnReports.UseVisualStyleBackColor = false;
            this.btnReports.Click += new System.EventHandler(this.btnReports_Click);
            // 
            // btnPayment
            // 
            this.btnPayment.BackColor = System.Drawing.SystemColors.MenuBar;
            this.btnPayment.Location = new System.Drawing.Point(3, 207);
            this.btnPayment.Name = "btnPayment";
            this.btnPayment.Size = new System.Drawing.Size(152, 45);
            this.btnPayment.TabIndex = 4;
            this.btnPayment.Text = "Payments";
            this.btnPayment.UseVisualStyleBackColor = false;
            this.btnPayment.Click += new System.EventHandler(this.btnPayment_Click);
            // 
            // btnOrderTracking
            // 
            this.btnOrderTracking.BackColor = System.Drawing.SystemColors.MenuBar;
            this.btnOrderTracking.Location = new System.Drawing.Point(3, 156);
            this.btnOrderTracking.Name = "btnOrderTracking";
            this.btnOrderTracking.Size = new System.Drawing.Size(152, 45);
            this.btnOrderTracking.TabIndex = 3;
            this.btnOrderTracking.Text = "Order Tracking";
            this.btnOrderTracking.UseVisualStyleBackColor = false;
            this.btnOrderTracking.Click += new System.EventHandler(this.btnOrderTracking_Click);
            // 
            // btnService
            // 
            this.btnService.BackColor = System.Drawing.SystemColors.MenuBar;
            this.btnService.Location = new System.Drawing.Point(3, 105);
            this.btnService.Name = "btnService";
            this.btnService.Size = new System.Drawing.Size(152, 45);
            this.btnService.TabIndex = 2;
            this.btnService.Text = "Service";
            this.btnService.UseVisualStyleBackColor = false;
            this.btnService.Click += new System.EventHandler(this.btnService_Click);
            // 
            // btnCustomer
            // 
            this.btnCustomer.BackColor = System.Drawing.SystemColors.MenuBar;
            this.btnCustomer.Location = new System.Drawing.Point(3, 54);
            this.btnCustomer.Name = "btnCustomer";
            this.btnCustomer.Size = new System.Drawing.Size(152, 45);
            this.btnCustomer.TabIndex = 1;
            this.btnCustomer.Text = "Customer";
            this.btnCustomer.UseVisualStyleBackColor = false;
            this.btnCustomer.Click += new System.EventHandler(this.btnCustomer_Click);
            // 
            // btnButton
            // 
            this.btnButton.BackColor = System.Drawing.SystemColors.MenuBar;
            this.btnButton.Location = new System.Drawing.Point(3, 3);
            this.btnButton.Name = "btnButton";
            this.btnButton.Size = new System.Drawing.Size(152, 45);
            this.btnButton.TabIndex = 0;
            this.btnButton.Text = "Dashboard";
            this.btnButton.UseVisualStyleBackColor = false;
            this.btnButton.Click += new System.EventHandler(this.btnButton_Click);
            // 
            // panel2
            // 
            this.panel2.Controls.Add(this.lblTotalCustomer);
            this.panel2.Controls.Add(this.label1);
            this.panel2.Location = new System.Drawing.Point(214, 0);
            this.panel2.Name = "panel2";
            this.panel2.Size = new System.Drawing.Size(215, 72);
            this.panel2.TabIndex = 3;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(3, 6);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(117, 20);
            this.label1.TabIndex = 0;
            this.label1.Text = "Total Customer";
            // 
            // panel3
            // 
            this.panel3.Controls.Add(this.lblTotalIncome);
            this.panel3.Controls.Add(this.label2);
            this.panel3.Location = new System.Drawing.Point(452, 3);
            this.panel3.Name = "panel3";
            this.panel3.Size = new System.Drawing.Size(215, 72);
            this.panel3.TabIndex = 4;
            // 
            // lblTotalIncome
            // 
            this.lblTotalIncome.AutoSize = true;
            this.lblTotalIncome.Location = new System.Drawing.Point(170, 49);
            this.lblTotalIncome.Name = "lblTotalIncome";
            this.lblTotalIncome.Size = new System.Drawing.Size(42, 20);
            this.lblTotalIncome.TabIndex = 3;
            this.lblTotalIncome.Text = "label";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(3, 6);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(101, 20);
            this.label2.TabIndex = 0;
            this.label2.Text = "Total Income";
            // 
            // panel4
            // 
            this.panel4.Controls.Add(this.lblPendingOrder);
            this.panel4.Controls.Add(this.label3);
            this.panel4.Location = new System.Drawing.Point(691, 3);
            this.panel4.Name = "panel4";
            this.panel4.Size = new System.Drawing.Size(215, 72);
            this.panel4.TabIndex = 4;
            // 
            // lblPendingOrder
            // 
            this.lblPendingOrder.AutoSize = true;
            this.lblPendingOrder.Location = new System.Drawing.Point(170, 49);
            this.lblPendingOrder.Name = "lblPendingOrder";
            this.lblPendingOrder.Size = new System.Drawing.Size(42, 20);
            this.lblPendingOrder.TabIndex = 7;
            this.lblPendingOrder.Text = "label";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(3, 6);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(111, 20);
            this.label3.TabIndex = 0;
            this.label3.Text = "Pending Order";
            // 
            // panel5
            // 
            this.panel5.Controls.Add(this.lblCompletedOrders);
            this.panel5.Controls.Add(this.label4);
            this.panel5.Location = new System.Drawing.Point(214, 93);
            this.panel5.Name = "panel5";
            this.panel5.Size = new System.Drawing.Size(215, 72);
            this.panel5.TabIndex = 4;
            // 
            // lblCompletedOrders
            // 
            this.lblCompletedOrders.AutoSize = true;
            this.lblCompletedOrders.Location = new System.Drawing.Point(136, 37);
            this.lblCompletedOrders.Name = "lblCompletedOrders";
            this.lblCompletedOrders.Size = new System.Drawing.Size(42, 20);
            this.lblCompletedOrders.TabIndex = 6;
            this.lblCompletedOrders.Text = "label";
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(3, 6);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(138, 20);
            this.label4.TabIndex = 0;
            this.label4.Text = "Completed Orders";
            // 
            // lblTotalCustomer
            // 
            this.lblTotalCustomer.AutoSize = true;
            this.lblTotalCustomer.Location = new System.Drawing.Point(136, 52);
            this.lblTotalCustomer.Name = "lblTotalCustomer";
            this.lblTotalCustomer.Size = new System.Drawing.Size(42, 20);
            this.lblTotalCustomer.TabIndex = 1;
            this.lblTotalCustomer.Text = "label";
            // 
            // Dashboard
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(9F, 20F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(972, 398);
            this.Controls.Add(this.panel5);
            this.Controls.Add(this.panel4);
            this.Controls.Add(this.panel3);
            this.Controls.Add(this.panel2);
            this.Controls.Add(this.dgvRecentOrders);
            this.Controls.Add(this.panel1);
            this.Name = "Dashboard";
            this.Text = "Dashboard";
            this.Load += new System.EventHandler(this.Dashboard_Load);
            ((System.ComponentModel.ISupportInitialize)(this.dgvRecentOrders)).EndInit();
            this.panel1.ResumeLayout(false);
            this.tableLayoutPanel1.ResumeLayout(false);
            this.panel2.ResumeLayout(false);
            this.panel2.PerformLayout();
            this.panel3.ResumeLayout(false);
            this.panel3.PerformLayout();
            this.panel4.ResumeLayout(false);
            this.panel4.PerformLayout();
            this.panel5.ResumeLayout(false);
            this.panel5.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.DataGridView dgvRecentOrders;
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel1;
        private System.Windows.Forms.Button btnReports;
        private System.Windows.Forms.Button btnPayment;
        private System.Windows.Forms.Button btnOrderTracking;
        private System.Windows.Forms.Button btnService;
        private System.Windows.Forms.Button btnCustomer;
        private System.Windows.Forms.Button btnButton;
        private System.Windows.Forms.Panel panel2;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Panel panel3;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Panel panel4;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Panel panel5;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Label lblTotalIncome;
        private System.Windows.Forms.Label lblPendingOrder;
        private System.Windows.Forms.Label lblCompletedOrders;
        private System.Windows.Forms.Label lblTotalCustomer;
    }
}

