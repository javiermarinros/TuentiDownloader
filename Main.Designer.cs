namespace TuentiDownloader
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
            this.bStart = new System.Windows.Forms.Button();
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.savePath = new System.Windows.Forms.TextBox();
            this.bExplore = new System.Windows.Forms.Button();
            this.folderBrowserDialog1 = new System.Windows.Forms.FolderBrowserDialog();
            this.statusStrip1 = new System.Windows.Forms.StatusStrip();
            this.toolStripStatusLabel1 = new System.Windows.Forms.ToolStripStatusLabel();
            this.toolStripProgressBar1 = new System.Windows.Forms.ToolStripProgressBar();
            this.toolStripStatusLabel2 = new System.Windows.Forms.ToolStripStatusLabel();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.numericUpDown1 = new System.Windows.Forms.NumericUpDown();
            this.label3 = new System.Windows.Forms.Label();
            this.downloadProfile = new System.Windows.Forms.CheckBox();
            this.downloadMessages = new System.Windows.Forms.CheckBox();
            this.downloadPhotos = new System.Windows.Forms.CheckBox();
            this.bToggleSettings = new System.Windows.Forms.Button();
            this.webBrowser = new TuentiDownloader.WebBrowserEx();
            this.statusStrip1.SuspendLayout();
            this.groupBox1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDown1)).BeginInit();
            this.SuspendLayout();
            // 
            // bStart
            // 
            this.bStart.Location = new System.Drawing.Point(329, 8);
            this.bStart.Name = "bStart";
            this.bStart.Size = new System.Drawing.Size(83, 23);
            this.bStart.TabIndex = 1;
            this.bStart.Text = "Comenzar";
            this.bStart.UseVisualStyleBackColor = true;
            this.bStart.Click += new System.EventHandler(this.button1_Click);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(13, 13);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(311, 13);
            this.label1.TabIndex = 2;
            this.label1.Text = "Inicia sesión en Tuenti, dirígete a tu primera foto y pulsa el botón";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(8, 24);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(74, 13);
            this.label2.TabIndex = 3;
            this.label2.Text = "Descargar en:";
            // 
            // savePath
            // 
            this.savePath.Location = new System.Drawing.Point(88, 20);
            this.savePath.Name = "savePath";
            this.savePath.Size = new System.Drawing.Size(184, 20);
            this.savePath.TabIndex = 4;
            // 
            // bExplore
            // 
            this.bExplore.Location = new System.Drawing.Point(278, 19);
            this.bExplore.Name = "bExplore";
            this.bExplore.Size = new System.Drawing.Size(31, 23);
            this.bExplore.TabIndex = 5;
            this.bExplore.Text = "...";
            this.bExplore.UseVisualStyleBackColor = true;
            this.bExplore.Click += new System.EventHandler(this.button2_Click);
            // 
            // statusStrip1
            // 
            this.statusStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.toolStripStatusLabel1,
            this.toolStripProgressBar1,
            this.toolStripStatusLabel2});
            this.statusStrip1.Location = new System.Drawing.Point(0, 532);
            this.statusStrip1.Name = "statusStrip1";
            this.statusStrip1.Size = new System.Drawing.Size(1028, 22);
            this.statusStrip1.TabIndex = 6;
            this.statusStrip1.Text = "statusStrip1";
            // 
            // toolStripStatusLabel1
            // 
            this.toolStripStatusLabel1.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.toolStripStatusLabel1.Name = "toolStripStatusLabel1";
            this.toolStripStatusLabel1.Size = new System.Drawing.Size(613, 17);
            this.toolStripStatusLabel1.Spring = true;
            this.toolStripStatusLabel1.Text = "Listo";
            this.toolStripStatusLabel1.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // toolStripProgressBar1
            // 
            this.toolStripProgressBar1.Name = "toolStripProgressBar1";
            this.toolStripProgressBar1.Size = new System.Drawing.Size(250, 16);
            // 
            // toolStripStatusLabel2
            // 
            this.toolStripStatusLabel2.IsLink = true;
            this.toolStripStatusLabel2.Name = "toolStripStatusLabel2";
            this.toolStripStatusLabel2.Size = new System.Drawing.Size(117, 17);
            this.toolStripStatusLabel2.Text = "Compartir en Twitter";
            this.toolStripStatusLabel2.Click += new System.EventHandler(this.toolStripStatusLabel2_Click);
            // 
            // groupBox1
            // 
            this.groupBox1.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.groupBox1.Controls.Add(this.numericUpDown1);
            this.groupBox1.Controls.Add(this.label3);
            this.groupBox1.Controls.Add(this.downloadProfile);
            this.groupBox1.Controls.Add(this.downloadMessages);
            this.groupBox1.Controls.Add(this.downloadPhotos);
            this.groupBox1.Controls.Add(this.bExplore);
            this.groupBox1.Controls.Add(this.label2);
            this.groupBox1.Controls.Add(this.savePath);
            this.groupBox1.Location = new System.Drawing.Point(693, 37);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(323, 172);
            this.groupBox1.TabIndex = 7;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Opciones";
            this.groupBox1.Visible = false;
            // 
            // numericUpDown1
            // 
            this.numericUpDown1.Location = new System.Drawing.Point(128, 139);
            this.numericUpDown1.Maximum = new decimal(new int[] {
            100000000,
            0,
            0,
            0});
            this.numericUpDown1.Minimum = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.numericUpDown1.Name = "numericUpDown1";
            this.numericUpDown1.Size = new System.Drawing.Size(85, 20);
            this.numericUpDown1.TabIndex = 10;
            this.numericUpDown1.Value = new decimal(new int[] {
            1000,
            0,
            0,
            0});
            this.numericUpDown1.ValueChanged += new System.EventHandler(this.numericUpDown1_ValueChanged);
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(8, 143);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(114, 13);
            this.label3.TabIndex = 9;
            this.label3.Text = "Tiempo de espera (ms)";
            // 
            // downloadProfile
            // 
            this.downloadProfile.AutoSize = true;
            this.downloadProfile.Checked = true;
            this.downloadProfile.CheckState = System.Windows.Forms.CheckState.Checked;
            this.downloadProfile.Location = new System.Drawing.Point(11, 97);
            this.downloadProfile.Name = "downloadProfile";
            this.downloadProfile.Size = new System.Drawing.Size(135, 17);
            this.downloadProfile.TabIndex = 8;
            this.downloadProfile.Text = "Descargar comentarios";
            this.downloadProfile.UseVisualStyleBackColor = true;
            // 
            // downloadMessages
            // 
            this.downloadMessages.AutoSize = true;
            this.downloadMessages.Checked = true;
            this.downloadMessages.CheckState = System.Windows.Forms.CheckState.Checked;
            this.downloadMessages.Location = new System.Drawing.Point(11, 74);
            this.downloadMessages.Name = "downloadMessages";
            this.downloadMessages.Size = new System.Drawing.Size(165, 17);
            this.downloadMessages.TabIndex = 7;
            this.downloadMessages.Text = "Descargar mensajes privados";
            this.downloadMessages.UseVisualStyleBackColor = true;
            // 
            // downloadPhotos
            // 
            this.downloadPhotos.AutoSize = true;
            this.downloadPhotos.Checked = true;
            this.downloadPhotos.CheckState = System.Windows.Forms.CheckState.Checked;
            this.downloadPhotos.Location = new System.Drawing.Point(11, 51);
            this.downloadPhotos.Name = "downloadPhotos";
            this.downloadPhotos.Size = new System.Drawing.Size(101, 17);
            this.downloadPhotos.TabIndex = 6;
            this.downloadPhotos.Text = "Descargar fotos";
            this.downloadPhotos.UseVisualStyleBackColor = true;
            // 
            // bToggleSettings
            // 
            this.bToggleSettings.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.bToggleSettings.Location = new System.Drawing.Point(889, 8);
            this.bToggleSettings.Name = "bToggleSettings";
            this.bToggleSettings.Size = new System.Drawing.Size(126, 23);
            this.bToggleSettings.TabIndex = 8;
            this.bToggleSettings.Text = "Mostrar opciones";
            this.bToggleSettings.UseVisualStyleBackColor = true;
            this.bToggleSettings.Click += new System.EventHandler(this.button3_Click);
            // 
            // webBrowser
            // 
            this.webBrowser.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.webBrowser.Location = new System.Drawing.Point(28, 37);
            this.webBrowser.MinimumSize = new System.Drawing.Size(20, 20);
            this.webBrowser.Name = "webBrowser";
            this.webBrowser.ScriptErrorsSuppressed = true;
            this.webBrowser.Size = new System.Drawing.Size(1000, 493);
            this.webBrowser.TabIndex = 0;
            this.webBrowser.Timeout = 3000;
            this.webBrowser.Url = new System.Uri("http://www.tuenti.com/#m=Home&func=index", System.UriKind.Absolute);
            this.webBrowser.Navigated += new System.Windows.Forms.WebBrowserNavigatedEventHandler(this.webBrowser_Navigated);
            this.webBrowser.Navigating += new System.Windows.Forms.WebBrowserNavigatingEventHandler(this.webBrowser_Navigating);
            this.webBrowser.ProgressChanged += new System.Windows.Forms.WebBrowserProgressChangedEventHandler(this.webBrowser_ProgressChanged);
            // 
            // Main
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1028, 554);
            this.Controls.Add(this.bToggleSettings);
            this.Controls.Add(this.groupBox1);
            this.Controls.Add(this.statusStrip1);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.bStart);
            this.Controls.Add(this.webBrowser);
            this.Name = "Main";
            this.Text = "Tuenti Downloader";
            this.FormClosed += new System.Windows.Forms.FormClosedEventHandler(this.Main_FormClosed);
            this.statusStrip1.ResumeLayout(false);
            this.statusStrip1.PerformLayout();
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDown1)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private WebBrowserEx webBrowser;
        private System.Windows.Forms.Button bStart;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.TextBox savePath;
        private System.Windows.Forms.Button bExplore;
        private System.Windows.Forms.FolderBrowserDialog folderBrowserDialog1;
        private System.Windows.Forms.StatusStrip statusStrip1;
        private System.Windows.Forms.ToolStripStatusLabel toolStripStatusLabel1;
        private System.Windows.Forms.ToolStripProgressBar toolStripProgressBar1;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.CheckBox downloadProfile;
        private System.Windows.Forms.CheckBox downloadMessages;
        private System.Windows.Forms.CheckBox downloadPhotos;
        private System.Windows.Forms.Button bToggleSettings;
        private System.Windows.Forms.NumericUpDown numericUpDown1;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.ToolStripStatusLabel toolStripStatusLabel2;
    }
}

