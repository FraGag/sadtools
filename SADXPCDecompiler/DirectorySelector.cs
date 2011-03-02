//------------------------------------------------------------------------------
// <copyright file="DirectorySelector.cs" company="Sonic Retro &amp; Contributors">
//     Copyright (c) Sonic Retro &amp; Contributors. All rights reserved.
// </copyright>
//------------------------------------------------------------------------------

namespace SonicRetro.SonicAdventure.SADXPCDecompiler
{
    using System;
    using System.ComponentModel;
    using System.IO;
    using System.Windows.Forms;

    /// <summary>
    /// Provides a control for selecting a directory from a <see cref="FolderBrowserDialog"/>, by drag and drop or manually
    /// typing the path in a <see cref="TextBox"/>.
    /// </summary>
    public partial class DirectorySelector : UserControl
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="DirectorySelector"/> class.
        /// </summary>
        public DirectorySelector()
        {
            this.InitializeComponent();
        }

        /// <summary>
        /// Gets or sets a value indicating whether the control can accept data that the user drags onto it.
        /// </summary>
        /// <value>
        /// <c>true</c> if drag-and-drop operations are allowed in the control; otherwise, <c>false</c>. The default is
        /// <c>true</c>.
        /// </value>
        [DefaultValue(true)]
        public override bool AllowDrop
        {
            get
            {
                return base.AllowDrop;
            }
            set
            {
                base.AllowDrop = value;
            }
        }

        /// <summary>
        /// Gets or sets the path selected in the directory selector.
        /// </summary>
        public string SelectedPath
        {
            get
            {
                return this.directoryNameTextBox.Text;
            }

            set
            {
                this.directoryNameTextBox.Text = value;
                this.folderBrowserDialog.SelectedPath = value;
            }
        }

        /// <summary>
        /// Sets the target drop effect according to whether the drag and drop data contains acceptable data, then raises the
        /// <see cref="Control.DragEnter"/> event.
        /// </summary>
        /// <param name="drgevent">A <see cref="DragEventArgs"/> that contains the event data.</param>
        protected override void OnDragEnter(DragEventArgs drgevent)
        {
            if (drgevent.Data.GetDataPresent(DataFormats.FileDrop))
            {
                object data = drgevent.Data.GetData(DataFormats.FileDrop);
                string[] dataAsStringArray = data as string[];
                if (dataAsStringArray != null && dataAsStringArray.Length > 0 && Directory.Exists(dataAsStringArray[0]))
                {
                    drgevent.Effect = DragDropEffects.Copy;
                }
                else
                {
                    drgevent.Effect = DragDropEffects.None;
                }
            }
            else
            {
                drgevent.Effect = DragDropEffects.None;
            }

            base.OnDragEnter(drgevent);
        }

        /// <summary>
        /// Sets the file name of the current instance to the first file in the file list if the drag and drop data is
        /// acceptable, then raises the <see cref="Control.DragDrop"/> event.
        /// </summary>
        /// <param name="drgevent">A <see cref="DragEventArgs"/> that contains the event data.</param>
        protected override void OnDragDrop(DragEventArgs drgevent)
        {
            if (drgevent.Data.GetDataPresent(DataFormats.FileDrop))
            {
                object data = drgevent.Data.GetData(DataFormats.FileDrop);
                string[] dataAsStringArray = data as string[];
                if (dataAsStringArray != null && dataAsStringArray.Length > 0 && Directory.Exists(dataAsStringArray[0]))
                {
                    this.directoryNameTextBox.Text = dataAsStringArray[0];
                    this.folderBrowserDialog.SelectedPath = dataAsStringArray[0];
                }
            }

            base.OnDragDrop(drgevent);
        }

        private void browseButton_Click(object sender, EventArgs e)
        {
            if (this.folderBrowserDialog.ShowDialog() == DialogResult.OK)
            {
                this.directoryNameTextBox.Text = this.folderBrowserDialog.SelectedPath;
            }
        }
    }
}
