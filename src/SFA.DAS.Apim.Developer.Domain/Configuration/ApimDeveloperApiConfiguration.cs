namespace SFA.DAS.Apim.Developer.Domain.Configuration
{
    public class ApimDeveloperApiConfiguration
    {
        public string ConnectionString { get ; set ; }
        public int NumberOfAuthFailuresToLockAccount { get; set; }
        public int AccountLockedDurationMinutes { get; set; }
    }
}