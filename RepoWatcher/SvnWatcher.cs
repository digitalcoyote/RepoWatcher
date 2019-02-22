using System;
using System.Diagnostics;
using System.IO;
using System.Text;

namespace CodingCoyote
{
    public class SvnWatcher : RepoWatcher
    {
        /// <summary>
        ///     Creates a new Instance of GitWatcher
        /// </summary>
        /// <param name="pathToRepo">Path to Git Repository or Clone URL</param>
        /// <param name="interval"> Interval (in milliseconds) used to check Git. Default: 1 Minute</param>
        public SvnWatcher(string pathToRepo, double? interval = DefaultInterval) : base(interval ?? DefaultInterval)
        {
            Repo = pathToRepo;
        }

        public SvnWatcher(string pathToRepo, string username, string password, double interval = DefaultInterval) :
            this(pathToRepo, interval)
        {
            Username = username;
            Password = password;
        }

        private string Username { get; }
        private string Password { get; }

        public override string Repo { get; set; }

        public override void CheckRepo()
        {
            lock (syncLock)
            {
                    Stop();
                    try
                    {
                        var svnCommandBuilder = new StringBuilder("log -l 1");
                        if (!string.IsNullOrWhiteSpace(Username) || Password != null)
                            svnCommandBuilder.Append(
                                $" --non-interactive --no-auth-cache --username {Username} --password {Password}");

                        svnCommandBuilder.Append($" {Repo}");
                        var lastCommitRef = string.Empty;

                        var startInfo = new ProcessStartInfo
                        {
                            FileName = "svn",
                            Arguments = svnCommandBuilder.ToString(),
                            CreateNoWindow = true,
                            UseShellExecute = false,
                            WindowStyle = ProcessWindowStyle.Hidden,
                            RedirectStandardOutput = true
                        };

                        string logMessage = string.Empty;
                        using (var svn = Process.Start(startInfo))
                        {
                            logMessage = svn.StandardOutput.ReadToEnd();
                        }

                        var latestReportedCommit = new SvnCommit(logMessage);
                        
                        if (LastCommit?.CommitSignature == null || (!string.IsNullOrWhiteSpace(latestReportedCommit?.CommitSignature) && LastCommit != latestReportedCommit))
                        {
                            LastCommit = latestReportedCommit;
                            NewCommit?.Invoke(this, new NewCommitEventArgs(LastCommit));
                        }
                    }
                    catch (Exception ex)
                    {
                        //Raise some kind of event with the error message
                        //EventLog.WriteEntry("BuildService", ex.ToString());
                    }

                    // check git repo here
                    Start();
                }
            }

        public override event NewCommitHandler NewCommit;
    }
}