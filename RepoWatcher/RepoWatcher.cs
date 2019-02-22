using System.Timers;

namespace CodingCoyote
{
    /// <summary>
    /// Monitors a source control repository and raises events when new commits occur.
    /// </summary>
    public abstract class RepoWatcher
    {
        public delegate void NewCommitHandler(RepoWatcher repoWatcher, NewCommitEventArgs e);

        /// <summary>
        /// Default interval for checking repository.
        /// </summary>
        protected const double DefaultInterval = 60000;

        public Commit LastCommit;
        protected readonly Timer repoCheckTimer = new Timer();
        protected readonly object syncLock = new object();

        /// <summary>
        /// Creates a new Instance of RepoWatcher.
        /// <param name="interval">Interval to check repository (in milliseconds). Default: 60 seconds.</param>
        /// </summary>
        protected RepoWatcher(double interval = DefaultInterval)
        {
            repoCheckTimer.AutoReset = true;
            repoCheckTimer.Interval = interval;
            repoCheckTimer.Elapsed += (sender,args) => CheckRepo();
            repoCheckTimer.Elapsed += (sender, args) => CheckingForModifications?.Invoke(this, args);;
            repoCheckTimer.Enabled = true;
            repoCheckTimer.Start();
        }

        /// <summary>
        /// Interval to check repository (in milliseconds).
        /// Default: 60 seconds.
        /// </summary>
        public virtual double Interval => repoCheckTimer.Interval;

        /// <summary>
        /// Repository that is being monitored.
        /// </summary>
        public virtual string Repo { get; set; }

        /// <summary>
        /// Starts monitoring the Repo.
        /// </summary>
        public virtual void Start() => repoCheckTimer.Start();

        /// <summary>
        /// Stops monitoring the Repo.
        /// </summary>
        public virtual void Stop() => repoCheckTimer.Stop();
        
        /// <summary>
        /// Checks the repository for new commits.
        /// </summary>
        public abstract void CheckRepo();

        public event ElapsedEventHandler CheckingForModifications;

        public abstract event NewCommitHandler NewCommit;
    }
}