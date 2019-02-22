using System;

namespace CodingCoyote
{
    public class SvnCommit : Commit
    {
        public SvnCommit(string logMessage)
        {
            CommitSignature = logMessage;
        }
    }
}