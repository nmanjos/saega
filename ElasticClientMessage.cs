public class ElasticClientMessage
    {
        public string EndPoint { get; set; }
        public HttpVerb HttpMethod { get; set; }
        public AuthenticationType AuthType { get; set; }
        public string UserName { get; set; }
        public string UserPassword { get; set; }
        public dynamic Document { get; set; }
        public string Credentials { get; set; }
    }
