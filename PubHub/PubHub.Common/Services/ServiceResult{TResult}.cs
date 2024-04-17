namespace PubHub.Common.Services
{
    public class ServiceResult<TResult> : ServiceResult
    {
        internal ServiceResult(TResult? instance, string? errorDescriptor = null) : base(errorDescriptor)
        {
            Instance = instance;
        }

        public TResult? Instance { get; }
    }
}
