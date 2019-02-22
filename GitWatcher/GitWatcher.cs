using System;
using System.Diagnostics;
using System.IO;

namespace CodingCoyote
{
    public class GitWatcher : RepoWatcher
    {
        private string _branch;

        private string _statusRepo;

        /// <summary>
        ///     Creates a new Instance of GitWatcher
        /// </summary>
        /// <param name="pathToRepo">Path to Git Repository or Clone URL</param>
        /// <param name="interval"> Interval (in milliseconds) used to check Git. Default: 1 Minute</param>
        public GitWatcher(string pathToRepo, double interval = DefaultInterval) : base(interval)
        {
            Repo = pathToRepo;
        }

        /// <summary>
        ///     Creates a new Instance of GitWatcher
        /// </summary>
        /// <param name="pathToRepo"> Path to Git Repository or Clone URL</param>
        /// <param name="branch"> Branch to track</param>
        /// <param name="interval"> Interval (in milliseconds) used to check Git. Default: 1 Minute</param>
        public GitWatcher(string pathToRepo, string branch, double? interval) : this(pathToRepo,
            interval ?? DefaultInterval)
        {
            Branch = branch;
        }

        /// <summary>
        ///     Creates a new Instance of GitWatcher
        /// </summary>
        /// <param name="pathToRepo"> Path to Git Repository or Clone URL</param>
        /// <param name="branch"> Branch to track</param>
        /// <param name="username"> User for git repository</param>
        /// <param name="password"> Password for git repository</param>
        /// <param name="interval"> Interval (in milliseconds) used to check Git. Default: 1 Minute</param>
        public GitWatcher(string pathToRepo, string branch, string username, string password,
            double? interval = DefaultInterval) : this(pathToRepo, branch, interval ?? DefaultInterval)
        {
            Username = username;
            Password = password;
        }

        public GitWatcher(string branch, string statusRepo, double interval = DefaultInterval) : base(interval)
        {
            _branch = branch;
            _statusRepo = statusRepo;
        }

        private string Username { get; }
        private string Password { get; }


        public override string Repo
        {
            get
            {
                string _repo;
                if (!string.IsNullOrWhiteSpace(Username) && !string.IsNullOrWhiteSpace(Password))
                    _repo = _statusRepo.Substring(0, _statusRepo.IndexOf("://") + 3) + Username + ":" + Password +
                            "@" + _statusRepo.Substring(_statusRepo.IndexOf("://") + 3);
                else
                    _repo = _statusRepo;

                return _repo;
            }
            set => _statusRepo = value;
        }

        public string Branch
        {
            get => string.IsNullOrWhiteSpace(_branch) ? "HEAD" : _branch;
            set => _branch = value;
        }

        public override void CheckRepo()
        {
            lock (syncLock)
            {
                    Stop();
                    // TODO: Implement separate logic for when no previous commit exists and when a date is provided. Perhaps git-rev-list with --since.
                    try
                    {
                        var gitCommand = $"ls-remote {Repo} --refs {Branch}";
                        var logMessage = string.Empty;

                        var startInfo = new ProcessStartInfo
                        {
                            FileName = "git",
                            Arguments = gitCommand,
                            CreateNoWindow = true,
                            UseShellExecute = false,
                            WindowStyle = ProcessWindowStyle.Hidden,
                            RedirectStandardOutput = true
                        };

                        using (var git = Process.Start(startInfo))
                        {
                            logMessage = git.StandardOutput.ReadToEnd();
                        }

                        var latestReportedCommit = new GitCommit(logMessage);
                        
                        if (LastCommit?.CommitSignature == null || (!string.IsNullOrWhiteSpace(latestReportedCommit?.CommitSignature) && LastCommit != latestReportedCommit))
                        {
                            LastCommit = latestReportedCommit;
                            NewCommit?.Invoke(this, new NewCommitEventArgs(LastCommit));
                        }
                        else
                        {
                            //TODO: Some kind of error Event

                            //EventLog.WriteEntry("GitWatcher",
                                //$"Repo: {_statusRepo}{Environment.NewLine}No New Commit!");
                        }

                        //EventLog.WriteEntry("GitWatcher",
                          //  $"Repo: {_statusRepo}{Environment.NewLine} LastCommit: {logMessage}");
                    }
                    catch (Exception ex)
                    {
                        //TODO: Some kind of error Event
                        //EventLog.WriteEntry("GitWatcher",
                          //  $"Something went wrong with Repo: {_statusRepo}{Environment.NewLine}Error: {ex}");
                    }

                    // check git repo here
                    Start();
                }
            }

        public override event NewCommitHandler NewCommit;
    }
}