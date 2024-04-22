using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Text;

namespace PubHub.Common.Services
{
    public class PollyInfoService : INotifyPropertyChanged
    {
        private const string FETCH_MESSAGE = "Fetching data...";
        private const string SEND_MESSAGE = "Sending data...";
        private const string REQUEST_MESSAGE = "Sending request...";

        private int _lastRetryAttempt = -1;

        private string _detail = string.Empty;
        private string _errorText = string.Empty;

        public event PropertyChangedEventHandler? PropertyChanged;

        public enum ActionType
        {
            NONE,
            GET,
            POST,
            PUT,
            DELETE,
            OTHER
        }

        public ActionType CurrentActionType { get; private set; } = ActionType.NONE;
        public string RetryString { get; private set; } = string.Empty;
        public string Message { get; private set; } = string.Empty;
        public string Detail
        {
            get => _detail;
            set
            {
                if (value != _detail)
                {
                    _detail = value;
                    NotifyPropertyChanged();
                }
            }
        }
        public string ErrorText
        {
            get => _errorText;
            set
            {
                if (value != _errorText)
                {
                    _errorText = value;
                    NotifyPropertyChanged();
                }
            }
        }

        /// <summary>
        /// Set the current action type to display an appropriate info message.
        /// </summary>
        /// <param name="actionType">Request type.</param>
        public void SetActionType(ActionType actionType)
        {
            CurrentActionType = actionType;

            // Set message based on action type.
            Message = CurrentActionType switch
            {
                ActionType.NONE => string.Empty,
                ActionType.GET => FETCH_MESSAGE,
                ActionType.POST => SEND_MESSAGE,
                ActionType.PUT => SEND_MESSAGE,
                _ => REQUEST_MESSAGE
            };
            NotifyPropertyChanged();
        }

        /// <summary>
        /// Set the indicator for a given retry attempt out of total number of retry attempts.
        /// </summary>
        /// <param name="retryAttempt">Current retry attempt.</param>
        /// <param name="totalAttempts">Total max retry attempts.</param>
        public void SetRetryIndicator(int retryAttempt, int totalAttempts)
        {
            // Update retry counter.
            if (_lastRetryAttempt == retryAttempt && !string.IsNullOrEmpty(RetryString))
                return;

            StringBuilder sb = new();
            for (int i = 0; i < retryAttempt; i++)
                sb.Append('|');
            for (int i = retryAttempt; i < totalAttempts; i++)
                sb.Append('·');
            RetryString = sb.ToString();
            _lastRetryAttempt = retryAttempt;

            NotifyPropertyChanged();
        }

        /// <summary>
        /// Indicate that the current action is over.
        /// </summary>
        public void Stop()
        {
            CurrentActionType = ActionType.NONE;
            RetryString = string.Empty;
            Message = string.Empty;
            _detail = string.Empty;

            NotifyPropertyChanged();
        }

        /// <summary>
        /// Remove all states and messages.
        /// </summary>
        public void Reset()
        {
            Stop();
            _errorText = string.Empty;

            NotifyPropertyChanged();
        }

        /// <summary>
        /// Notify if one or all properties of <see cref="PollyInfoService"/> has changed.
        /// </summary>
        /// <param name="propertyName">Name of changed property. Use <see langword="null"/> to indicate that multiple properties has changed.</param>
        protected virtual void NotifyPropertyChanged([CallerMemberName] string? propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
