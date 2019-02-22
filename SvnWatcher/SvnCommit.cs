using System;

namespace CodingCoyote
{
    public class SvnCommit : Commit
    {
        // TODO: Split Log messages into larger parts.
        public SvnCommit(string logMessage)
        {
            CommitSignature = logMessage;
        }
    }
}