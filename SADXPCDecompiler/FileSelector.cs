//------------------------------------------------------------------------------
// <copyright file="FileSelector.cs" company="Sonic Retro &amp; Contributors">
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
    /// Provides a control for selecting a file from an <see cref="OpenFileDialog"/>, by drag and drop or manually typing the
    /// path in a <see cref="TextBox"/>.
    /// </summary>
    public partial class FileSelector : UserControl
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="FileSelector"/> class.
        /// </summary>
        public FileSelector()
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
        /// Gets or sets a string containing the file name selected in the file selector.
        /// </summary>
        /// <value>The file name selected in the file dialog box. The default value is an empty string ("").</value>
        public string FileName
        {
            get
            {
                return this.fileNameTextBox.Text;
            }

            set
            {
                this.fileNameTextBox.Text = value;
                this.openFileDialog.FileName = value;
            }
        }

        /// <summary>
        /// Gets or sets the current file name filter string, which determines the choices that appear in the "Save as file
        /// type" or "Files of type" box in the dialog box.
        /// </summary>
        /// <value>The file filtering options available in the dialog box.</value>
        /// <exception cref="System.ArgumentException">Filter format is invalid.</exception>
        public string Filter
        {
            get
            {
                return this.openFileDialog.Filter;
            }

            set
            {
                this.openFileDialog.Filter = value;
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
                if (dataAsStringArray != null && dataAsStringArray.Length > 0 && File.Exists(dataAsStringArray[0]))
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
        /// Sets the selected path of the current instance to the first file in the file list if the drag and drop data is
        /// acceptable, then raises the <see cref="Control.DragDrop"/> event.
        /// </summary>
        /// <param name="drgevent">A <see cref="DragEventArgs"/> that contains the event data.</param>
        protected override void OnDragDrop(DragEventArgs drgevent)
        {
            if (drgevent.Data.GetDataPresent(DataFormats.FileDrop))
            {
                object data = drgevent.Data.GetData(DataFormats.FileDrop);
                string[] dataAsStringArray = data as string[];
                if (dataAsStringArray != null && dataAsStringArray.Length > 0 && File.Exists(dataAsStringArray[0]))
                {
                    this.fileNameTextBox.Text = dataAsStringArray[0];
                    this.openFileDialog.FileName = dataAsStringArray[0];
                }
            }

            base.OnDragDrop(drgevent);
        }

        private void browseButton_Click(object sender, EventArgs e)
        {
            if (this.openFileDialog.ShowDialog() == DialogResult.OK)
            {
                this.fileNameTextBox.Text = this.openFileDialog.FileName;
            }
        }
    }
}
