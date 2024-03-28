namespace PubHub.Common.Models
{
    public abstract class PaginationQuery : SearchQuery
    {
        public virtual bool Descending { get; set; }
        /// <summary>
        /// The maximum amount of entries to return
        /// </summary>
        public virtual int? Max { get; set; }
        private int _currentPage = 1;
        /// <summary>
        /// This can never be less than 1. If the recieved number is less; this is incremented by 1
        /// </summary>
        public virtual int Page
        {
            get
            {
                return _currentPage;
            }
            set
            {
                _currentPage = ((value < 1) ? (1) : (value));
            }
        }
    }
}
