//------------------------------------------------------------------------------
// <copyright file="MainForm.cs" company="Sonic Retro &amp; Contributors">
//     Copyright (c) Sonic Retro &amp; Contributors. All rights reserved.
// </copyright>
//------------------------------------------------------------------------------

namespace SonicRetro.SonicAdventure.SADXPCDecompiler
{
    using System;
    using System.Drawing;
    using System.Runtime.InteropServices;
    using System.Security.Permissions;
    using System.Threading;
    using System.Threading.Tasks;
    using System.Windows.Forms;

    public partial class MainForm : Form
    {
        private Task decompilationTask = null;
        private DecompilationStatus decompilationStatus = new DecompilationStatus();
        private CancellationTokenSource cancellationTokenSource = null;
        private NativeMethods.ITaskbarList3 taskbarList;
        private int closed;

        public MainForm()
        {
            this.InitializeComponent();
            this.MinimumSize = this.SizeFromClientSize(this.mainTableLayoutPanel.GetPreferredSize(Size.Empty) + this.Padding.Size);
        }

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                // Dispose managed resources
                if (this.decompilationTask != null)
                {
                    if (!this.decompilationTask.IsCompleted)
                    {
                        try
                        {
                            this.decompilationTask.Wait();
                        }
                        catch (AggregateException)
                        {
                        }
                    }

                    this.decompilationTask.Dispose();
                    this.decompilationTask = null;
                }

                if (this.cancellationTokenSource != null)
                {
                    this.cancellationTokenSource.Dispose();
                    this.cancellationTokenSource = null;
                }

                if (this.components != null)
                {
                    this.components.Dispose();
                }
            }

            // Dispose unmanaged resources
            if (this.taskbarList != null)
            {
                Marshal.FinalReleaseComObject(this.taskbarList);
                this.taskbarList = null;
            }

