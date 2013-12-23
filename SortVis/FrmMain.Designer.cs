namespace SortVis
{
    partial class FrmMain
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
                if (components != null)
                {
                    components.Dispose();
                }
                _container.Dispose();
                if (_cts != null)
                {
                    _cts.Dispose();
                }
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
            this.components = new System.ComponentModel.Container();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle7 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle8 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle9 = new System.Windows.Forms.DataGridViewCellStyle();
            this.CmbGenerator = new System.Windows.Forms.ComboBox();
            this.NumCount = new System.Windows.Forms.NumericUpDown();
            this.BtnAll = new System.Windows.Forms.Button();
            this.BtnAbort = new System.Windows.Forms.Button();
            this.DgvSorters = new System.Windows.Forms.DataGridView();
            this.ClmRun = new System.Windows.Forms.DataGridViewCheckBoxColumn();
            this.nameDataGridViewTextBoxColumn = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.ClmnBigO = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.stableDataGridViewCheckBoxColumn = new System.Windows.Forms.DataGridViewCheckBoxColumn();
            this.comparesDataGridViewTextBoxColumn = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.writesDataGridViewTextBoxColumn = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.millisecondsDataGridViewTextBoxColumn = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.ArrayBitmap = new System.Windows.Forms.DataGridViewImageColumn();
            this.iSorterBindingSource = new System.Windows.Forms.BindingSource(this.components);
            ((System.ComponentModel.ISupportInitialize)(this.NumCount)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.DgvSorters)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.iSorterBindingSource)).BeginInit();
            this.SuspendLayout();
            // 
            // CmbGenerator
            // 
            this.CmbGenerator.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.CmbGenerator.FormattingEnabled = true;
            this.CmbGenerator.Location = new System.Drawing.Point(12, 12);
            this.CmbGenerator.Name = "CmbGenerator";
            this.CmbGenerator.Size = new System.Drawing.Size(757, 21);
            this.CmbGenerator.TabIndex = 1;
            this.CmbGenerator.SelectedValueChanged += new System.EventHandler(this.CmbGenerator_SelectedValueChanged);
            // 
            // NumCount
            // 
            this.NumCount.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.NumCount.Increment = new decimal(new int[] {
            255,
            0,
            0,
            0});
            this.NumCount.Location = new System.Drawing.Point(775, 12);
            this.NumCount.Maximum = new decimal(new int[] {
            9999999,
            0,
            0,
            0});
            this.NumCount.Minimum = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.NumCount.Name = "NumCount";
            this.NumCount.Size = new System.Drawing.Size(100, 20);
            this.NumCount.TabIndex = 2;
            this.NumCount.ThousandsSeparator = true;
            this.NumCount.Value = new decimal(new int[] {
            256,
            0,
            0,
            0});
            this.NumCount.ValueChanged += new System.EventHandler(this.NumCount_ValueChanged);
            this.NumCount.Enter += new System.EventHandler(this.NumCount_Enter);
            // 
            // BtnAll
            // 
            this.BtnAll.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.BtnAll.Location = new System.Drawing.Point(12, 593);
            this.BtnAll.Name = "BtnAll";
            this.BtnAll.Size = new System.Drawing.Size(75, 23);
            this.BtnAll.TabIndex = 4;
            this.BtnAll.Text = "&Start all";
            this.BtnAll.UseVisualStyleBackColor = true;
            this.BtnAll.Click += new System.EventHandler(this.BtnAll_Click);
            // 
            // BtnAbort
            // 
            this.BtnAbort.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.BtnAbort.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.BtnAbort.Enabled = false;
            this.BtnAbort.Location = new System.Drawing.Point(800, 593);
            this.BtnAbort.Name = "BtnAbort";
            this.BtnAbort.Size = new System.Drawing.Size(75, 23);
            this.BtnAbort.TabIndex = 5;
            this.BtnAbort.Text = "&Abort";
            this.BtnAbort.UseVisualStyleBackColor = true;
            this.BtnAbort.Click += new System.EventHandler(this.BtnAbort_Click);
            // 
            // DgvSorters
            // 
            this.DgvSorters.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.DgvSorters.AutoGenerateColumns = false;
            this.DgvSorters.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.AllCells;
            this.DgvSorters.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.DgvSorters.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.ClmRun,
            this.nameDataGridViewTextBoxColumn,
            this.ClmnBigO,
            this.stableDataGridViewCheckBoxColumn,
            this.comparesDataGridViewTextBoxColumn,
            this.writesDataGridViewTextBoxColumn,
            this.millisecondsDataGridViewTextBoxColumn,
            this.ArrayBitmap});
            this.DgvSorters.DataSource = this.iSorterBindingSource;
            this.DgvSorters.Location = new System.Drawing.Point(12, 39);
            this.DgvSorters.Name = "DgvSorters";
            this.DgvSorters.RowHeadersVisible = false;
            this.DgvSorters.RowTemplate.Height = 64;
            this.DgvSorters.RowTemplate.Resizable = System.Windows.Forms.DataGridViewTriState.False;
            this.DgvSorters.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this.DgvSorters.Size = new System.Drawing.Size(863, 548);
            this.DgvSorters.TabIndex = 6;
            this.DgvSorters.CellMouseDoubleClick += new System.Windows.Forms.DataGridViewCellMouseEventHandler(this.DgvSorters_CellMouseDoubleClick);
            this.DgvSorters.ColumnHeaderMouseClick += new System.Windows.Forms.DataGridViewCellMouseEventHandler(this.DgvSorters_ColumnHeaderMouseClick);
            this.DgvSorters.ColumnWidthChanged += new System.Windows.Forms.DataGridViewColumnEventHandler(this.DgvSorters_ColumnWidthChanged);
            this.DgvSorters.RowHeightChanged += new System.Windows.Forms.DataGridViewRowEventHandler(this.DgvSorters_RowHeightChanged);
            // 
            // ClmRun
            // 
            this.ClmRun.DataPropertyName = "Run";
            this.ClmRun.HeaderText = "Run";
            this.ClmRun.Name = "ClmRun";
            this.ClmRun.Width = 33;
            // 
            // nameDataGridViewTextBoxColumn
            // 
            this.nameDataGridViewTextBoxColumn.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.Fill;
            this.nameDataGridViewTextBoxColumn.DataPropertyName = "Name";
            this.nameDataGridViewTextBoxColumn.HeaderText = "Name";
            this.nameDataGridViewTextBoxColumn.Name = "nameDataGridViewTextBoxColumn";
            this.nameDataGridViewTextBoxColumn.ReadOnly = true;
            // 
            // ClmnBigO
            // 
            this.ClmnBigO.DataPropertyName = "BigO";
            this.ClmnBigO.HeaderText = "BigO";
            this.ClmnBigO.Name = "ClmnBigO";
            this.ClmnBigO.ReadOnly = true;
            this.ClmnBigO.Width = 55;
            // 
            // stableDataGridViewCheckBoxColumn
            // 
            this.stableDataGridViewCheckBoxColumn.DataPropertyName = "Stable";
            this.stableDataGridViewCheckBoxColumn.HeaderText = "Stable";
            this.stableDataGridViewCheckBoxColumn.Name = "stableDataGridViewCheckBoxColumn";
            this.stableDataGridViewCheckBoxColumn.ReadOnly = true;
            this.stableDataGridViewCheckBoxColumn.Width = 43;
            // 
            // comparesDataGridViewTextBoxColumn
            // 
            this.comparesDataGridViewTextBoxColumn.DataPropertyName = "Compares";
            dataGridViewCellStyle7.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleRight;
            this.comparesDataGridViewTextBoxColumn.DefaultCellStyle = dataGridViewCellStyle7;
            this.comparesDataGridViewTextBoxColumn.HeaderText = "Compares";
            this.comparesDataGridViewTextBoxColumn.Name = "comparesDataGridViewTextBoxColumn";
            this.comparesDataGridViewTextBoxColumn.ReadOnly = true;
            this.comparesDataGridViewTextBoxColumn.Width = 79;
            // 
            // writesDataGridViewTextBoxColumn
            // 
            this.writesDataGridViewTextBoxColumn.DataPropertyName = "Writes";
            dataGridViewCellStyle8.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleRight;
            this.writesDataGridViewTextBoxColumn.DefaultCellStyle = dataGridViewCellStyle8;
            this.writesDataGridViewTextBoxColumn.HeaderText = "Writes";
            this.writesDataGridViewTextBoxColumn.Name = "writesDataGridViewTextBoxColumn";
            this.writesDataGridViewTextBoxColumn.ReadOnly = true;
            this.writesDataGridViewTextBoxColumn.Width = 62;
            // 
            // millisecondsDataGridViewTextBoxColumn
            // 
            this.millisecondsDataGridViewTextBoxColumn.DataPropertyName = "Milliseconds";
            dataGridViewCellStyle9.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleRight;
            this.millisecondsDataGridViewTextBoxColumn.DefaultCellStyle = dataGridViewCellStyle9;
            this.millisecondsDataGridViewTextBoxColumn.HeaderText = "Milliseconds";
            this.millisecondsDataGridViewTextBoxColumn.Name = "millisecondsDataGridViewTextBoxColumn";
            this.millisecondsDataGridViewTextBoxColumn.ReadOnly = true;
            this.millisecondsDataGridViewTextBoxColumn.Width = 89;
            // 
            // ArrayBitmap
            // 
            this.ArrayBitmap.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.Fill;
            this.ArrayBitmap.HeaderText = "Array state";
            this.ArrayBitmap.Name = "ArrayBitmap";
            this.ArrayBitmap.ReadOnly = true;
            // 
            // iSorterBindingSource
            // 
            this.iSorterBindingSource.DataSource = typeof(SortLib.ISorter);
            // 
            // FrmMain
            // 
            this.AcceptButton = this.BtnAll;
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.CancelButton = this.BtnAbort;
            this.ClientSize = new System.Drawing.Size(887, 628);
            this.Controls.Add(this.DgvSorters);
            this.Controls.Add(this.BtnAbort);
            this.Controls.Add(this.BtnAll);
            this.Controls.Add(this.NumCount);
            this.Controls.Add(this.CmbGenerator);
            this.Name = "FrmMain";
            this.Text = "SortVis";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.FrmMain_FormClosing);
            this.Load += new System.EventHandler(this.FrmMain_Load);
            ((System.ComponentModel.ISupportInitialize)(this.NumCount)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.DgvSorters)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.iSorterBindingSource)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.ComboBox CmbGenerator;
        private System.Windows.Forms.NumericUpDown NumCount;
        private System.Windows.Forms.Button BtnAll;
        private System.Windows.Forms.Button BtnAbort;
        private System.Windows.Forms.DataGridView DgvSorters;
        private System.Windows.Forms.BindingSource iSorterBindingSource;
        private System.Windows.Forms.DataGridViewCheckBoxColumn ClmRun;
        private System.Windows.Forms.DataGridViewTextBoxColumn nameDataGridViewTextBoxColumn;
        private System.Windows.Forms.DataGridViewTextBoxColumn ClmnBigO;
        private System.Windows.Forms.DataGridViewCheckBoxColumn stableDataGridViewCheckBoxColumn;
        private System.Windows.Forms.DataGridViewTextBoxColumn comparesDataGridViewTextBoxColumn;
        private System.Windows.Forms.DataGridViewTextBoxColumn writesDataGridViewTextBoxColumn;
        private System.Windows.Forms.DataGridViewTextBoxColumn millisecondsDataGridViewTextBoxColumn;
        private System.Windows.Forms.DataGridViewImageColumn ArrayBitmap;
    }
}

