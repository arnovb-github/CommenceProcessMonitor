using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace CommenceProcessMonitor
{
    public class ProcessMonitor
    {
        public event EventHandler<CommenceDatabaseChangedArgs> CommenceDatabaseChanged;

        public static readonly string commenceProcessName = "commence";
        private readonly uint _interval;
        System.Timers.Timer _timer;
        // everything in the window title after the prefix should represent the database name
        // we explicitly do not want to query the Commence database for it
        // because we cannot reference a 3-rd party library in a shared project.
        private readonly List<string> windowTitlePrefixes = new List<string>() { "Commence - ", "Commence/CLIENT - ", "Commence/SERVER - " };

        public ProcessMonitor(int checkMilliSeconds = 1000)
        {
            _interval = (uint)Math.Abs(checkMilliSeconds);
            _currentDatabaseName = GetCurrentDatabaseName();
            // Create a timer and set a two second interval.
            _timer = new System.Timers.Timer();
            _timer.Interval = _interval;
            // Hook up the Elapsed event for the timer. 
            _timer.Elapsed += OnTimerElapsedEvent;
            // Have the timer fire repeated events (true is the default)
            _timer.AutoReset = true;
            // Start the timer
            _timer.Enabled = true;
        }

        private string _currentDatabaseName;

        public string CurrentDatabaseName
        {
            get { return _currentDatabaseName; }
            private set { _currentDatabaseName = value; }
        }

        private IEnumerable<Process> GetCommenceProcessesCurrentSession(string processName)
        {
            Process[] p = Process.GetProcessesByName(processName);
            int currentSessionID = Process.GetCurrentProcess().SessionId;
            Process[] sameAsthisSession = (from c in p where c.SessionId == currentSessionID select c).ToArray();
            return sameAsthisSession;

        }

        private string GetCurrentDatabaseName()
        {
            string retval = string.Empty;
            IEnumerable<Process> p = GetCommenceProcessesCurrentSession(commenceProcessName);
            if (p.Count() == 1)
            {
                string windowTitle = p.First().MainWindowTitle;
                foreach (string s in windowTitlePrefixes)
                {
                    if (windowTitle.StartsWith(s))
                    {
                        retval = windowTitle.Substring(s.Length);
                        break;
                    }
                }
            }
            return retval;
        }

        private void OnTimerElapsedEvent(object sender, System.Timers.ElapsedEventArgs e)
        {
            string tempName = GetCurrentDatabaseName();
            if (!_currentDatabaseName.Equals(tempName))
            {
                _currentDatabaseName = tempName;
                CommenceDatabaseChanged?.Invoke(this, new CommenceDatabaseChangedArgs(_currentDatabaseName));
            }
        }
    }
}