            base.Dispose(disposing);
        }

        protected override void WndProc(ref Message m)
        {
            new SecurityPermission(SecurityPermissionFlag.UnmanagedCode).Demand();
            if (m.Msg == NativeMethods.WM_TASKBARBUTTONCREATED)
            {
                this.taskbarList = (NativeMethods.ITaskbarList3)Activator.CreateInstance(Type.GetTypeFromCLSID(new Guid("56FDF344-FD6D-11d0-958A-006097C9A090")));
                this.taskbarList.HrInit();
            }

            base.WndProc(ref m);
        }

        private void MainForm_FormClosed(object sender, FormClosedEventArgs e)
        {
            Interlocked.Increment(ref this.closed);
            if (this.cancellationTokenSource != null)
            {
                this.cancellationTokenSource.Cancel();
            }
        }

        private void decompileButton_Click(object sender, EventArgs e)
        {
            // Save settings now, in case the finally block in Program.Main isn't run.
            Properties.Settings.Default.Save();

            this.decompilationStatus.StatusText = "Starting up...";
            if (this.taskbarList != null)
            {
                this.taskbarList.SetProgressState(this.Handle, NativeMethods.TBPFLAG.TBPF_INDETERMINATE);
            }

            this.cancellationTokenSource = new CancellationTokenSource();
            DecompilationOperation decompilationOperation = new DecompilationOperation(
                this.sourceModuleFileSelector.FileName,
                this.moduleDescriptionFileSelector.FileName,
                this.outputDirectorySelector.SelectedPath,
                this.decompilationStatus,
                cancellationTokenSource.Token);
            this.decompilationTask = new Task(
                decompilationOperation.Decompile,
                this.cancellationTokenSource.Token,
                TaskCreationOptions.LongRunning);
            this.decompilationTask.Start();

            Thread thread = new Thread(new ThreadStart(
                delegate
                {
                    try
                    {
                        this.decompilationTask.Wait();
                    }
                    catch (AggregateException)
                    {
                        // Ignore the AggregateException. It is stored in the task and will be displayed in
                        // OnDecompilationCompleted.
                    }

                    if (this.closed == 0)
                    {
                        this.Invoke(new MethodInvoker(this.OnDecompilationCompleted));
                    }
                }));
            thread.Start();

            // Lock input controls
            this.sourceModuleFileSelector.Enabled = false;
            this.moduleDescriptionFileSelector.Enabled = false;
            this.outputDirectorySelector.Enabled = false;

            // Transform the "Decompile" button into a "Cancel" button
            this.decompileButton.Text = "&Cancel";
            this.decompileButton.Click -= this.decompileButton_Click;
            this.decompileButton.Click += this.cancelButton_Click;

            // Start the status updater timer
            this.statusUpdaterTimer.Start();
        }

        private void cancelButton_Click(object sender, EventArgs e)
        {
            if (this.cancellationTokenSource != null)
            {
                this.cancellationTokenSource.Cancel();
                this.decompileButton.Enabled = false;
            }
        }

        private void exitButton_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void OnDecompilationCompleted()
        {
            try
            {
                if (this.cancellationTokenSource != null)
                {
                    try
                    {
                    }
                    finally
                    {
                        this.cancellationTokenSource.Dispose();
                        this.cancellationTokenSource = null;
                    }
                }

                if (this.decompilationTask != null)
                {
                    if (this.decompilationTask.IsCanceled)
                    {
                        this.statusLabel.Text = "Canceled.";
                        if (this.taskbarList != null)
                        {
                            this.taskbarList.SetProgressState(this.Handle, NativeMethods.TBPFLAG.TBPF_NOPROGRESS);
                        }
                    }
                    else
                    {
                        AggregateException aggregateException;
                        try
                        {
                            aggregateException = this.decompilationTask.Exception;
                        }
                        finally
                        {
                            this.decompilationTask.Dispose();
                        }

                        if (aggregateException != null)
                        {
                            this.statusLabel.Text = this.decompilationStatus.StatusText = "An error occurred.";
                            if (aggregateException.InnerExceptions.Count == 1 && aggregateException.InnerExceptions[0] is PrettyMessageException)
                            {
                                this.ShowMessage(aggregateException.InnerExceptions[0].Message, true);
                            }
                            else
                            {
                                this.ShowMessage(aggregateException.ToString(), true);
                            }
                        }
                        else
                        {
                            this.statusLabel.Text = this.decompilationStatus.StatusText = "Done.";
                            this.ShowMessage("Done.", false);
                        }
                    }
                }
            }
            finally
            {
                // Stop the status updater timer
                this.statusUpdaterTimer.Stop();

                // Transform the "Cancel" button back into a "Decompile" button
                this.decompileButton.Text = "D&ecompile";
                this.decompileButton.Click -= this.cancelButton_Click;
                this.decompileButton.Click += this.decompileButton_Click;

                // Unlock input controls
                this.sourceModuleFileSelector.Enabled = true;
                this.moduleDescriptionFileSelector.Enabled = true;
                this.outputDirectorySelector.Enabled = true;
                this.decompileButton.Enabled = true;
            }
        }

        private void statusUpdaterTimer_Tick(object sender, EventArgs e)
        {
            this.statusLabel.Text = this.decompilationStatus.StatusText;
        }

        private void ShowMessage(string message, bool error)
        {
            this.taskbarList.SetProgressState(this.Handle, error ? NativeMethods.TBPFLAG.TBPF_ERROR : NativeMethods.TBPFLAG.TBPF_NORMAL);
            this.taskbarList.SetProgressValue(this.Handle, 1, 1);
            MessageBox.Show(this, message, this.Text, MessageBoxButtons.OK, error ? MessageBoxIcon.Error : MessageBoxIcon.Information, MessageBoxDefaultButton.Button1, this.RightToLeft == RightToLeft.Yes ? MessageBoxOptions.RightAlign | MessageBoxOptions.RtlReading : 0);
            this.taskbarList.SetProgressState(this.Handle, NativeMethods.TBPFLAG.TBPF_NOPROGRESS);
        }
    }
}
