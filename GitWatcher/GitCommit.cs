using System;

namespace CodingCoyote
{
    public class GitCommit : Commit
    {
        
        // TODO: Make this able to support the smaller output of git log -n 1. Probably just 
        public string CommitHash;
        public string AbbreviatedCommitHash;
        public string TreeHash;
        public string ParentHashes;
        public string AbbreviatedParentHashes;
        public string AuthorName;
        public string AuthorEmail;
        public DateTime AuthorDate;
        public string CommitterName;
        public string CommitterEmail;
        public DateTime CommitterDate;
        public string RefNames;
        public string Subject;
        public string Body;

        public GitCommit(string logMessage)
        {
            // TODO: Extend Constructor to fully populate gitCommit
            CommitHash = logMessage.Split(' ')[0];
            CommitSignature = CommitHash;
        }
        
        //TODO: Alternate Constructors using Other data.
    }
}