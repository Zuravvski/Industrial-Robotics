using System.Threading;

namespace IDE.Common.Models
{
    /// <summary>
    /// DialogHost class
    /// </summary>
    public class DialogHost
    {
        /// <summary>
        /// The cancellation token source
        /// </summary>
        private readonly CancellationTokenSource cancellationTokenSource;

        #region Constructor

        /// <summary>
        /// Initializes a new instance of the <see cref="DialogHost"/> class.
        /// </summary>
        public DialogHost()
        {
            cancellationTokenSource = new CancellationTokenSource();
        }

        /// <summary>
        /// Cancels this instance.
        /// </summary>
        public void Cancel()
        {
            cancellationTokenSource.Cancel();
        }

        #endregion

        #region Properties

        /// <summary>
        /// Gets or sets the current action.
        /// </summary>
        /// <value>
        /// The current action.
        /// </value>
        public string CurrentAction { get; set; }
        /// <summary>
        /// Gets or sets the current progress.
        /// </summary>
        /// <value>
        /// The current progress.
        /// </value>
        public string CurrentProgress { get; set; }
        /// <summary>
        /// Gets or sets the message.
        /// </summary>
        /// <value>
        /// The message.
        /// </value>
        public string Message { get; set; }
        /// <summary>
        /// Gets the cancellation token.
        /// </summary>
        /// <value>
        /// The cancellation token.
        /// </value>
        public CancellationToken CancellationToken => cancellationTokenSource.Token;

        #endregion

    }
}
