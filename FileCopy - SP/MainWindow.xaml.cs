using Microsoft.Win32;
using System;
using System.IO;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Animation;

namespace FileCopy___SP
{

    public partial class MainWindow : Window
    {
        private string selectedFilePathFrom;
        private string selectedFilePathTo;

        public Thread MainThread {  get; set; }
        public MainWindow()
        {
            InitializeComponent();
            trueCopy();
        }


        private void btnFileFrom_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog
            {
                Filter = "Text files (*.txt)|*.txt",
            };

            if (openFileDialog.ShowDialog() == true)
            {
                selectedFilePathFrom = openFileDialog.FileName;
                btnFileTo.IsEnabled = true;
                txtBoxFrom.Text = selectedFilePathFrom;
            }
        }


        private void btnFileTo_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog
            {
                Filter = "Text files (*.txt)|*.txt",
            };

            if (openFileDialog.ShowDialog() == true)
            {
                selectedFilePathTo = openFileDialog.FileName;
                txtBoxTo.Text = selectedFilePathTo;
            }
        }



        private void btnSupsent_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                Dispatcher.Invoke(() =>
                {
                    MainThread.Suspend();

                    btnResume.IsEnabled = true;
                    btnAbort.IsEnabled = true;

                    btnCopy.IsEnabled = false;
                });
            }
            catch (Exception ex)
            {

                Dispatcher.Invoke(() =>
                {
                    MessageBox.Show(ex.Message);
                    trueCopy();
                });
            }
        }

        private void btnResume_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                Dispatcher.Invoke(() =>
                {
                    MainThread.Resume();

                });
            }
            catch (Exception ex)
            {

                Dispatcher.Invoke(() =>
                {
                    MessageBox.Show(ex.Message);
                    trueCopy();
                });
            }
        }

        private void btnAbort_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                Dispatcher.Invoke(() =>
                {
                    txtBoxFrom.Text = null;
                    txtBoxTo.Text = null;
                    Bar.Value = 0;

                    MainThread.Abort();
                    trueCopy();
                });
            }
            catch (Exception ex)
            {

                Dispatcher.Invoke(() =>
                {
                    MessageBox.Show(ex.Message);
                    trueCopy();
                });
            }
        }

        private void btnCopy_Click(object sender, RoutedEventArgs e)
        {
            falseCopy();

            MainThread = new Thread(CopyMethod);
            MainThread.Start();

        }
        

        private void CopyMethod()
        {
            bool check = false;
            try
            {
                if (string.IsNullOrEmpty(selectedFilePathFrom) || string.IsNullOrEmpty(selectedFilePathTo))
                {
                    Dispatcher.Invoke(() =>
                    {
                        MessageBox.Show("Please select both source and destination files.");
                        trueCopy();
                    });
                    return;
                }

                string body = null;

                body = File.ReadAllText(selectedFilePathFrom);

                Dispatcher.Invoke(() =>
                {
                    Bar.Value = 0;
                    Bar.Maximum = body.Length;  
                });           
                


                for (int i = 0; i < body.Length; i++)
                {
                    Dispatcher.Invoke(() => Bar.Value += i);
                    Thread.Sleep(100);
                    
                    if (i == body.Length - 1) check = true;
                }

                if (check)
                {
                    File.AppendAllText(selectedFilePathTo, body);
                    Dispatcher.Invoke(() =>
                    {
                        txtBoxFrom.Text = null;
                        txtBoxTo.Text = null;
                        Bar.Value = 0;
                        MessageBox.Show("File copy completed.");
                        trueCopy();
                    });
                }



            }
            catch (Exception ex)
            {
                Dispatcher.Invoke(() =>
                {
                    MessageBox.Show(ex.Message);
                    trueCopy();
                });
            }
        }

        void trueCopy()
        {
            btnCopy.IsEnabled = true;
            btnAbort.IsEnabled = false;
            btnResume.IsEnabled = false;
            btnSupsent.IsEnabled = false;
        }

        void falseCopy()
        {
            btnCopy.IsEnabled = false;
            btnAbort.IsEnabled = true;
            btnResume.IsEnabled = true;
            btnSupsent.IsEnabled = true;
        }


    }
}





//using Microsoft.Win32;
//using System;
//using System.IO;
//using System.Threading;
//using System.Windows;
//using System.Windows.Controls;

//namespace FileCopy___SP
//{
//    public partial class MainWindow : Window
//    {
//        private string selectedFilePathFrom;
//        private string selectedFilePathTo;
//        private Thread fileCopyThread;
//        private bool stopRequested = false;
//        private bool isPaused = false;
//        private AutoResetEvent pauseEvent = new AutoResetEvent(true);
//        private long bytesCopied = 0;
//        private long totalBytes = 0;

