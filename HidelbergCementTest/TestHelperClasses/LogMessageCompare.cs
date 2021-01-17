using HeidelbergCement.Model;
using System;
using System.Collections.Generic;
using System.Text;

namespace HidelbergCementTest.TestHelperClasses
{
    internal class LogMessageCompare : IEqualityComparer<LogMessage>
    {
        public bool Equals(LogMessage x, LogMessage y)
        {
            if (x.Text == y.Text && x.Title == x.Title)
            {
                return true;
            }
            else { return false; }
        }
        public int GetHashCode(LogMessage codeh)
        {
            return codeh.GetHashCode();
        }
    }
}
