using System;

namespace CommenceProcessMonitor
{
    public class CommenceDatabaseChangedArgs : EventArgs
    {
        public CommenceDatabaseChangedArgs(string name)
        {
            _name = name;
        }
        private string _name;
        public string DatabaseName
        {
            get { return _name; }
        }
    }
}
