using Newtonsoft.Json;
using RestSharp;
using RestSharp.Deserializers;
using RestSharp.Serializers;
using System.IO;
using System.Threading.Tasks;

namespace NTCheck
{
    public static class RestSharpUtils
    {

        public static Task<IRestResponse<T>> ExecuteAsync<T>(this RestClient client, IRestRequest request) where T : new()
        {
            var taskCompletionSource = new TaskCompletionSource<IRestResponse<T>>();
            client.ExecuteAsync<T>(request, (response) => taskCompletionSource.SetResult(response));
            return taskCompletionSource.Task;
        }

        public static Task<IRestResponse> ExecuteAsync(this RestClient client, IRestRequest request)
        {
            var taskCompletionSource = new TaskCompletionSource<IRestResponse>();
            client.ExecuteAsync(request, (response) => taskCompletionSource.SetResult(response));
            return taskCompletionSource.Task;
        }
    }


    public class DynamicJsonDeserializer : IDeserializer
    {
        public string RootElement { get; set; }
        public string Namespace { get; set; }
        public string DateFormat { get; set; }

        public T Deserialize<T>(IRestResponse response)
        {
            return JsonConvert.DeserializeObject<dynamic>(response.Content);
        }
    }

    /// <summary>
    /// Use to override the dedault serialiser in Rest sharp and uses the newtonsoft one
    /// </summary>
    public class RestSharpJsonNetSerializer : ISerializer
    {
        private readonly Newtonsoft.Json.JsonSerializer _serializer;

        public string ContentType {get;set;}
        
        public string DateFormat {get;set;}
        
        public string Namespace {get;set;}
        
        public string RootElement {get;set;}
        
        public RestSharpJsonNetSerializer(bool includeNullValues)

        {
            ContentType = "application/json";
            _serializer = new Newtonsoft.Json.JsonSerializer
            {
                MissingMemberHandling = MissingMemberHandling.Ignore,
                NullValueHandling =  includeNullValues ?  NullValueHandling.Include : NullValueHandling.Ignore,
                DefaultValueHandling = DefaultValueHandling.Include
            };
        }

        public string Serialize(object obj)
        {
            using (var stringWriter = new StringWriter())
            {
                using (var jsonTextWriter = new JsonTextWriter(stringWriter))
              {
                    jsonTextWriter.Formatting = Formatting.Indented;
                    jsonTextWriter.QuoteChar = '"';
                    _serializer.Serialize(jsonTextWriter, obj);
                    var result = stringWriter.ToString();
                    return result;

                }

            }

        }
    }
}
