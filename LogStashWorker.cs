public static class LogStashWorker
    {
        public static log4net.ILog log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        public static async Task<HttpResponseMessage> LogEvent(ElasticClientMessage elasticMessage)
        {
            HttpResponseMessage res = null;
            using (var client = new HttpClient())
            using (var request = new HttpRequestMessage(HttpMethod.Post, elasticMessage.EndPoint))
            {
                string json = JsonConvert.SerializeObject(elasticMessage.Document);
                using (HttpContent stringContent = new StringContent(json, Encoding.UTF8, "application/json"))
                {
                    request.Content = stringContent;
                    client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic", elasticMessage.Credentials);
                    stringContent.Headers.ContentType = new MediaTypeHeaderValue("application/json");
                    using (var response = await client
                        .SendAsync(request, HttpCompletionOption.ResponseHeadersRead)
                        .ConfigureAwait(false))
                    {
                        response.EnsureSuccessStatusCode();
                        res = response;
                    }
                }
            }


            return res;
        }
    }
