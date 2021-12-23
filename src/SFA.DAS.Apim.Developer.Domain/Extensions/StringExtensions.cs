using Newtonsoft.Json;

namespace SFA.DAS.Apim.Developer.Domain.Extensions
{
    public static class StringExtensions
    {
        public static bool TryParseJson<T>(this string source, out T result)
        {
            var success = true;
            var settings = new JsonSerializerSettings
            {
                Error = (sender, args) => { success = false; args.ErrorContext.Handled = true; },
                MissingMemberHandling = MissingMemberHandling.Error
            };
            result = JsonConvert.DeserializeObject<T>(source, settings);
            return success;
        }
    }
}