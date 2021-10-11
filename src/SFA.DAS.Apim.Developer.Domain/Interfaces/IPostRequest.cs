namespace SFA.DAS.Apim.Developer.Domain.Interfaces
{
    public interface IPostRequest
    {
        string PostUrl { get; }
        object Data { get; set; }
    }
}