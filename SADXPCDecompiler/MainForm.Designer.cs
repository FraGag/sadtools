namespace SonicRetro.SonicAdventure.SADXPCDecompiler
{
    partial class MainForm
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            System.Windows.Forms.Label sourceModuleLabel;
            System.Windows.Forms.Label moduleDescriptionLabel;
            System.Windows.Forms.Label outputDirectoryLabel;
            this.mainTableLayoutPanel = new System.Windows.Forms.TableLayoutPanel();
            this.sourceModuleFileSelector = new SonicRetro.SonicAdventure.SADXPCDecompiler.FileSelector();
            this.moduleDescriptionFileSelector = new SonicRetro.SonicAdventure.SADXPCDecompiler.FileSelector();
            this.outputDirectorySelector = new SonicRetro.SonicAdventure.SADXPCDecompiler.DirectorySelector();
            this.bottomTableLayoutPanel = new System.Windows.Forms.TableLayoutPanel();
            this.statusLabel = new System.Windows.Forms.Label();
            this.decompileButton = new System.Windows.Forms.Button();
            this.exitButton = new System.Windows.Forms.Button();
            this.statusUpdaterTimer = new System.Windows.Forms.Timer(this.components);
            sourceModuleLabel = new System.Windows.Forms.Label();
            moduleDescriptionLabel = new System.Windows.Forms.Label();
            outputDirectoryLabel = new System.Windows.Forms.Label();
            this.mainTableLayoutPanel.SuspendLayout();
            this.bottomTableLayoutPanel.SuspendLayout();
            this.SuspendLayout();
            // 
            // sourceModuleLabel
            // 
            sourceModuleLabel.AutoSize = true;
            sourceModuleLabel.Dock = System.Windows.Forms.DockStyle.Fill;
            sourceModuleLabel.Location = new System.Drawing.Point(3, 0);
            sourceModuleLabel.Name = "sourceModuleLabel";
            sourceModuleLabel.Size = new System.Drawing.Size(131, 30);
            sourceModuleLabel.TabIndex = 0;
            sourceModuleLabel.Text = "&Source module (EXE/DLL):";
            sourceModuleLabel.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // moduleDescriptionLabel
            // 
            moduleDescriptionLabel.AutoSize = true;
            moduleDescriptionLabel.Dock = System.Windows.Forms.DockStyle.Fill;
            moduleDescriptionLabel.Location = new System.Drawing.Point(3, 30);
            moduleDescriptionLabel.Name = "moduleDescriptionLabel";
            moduleDescriptionLabel.Size = new System.Drawing.Size(131, 30);
            moduleDescriptionLabel.TabIndex = 2;
            moduleDescriptionLabel.Text = "Module &description:";
            moduleDescriptionLabel.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // outputDirectoryLabel
            // 
            outputDirectoryLabel.AutoSize = true;
            outputDirectoryLabel.Dock = System.Windows.Forms.DockStyle.Fill;
            outputDirectoryLabel.Location = new System.Drawing.Point(3, 60);
            outputDirectoryLabel.Name = "outputDirectoryLabel";
            outputDirectoryLabel.Size = new System.Drawing.Size(131, 30);
            outputDirectoryLabel.TabIndex = 4;
            outputDirectoryLabel.Text = "&Output directory:";
            outputDirectoryLabel.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // mainTableLayoutPanel
            // 
            this.mainTableLayoutPanel.ColumnCount = 2;
            this.mainTableLayoutPanel.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            this.mainTableLayoutPanel.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.mainTableLayoutPanel.Controls.Add(sourceModuleLabel, 0, 0);
            this.mainTableLayoutPanel.Controls.Add(this.sourceModuleFileSelector, 1, 0);
            this.mainTableLayoutPanel.Controls.Add(moduleDescriptionLabel, 0, 1);
            this.mainTableLayoutPanel.Controls.Add(this.moduleDescriptionFileSelector, 1, 1);
            this.mainTableLayoutPanel.Controls.Add(outputDirectoryLabel, 0, 2);
            this.mainTableLayoutPanel.Controls.Add(this.outputDirectorySelector, 1, 2);
            this.mainTableLayoutPanel.Controls.Add(this.bottomTableLayoutPanel, 0, 3);
            this.mainTableLayoutPanel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.mainTableLayoutPanel.Location = new System.Drawing.Point(0, 0);
            this.mainTableLayoutPanel.Name = "mainTableLayoutPanel";
            this.mainTableLayoutPanel.RowCount = 4;
            this.mainTableLayoutPanel.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.mainTableLayoutPanel.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.mainTableLayoutPanel.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.mainTableLayoutPanel.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.mainTableLayoutPanel.Size = new System.Drawing.Size(454, 119);
            this.mainTableLayoutPanel.TabIndex = 0;
            // 
            // sourceModuleFileSelector
            // 
            this.sourceModuleFileSelector.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right)));
            this.sourceModuleFileSelector.DataBindings.Add(new System.Windows.Forms.Binding("FileName", global::SonicRetro.SonicAdventure.SADXPCDecompiler.Properties.Settings.Default, "LastSourceModule", true, System.Windows.Forms.DataSourceUpdateMode.OnPropertyChanged));
            this.sourceModuleFileSelector.FileName = global::SonicRetro.SonicAdventure.SADXPCDecompiler.Properties.Settings.Default.LastSourceModule;
            this.sourceModuleFileSelector.Filter = "Portable Executable Modules (*.exe, *.dll)|*.exe;*.dll|All Files (*.*)|*.*";
            this.sourceModuleFileSelector.Location = new System.Drawing.Point(140, 3);
            this.sourceModuleFileSelector.Name = "sourceModuleFileSelector";
            this.sourceModuleFileSelector.Size = new System.Drawing.Size(311, 24);
            this.sourceModuleFileSelector.TabIndex = 1;
            // 
            // moduleDescriptionFileSelector
            // 
            this.moduleDescriptionFileSelector.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right)));
            this.moduleDescriptionFileSelector.DataBindings.Add(new System.Windows.Forms.Binding("FileName", global::SonicRetro.SonicAdventure.SADXPCDecompiler.Properties.Settings.Default, "LastModuleDescription", true, System.Windows.Forms.DataSourceUpdateMode.OnPropertyChanged));
            this.moduleDescriptionFileSelector.FileName = global::SonicRetro.SonicAdventure.SADXPCDecompiler.Properties.Settings.Default.LastModuleDescription;
            this.moduleDescriptionFileSelector.Filter = "XML Files (*.xml)|*.xml|All Files (*.*)|*.*";
            this.moduleDescriptionFileSelector.Location = new System.Drawing.Point(140, 33);
            this.moduleDescriptionFileSelector.Name = "moduleDescriptionFileSelector";
            this.moduleDescriptionFileSelector.Size = new System.Drawing.Size(311, 24);
            this.moduleDescriptionFileSelector.TabIndex = 3;
            // 
            // outputDirectorySelector
            // 
            this.outputDirectorySelector.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right)));
            this.outputDirectorySelector.DataBindings.Add(new System.Windows.Forms.Binding("SelectedPath", global::SonicRetro.SonicAdventure.SADXPCDecompiler.Properties.Settings.Default, "LastOutputDirectory", true, System.Windows.Forms.DataSourceUpdateMode.OnPropertyChanged));
            this.outputDirectorySelector.Location = new System.Drawing.Point(140, 63);
            this.outputDirectorySelector.Name = "outputDirectorySelector";
            this.outputDirectorySelector.SelectedPath = global::SonicRetro.SonicAdventure.SADXPCDecompiler.Properties.Settings.Default.LastOutputDirectory;
            this.outputDirectorySelector.Size = new System.Drawing.Size(311, 24);
            this.outputDirectorySelector.TabIndex = 5;
            // 
            // bottomTableLayoutPanel
            // 
            this.bottomTableLayoutPanel.AutoSize = true;
            this.bottomTableLayoutPanel.ColumnCount = 3;
            this.mainTableLayoutPanel.SetColumnSpan(this.bottomTableLayoutPanel, 2);
            this.bottomTableLayoutPanel.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.bottomTableLayoutPanel.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            this.bottomTableLayoutPanel.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            this.bottomTableLayoutPanel.Controls.Add(this.statusLabel, 0, 0);
            this.bottomTableLayoutPanel.Controls.Add(this.decompileButton, 1, 0);
            this.bottomTableLayoutPanel.Controls.Add(this.exitButton, 2, 0);
            this.bottomTableLayoutPanel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.bottomTableLayoutPanel.Location = new System.Drawing.Point(0, 90);
            this.bottomTableLayoutPanel.Margin = new System.Windows.Forms.Padding(0);
            this.bottomTableLayoutPanel.Name = "bottomTableLayoutPanel";
            this.bottomTableLayoutPanel.RowCount = 1;
            this.bottomTableLayoutPanel.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.bottomTableLayoutPanel.Size = new System.Drawing.Size(454, 29);
            this.bottomTableLayoutPanel.TabIndex = 7;
            // 
            // statusLabel
            // 
            this.statusLabel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.statusLabel.Location = new System.Drawing.Point(3, 0);
            this.statusLabel.Name = "statusLabel";
            this.statusLabel.Size = new System.Drawing.Size(286, 29);
            this.statusLabel.TabIndex = 7;
            // 
            // decompileButton
            // 
            this.decompileButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.decompileButton.Location = new System.Drawing.Point(295, 3);
            this.decompileButton.Name = "decompileButton";
            this.decompileButton.Size = new System.Drawing.Size(75, 23);
            this.decompileButton.TabIndex = 0;
            this.decompileButton.Text = "D&ecompile";
            this.decompileButton.UseVisualStyleBackColor = true;
            this.decompileButton.Click += new System.EventHandler(this.decompileButton_Click);
            // 
            // exitButton
            // 
            this.exitButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.exitButton.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.exitButton.Location = new System.Drawing.Point(376, 3);
            this.exitButton.Name = "exitButton";
            this.exitButton.Size = new System.Drawing.Size(75, 23);
            this.exitButton.TabIndex = 1;
            this.exitButton.Text = "E&xit";
            this.exitButton.UseVisualStyleBackColor = true;
            this.exitButton.Click += new System.EventHandler(this.exitButton_Click);
            // 
            // statusUpdaterTimer
            // 
            this.statusUpdaterTimer.Tick += new System.EventHandler(this.statusUpdaterTimer_Tick);
            // 
            // MainForm
            // 
            this.AcceptButton = this.decompileButton;
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.CancelButton = this.exitButton;
            this.ClientSize = new System.Drawing.Size(454, 119);
            this.Controls.Add(this.mainTableLayoutPanel);
            this.Font = new System.Drawing.Font("Tahoma", 8.25F);
            this.Name = "MainForm";
            this.Text = "SADX PC Data Decompiler";
            this.FormClosed += new System.Windows.Forms.FormClosedEventHandler(this.MainForm_FormClosed);
            this.mainTableLayoutPanel.ResumeLayout(false);
            this.mainTableLayoutPanel.PerformLayout();
            this.bottomTableLayoutPanel.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.TableLayoutPanel mainTableLayoutPanel;
        private SonicRetro.SonicAdventure.SADXPCDecompiler.FileSelector sourceModuleFileSelector;
        private SonicRetro.SonicAdventure.SADXPCDecompiler.FileSelector moduleDescriptionFileSelector;
        private SonicRetro.SonicAdventure.SADXPCDecompiler.DirectorySelector outputDirectorySelector;
        private System.Windows.Forms.TableLayoutPanel bottomTableLayoutPanel;
        private System.Windows.Forms.Label statusLabel;
        private System.Windows.Forms.Button exitButton;
        private System.Windows.Forms.Button decompileButton;
        private System.Windows.Forms.Timer statusUpdaterTimer;
    }
}

