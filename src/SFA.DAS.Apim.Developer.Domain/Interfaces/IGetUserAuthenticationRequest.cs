namespace SFA.DAS.Apim.Developer.Domain.Interfaces
{
    public interface IGetUserAuthenticationRequest : IGetRequest
    {
        string AuthorizationHeaderValue { get; }
    }
}