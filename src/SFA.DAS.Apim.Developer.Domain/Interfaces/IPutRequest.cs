namespace SFA.DAS.Apim.Developer.Domain.Interfaces
{
    public interface IPutRequest
    {
        string PutUrl { get; }
        object Data { get; set; }
    }
}