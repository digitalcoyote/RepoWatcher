using System;

namespace CodingCoyote
{
    public class NewCommitEventArgs : EventArgs
    {
        public Commit Commit;

        public NewCommitEventArgs(Commit commit)
        {
            Commit = commit;
        }
    }
}