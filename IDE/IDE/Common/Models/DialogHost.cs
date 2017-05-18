using System.Threading;

namespace IDE.Common.Models
{
    public class DialogHost
    {
        private readonly CancellationTokenSource cancellationTokenSource;

        #region Constructor

        public DialogHost()
        {
            cancellationTokenSource = new CancellationTokenSource();
        }

        public void Cancel()
        {
            cancellationTokenSource.Cancel();
        }

        #endregion

        #region Properties

        public string CurrentAction { get; set; }
        public string CurrentProgress { get; set; }
        public string Message { get; set; }
        public CancellationToken CancellationToken => cancellationTokenSource.Token;

        #endregion

    }
}
