using System.ComponentModel.DataAnnotations;
using System.Reflection;
using System.Text;
using System.Web;
using Microsoft.AspNetCore.JsonPatch;
using MockDoor.Client.Models;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace MockDoor.Client.Helpers
{
    public static class HelperMethodsExtensions
    {
        public static string SafePrint(this string name, string defaultValue = null)
        {
            if (string.IsNullOrWhiteSpace(defaultValue))
                defaultValue = "[Not Set]";

            return string.IsNullOrWhiteSpace(name) ? defaultValue : name;
        }

        public static decimal? GetPreciseMilliseconds(this DateTime? source, int decimalPlaces)
        {
            if (source == null)
                return null;

            decimal milliseconds = GetMillisecondsAsTicks(source.Value) / (decimal)TimeSpan.TicksPerMillisecond;

            return Math.Round(milliseconds, decimalPlaces);
        }

        public static long GetMillisecondsAsTicks(this DateTime source)
        {
            var currentTicksExcludingMilliseconds = new DateTime(source.Year, source.Month, source.Day, source.Hour, source.Minute, source.Second);

            var ticksRemaining = source.Ticks - currentTicksExcludingMilliseconds.Ticks;

            return ticksRemaining;
        }

        public static string MakePretty(this string value, bool replaceNullWithEmpty = true)
        {
            if (string.IsNullOrWhiteSpace(value))
            {
                if (replaceNullWithEmpty)
                    return string.Empty;
                else
                    return value;
            }

            try
            {
                string pretty = JToken.Parse(value).ToString(Formatting.Indented);

                return pretty;
            }
            catch (Exception)
            {
                return value;
            }
        }

        public static async Task<T> GetContentAsync<T>(this HttpResponseMessage httpResponseMessage)
        {
            return await GetContentAsync<T>(httpResponseMessage.Content);
        }

        public static async Task<T> GetContentAsync<T>(this HttpContent content)
        {
            try
            {
                var responseContentStream = await content.ReadAsStringAsync();

                return JsonConvert.DeserializeObject<T>(responseContentStream, new JsonSerializerSettings() { MissingMemberHandling = MissingMemberHandling.Ignore, NullValueHandling = NullValueHandling.Ignore });
            }
            catch (Exception)
            {
                return default;
            }
        }

        public static async Task<string> GetContentAsync(this HttpResponseMessage httpResponseMessage)
        {
            return await GetContentAsync(httpResponseMessage.Content);
        }

        public static async Task<string> GetContentAsync(this HttpContent content)
        {
            try
            {
                return await content.ReadAsStringAsync();
            }
            catch (Exception ex)
            {
                return ex.Message;
            }
        }

        public static async Task<HttpResponseMessage> PatchAsync<T>(this HttpClient client, string requestUri, JsonPatchDocument<T> patchDocument, CancellationTokenSource cancellationTokenSource = null)
        where T : class
        {
            var json = JsonConvert.SerializeObject(patchDocument.Operations);
            var content = new StringContent(json, Encoding.UTF8, "application/json-patch+json");

            if (cancellationTokenSource != null)
            {
                return await client.PatchAsync(requestUri, content, cancellationTokenSource.Token);
            }
            return await client.PatchAsync(requestUri, content);
        }

        private static string GetDisplayDescription(this Enum enumValue)
        {
            var val = enumValue.GetType().GetMember(enumValue.ToString()).FirstOrDefault();

            if (val is null)
                return enumValue.ToString();

            var attribute = val.GetCustomAttribute<DisplayAttribute>();
            if (attribute != null)
                return attribute.GetDescription();

            return enumValue.ToString();
        }

        public static IEnumerable<EnumItem<T>> GetEnumList<T>(bool includeAll = false) where T : struct, Enum
        {
            var items = new List<EnumItem<T>>();
            if (includeAll)
            {
                items.Add(new EnumItem<T>() { EnumValue = null, EnumName = $"Any" });
            }

            var values = Enum.GetValues(typeof(T)).Cast<T>().ToList();
            foreach (var val in values.GroupBy(v => Convert.ToInt32(v)))
            {
                var description = $"{string.Join('/', val.Select(v => v.GetDisplayDescription()))} ({val.Key})";

                items.Add(new EnumItem<T>{ EnumValue = val.First(), EnumName = description });
            }

            return items;
        }
        
        public static string RemoveQueryString(this string url, string key)
        {
            var uri = new Uri(url);

            // this gets all the query string key value pairs as a collection
            var newQueryString = HttpUtility.ParseQueryString(uri.Query);

            // this removes the key if exists
            newQueryString.Remove(key);

            // this gets the page path from root without QueryString
            string pagePathWithoutQueryString = uri.GetLeftPart(UriPartial.Path);

            return newQueryString.Count > 0
                ? String.Format("{0}?{1}", pagePathWithoutQueryString, newQueryString)
                : pagePathWithoutQueryString;
        }
    }
}
