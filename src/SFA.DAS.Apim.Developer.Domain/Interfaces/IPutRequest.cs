namespace SFA.DAS.Apim.Developer.Domain.Interfaces
{
    public interface IPutRequest
    {
        string PostUrl { get; }
        object Data { get; set; }       
    }
}