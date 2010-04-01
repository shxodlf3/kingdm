using System;
using System.IO;
using System.Threading;

using KingsDamageMeter.Properties;

namespace KingsDamageMeter
{
    public class LogReader
    {
        private StreamReader _StreamReader;
        private Thread _Worker;
        private object _LockObject = new object();
        private string _DebugLogPath = Settings.Default.DebugFile;

        public bool Running
        {
            get;
            private set;
        }

        public string Filename
        {
            get;
            private set;
        }

        public event ReadEventHandler DataRead;
        public event EventHandler Starting;
        public event EventHandler Started;
        public event EventHandler Stopping;
        public event EventHandler Stopped;
        public event EventHandler FileNotFound;
        public event EventHandler UnableToOpen;
        public event EventHandler AlreadyRunning;
        public event EventHandler NotRunning;

        public void Open(string filename)
        {
            if (Running)
            {
                if (AlreadyRunning != null)
                {
                    AlreadyRunning(this, EventArgs.Empty);
                }
                return;
            }

            if (Starting != null)
            {
                Starting(this, EventArgs.Empty);
            }

            Filename = filename;

            if ((_StreamReader = new StreamReader(GetFileStream())) != null)
            {
                Running = true;
                StartWorker();

                if (Started != null)
                {
                    Started(this, EventArgs.Empty);
                }
                DebugLogger.Write("Log parser initialized: \"" + _DebugLogPath + "\"");
            }
            else
            {
                if (Stopped != null)
                {
                    Stopped(this, EventArgs.Empty);
                }
            }
        }

        public void Close()
        {
            if (!Running)
            {
                if (NotRunning != null)
                {
                    NotRunning(this, EventArgs.Empty);
                }
                return;
            }

            if (Stopping != null)
            {
                Stopping(this, EventArgs.Empty);
            }

            Running = false;
            _Worker.Abort();
            _StreamReader.Close();

            if (Stopped != null)
            {
                Stopped(this, EventArgs.Empty);
            }
            DebugLogger.Write("Log parser stopped.");
        }

        private FileStream GetFileStream()
        {
            FileStream stream = null;

            try
            {
                stream = File.Open(Filename, FileMode.Open, FileAccess.Read, FileShare.ReadWrite);
                stream.Position = stream.Length;
            }
            catch (Exception e)
            {
                if (e is FileNotFoundException)
                {
                    if (FileNotFound != null)
                    {
                        FileNotFound(this, EventArgs.Empty);
                    }
                }
                else
                {
                    if (UnableToOpen != null)
                    {
                        UnableToOpen(this, EventArgs.Empty);
                    }
                }
            }
            return stream;
        }

        private void StartWorker()
        {
            _Worker = new Thread
            (
                delegate()
                {
                    lock (_LockObject)
                    {
                        while (Running)
                        {
                            string data = _StreamReader.ReadLine();

                            if (DataRead != null)
                            {
                                DataRead(this, new ReadEventArgs(data));
                            }
                            
                            Thread.Sleep(1);
                        }
                    }
                }
            );
            _Worker.IsBackground = true;
            _Worker.Start();
        }
    }
}
