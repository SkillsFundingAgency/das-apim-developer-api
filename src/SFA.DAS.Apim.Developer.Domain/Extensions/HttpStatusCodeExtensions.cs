using System.Net;

namespace SFA.DAS.Apim.Developer.Domain.Extensions
{
    public static class HttpStatusCodeExtensions
    {
        public static bool IsSuccessStatusCode(this HttpStatusCode statusCode)
        {
            var intStatusCode = (int) statusCode;
            return intStatusCode >= 200 && intStatusCode < 300;
        }
    }
}