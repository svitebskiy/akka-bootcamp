﻿namespace ChartApp
{
    partial class Main
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
            System.Windows.Forms.DataVisualization.Charting.ChartArea chartArea2 = new System.Windows.Forms.DataVisualization.Charting.ChartArea();
            System.Windows.Forms.DataVisualization.Charting.Legend legend2 = new System.Windows.Forms.DataVisualization.Charting.Legend();
            System.Windows.Forms.DataVisualization.Charting.Series series2 = new System.Windows.Forms.DataVisualization.Charting.Series();
            this.sysChart = new System.Windows.Forms.DataVisualization.Charting.Chart();
            this.cpuBtn = new System.Windows.Forms.Button();
            this.memoryBtn = new System.Windows.Forms.Button();
            this.diskBtn = new System.Windows.Forms.Button();
            this.pauseResumeBtn = new System.Windows.Forms.Button();
            ((System.ComponentModel.ISupportInitialize)(this.sysChart)).BeginInit();
            this.SuspendLayout();
            // 
            // sysChart
            // 
            chartArea2.Name = "ChartArea1";
            this.sysChart.ChartAreas.Add(chartArea2);
            this.sysChart.Dock = System.Windows.Forms.DockStyle.Fill;
            legend2.Name = "Legend1";
            this.sysChart.Legends.Add(legend2);
            this.sysChart.Location = new System.Drawing.Point(0, 0);
            this.sysChart.Name = "sysChart";
            series2.ChartArea = "ChartArea1";
            series2.Legend = "Legend1";
            series2.Name = "Series1";
            this.sysChart.Series.Add(series2);
            this.sysChart.Size = new System.Drawing.Size(684, 446);
            this.sysChart.TabIndex = 0;
            this.sysChart.Text = "sysChart";
            // 
            // cpuBtn
            // 
            this.cpuBtn.Location = new System.Drawing.Point(572, 323);
            this.cpuBtn.Name = "cpuBtn";
            this.cpuBtn.Size = new System.Drawing.Size(100, 23);
            this.cpuBtn.TabIndex = 1;
            this.cpuBtn.Text = "CPU (ON)";
            this.cpuBtn.UseVisualStyleBackColor = true;
            this.cpuBtn.Click += new System.EventHandler(this.cpuBtn_Click);
            // 
            // memoryBtn
            // 
            this.memoryBtn.Location = new System.Drawing.Point(572, 364);
            this.memoryBtn.Name = "memoryBtn";
            this.memoryBtn.Size = new System.Drawing.Size(100, 23);
            this.memoryBtn.TabIndex = 2;
            this.memoryBtn.Text = "MEMORY (OFF)";
            this.memoryBtn.UseVisualStyleBackColor = true;
            this.memoryBtn.Click += new System.EventHandler(this.memoryBtn_Click);
            // 
            // diskBtn
            // 
            this.diskBtn.Location = new System.Drawing.Point(572, 403);
            this.diskBtn.Name = "diskBtn";
            this.diskBtn.Size = new System.Drawing.Size(100, 23);
            this.diskBtn.TabIndex = 3;
            this.diskBtn.Text = "DISK (OFF)";
            this.diskBtn.UseVisualStyleBackColor = true;
            this.diskBtn.Click += new System.EventHandler(this.diskBtn_Click);
            // 
            // pauseResumeBtn
            // 
            this.pauseResumeBtn.Location = new System.Drawing.Point(572, 245);
            this.pauseResumeBtn.Name = "pauseResumeBtn";
            this.pauseResumeBtn.Size = new System.Drawing.Size(100, 23);
            this.pauseResumeBtn.TabIndex = 4;
            this.pauseResumeBtn.Text = "Pause ||";
            this.pauseResumeBtn.UseVisualStyleBackColor = true;
            this.pauseResumeBtn.Click += new System.EventHandler(this.pauseResumeBtn_Click);
            // 
            // Main
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(684, 446);
            this.Controls.Add(this.pauseResumeBtn);
            this.Controls.Add(this.diskBtn);
            this.Controls.Add(this.memoryBtn);
            this.Controls.Add(this.cpuBtn);
            this.Controls.Add(this.sysChart);
            this.Name = "Main";
            this.Text = "System Metrics";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.Main_FormClosing);
            this.Load += new System.EventHandler(this.Main_Load);
            ((System.ComponentModel.ISupportInitialize)(this.sysChart)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.DataVisualization.Charting.Chart sysChart;
        private System.Windows.Forms.Button cpuBtn;
        private System.Windows.Forms.Button memoryBtn;
        private System.Windows.Forms.Button diskBtn;
        private System.Windows.Forms.Button pauseResumeBtn;
    }
}

