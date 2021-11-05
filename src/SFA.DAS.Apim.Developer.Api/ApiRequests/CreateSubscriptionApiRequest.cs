namespace SFA.DAS.Apim.Developer.Api.ApiRequests
{
    public class CreateSubscriptionApiRequest
    {
        public string AccountIdentifier { get; set; }
        public string ProductId { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string EmailAddress { get; set; }
    }
}