//        public MainWindow()
//        {
//            InitializeComponent();
//        }

//        private void btnFileFrom_Click(object sender, RoutedEventArgs e)
//        {
//            OpenFileDialog openFileDialog = new OpenFileDialog
//            {
//                Filter = "Text files (*.txt)|*.txt",
//            };

//            if (openFileDialog.ShowDialog() == true)
//            {
//                selectedFilePathFrom = openFileDialog.FileName;
//                btnFileTo.IsEnabled = true;
//                txtBoxFrom.Text = selectedFilePathFrom;
//            }
//        }

//        private void btnFileTo_Click(object sender, RoutedEventArgs e)
//        {
//            OpenFileDialog openFileDialog = new OpenFileDialog
//            {
//                Filter = "Text files (*.txt)|*.txt",
//            };

//            if (openFileDialog.ShowDialog() == true)
//            {
//                selectedFilePathTo = openFileDialog.FileName;
//                txtBoxTo.Text = selectedFilePathTo;
//            }
//        }

//        private void CopyFileMethod()
//        {
//            try
//            {
//                using (FileStream sourceStream = new FileStream(selectedFilePathFrom, FileMode.Open, FileAccess.Read))
//                using (FileStream destinationStream = new FileStream(selectedFilePathTo, FileMode.Create, FileAccess.Write))
//                {
//                    byte[] buffer = new byte[8192]; // 8 KB buffer size
//                    int bytesRead;

//                    while ((bytesRead = sourceStream.Read(buffer, 0, buffer.Length)) > 0)
//                    {
//                        if (stopRequested)
//                        {
//                            break;
//                        }

//                        if (isPaused)
//                        {
//                            pauseEvent.WaitOne(); // Wait if paused
//                        }

//                        destinationStream.Write(buffer, 0, bytesRead);
//                        bytesCopied += bytesRead;

//                        Dispatcher.Invoke(() =>
//                        {
//                            Bar.Value = (double)bytesCopied / totalBytes * 100;
//                        });

//                        // Simulate progress for the demo
//                        Thread.Sleep(100);
//                    }
//                }
//            }
//            catch (Exception ex)
//            {
//                Dispatcher.Invoke(() =>
//                {
//                    MessageBox.Show(ex.Message);
//                });
//            }
//            finally
//            {
//                Dispatcher.Invoke(() =>
//                {
//                    if (stopRequested)
//                    {
//                        MessageBox.Show("File copy operation was canceled.");
//                    }
//                    else
//                    {
//                        MessageBox.Show("File copy completed.");
//                    }

//                    btnCopy.IsEnabled = true;
//                    btnSupsent.IsEnabled = false;
//                    btnResume.IsEnabled = false;
//                });
//            }
//        }

//        private void btnCopy_Click(object sender, RoutedEventArgs e)
//        {
//            if (string.IsNullOrEmpty(selectedFilePathFrom) || string.IsNullOrEmpty(selectedFilePathTo))
//            {
//                MessageBox.Show("Please select both source and destination files.");
//                return;
//            }

//            if (fileCopyThread != null && fileCopyThread.IsAlive)
//            {
//                MessageBox.Show("File copy is already in progress.");
//                return;
//            }

//            FileInfo fileInfo = new FileInfo(selectedFilePathFrom);
//            totalBytes = fileInfo.Length;
//            bytesCopied = 0;

//            Bar.Minimum = 0;
//            Bar.Maximum = 100;
//            Bar.Value = 0;

//            btnCopy.IsEnabled = false;
//            btnSupsent.IsEnabled = true;
//            btnResume.IsEnabled = false;
//            stopRequested = false;
//            isPaused = false;

//            fileCopyThread = new Thread(CopyFileMethod);
//            fileCopyThread.Start();
//        }

//        private void btnPause_Click(object sender, RoutedEventArgs e)
//        {
//            isPaused = true;
//            btnSupsent.IsEnabled = false;
//            btnResume.IsEnabled = true;
//        }

//        private void btnResume_Click(object sender, RoutedEventArgs e)
//        {
//            isPaused = false;
//            pauseEvent.Set(); // Resume the copy thread
//            btnResume.IsEnabled = true;
//            btnResume.IsEnabled = false;
//        }

//        private void btnCancel_Click(object sender, RoutedEventArgs e)
//        {
//            if (fileCopyThread != null && fileCopyThread.IsAlive)
//            {
//                stopRequested = true;
//                pauseEvent.Set(); // Ensure that the thread exits the pause state
//                fileCopyThread.Join(); // Wait for the thread to finish
//            }
//        }
//    }
//}
