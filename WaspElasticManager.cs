public static class WaspElasticManager
    {
        public static log4net.ILog logger = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        public static bool LogEvent(LogDocumentElasticJson loggingEvent)
        {            
            try
            {
                logger.Debug($"Input LogEvent - LogDocumentElasticJson: {JsonConvert.SerializeObject(loggingEvent)} ");

                if (String.IsNullOrEmpty(Convert.ToString(loggingEvent.MessageJson)))                
                    loggingEvent.MessageJson = null;

                var inputInJson = JsonConvert.SerializeObject(loggingEvent);
                dynamic inputJson = JsonConvert.DeserializeObject<dynamic>(inputInJson);                                
                // Call Service
                var result = CallLog(inputJson);

                logger.Debug($"Result LogEvent - LogDocumentElasticJson: {JsonConvert.SerializeObject(result)} ");

                return ((int)result.StatusCode) == 200 ? true : false;
            }
            catch (Exception exc)
            {
                logger.Error($"Input LogEvent - LogDocumentElasticJson: {JsonConvert.SerializeObject(loggingEvent)} ");
                logger.Error(exc);
                return false;
            }
        }

        public static bool LogEvent(LogDocumentElasticXml loggingEvent)
        {
            try
            {
                logger.Debug($"Input LogEvent - LogDocumentElasticXml: {JsonConvert.SerializeObject(loggingEvent)} ");

                if (String.IsNullOrEmpty(Convert.ToString(loggingEvent.MessageJson)))
                    loggingEvent.MessageJson = null;

                dynamic input = JsonConvert.DeserializeObject<dynamic>(loggingEvent.MessageJson);
                loggingEvent.MessageJson = string.Empty;

                var inputInJson = JsonConvert.SerializeObject(loggingEvent);
                dynamic inputJson = JsonConvert.DeserializeObject<dynamic>(inputInJson);
                inputJson.MessageJson = input;
                
                // Call Service
                var result = CallLog(inputJson);

                logger.Debug($"Result LogEvent - LogDocumentElasticXml: {JsonConvert.SerializeObject(result)} ");

                return ((int)result.StatusCode) == 200 ? true : false;
            }
            catch (Exception exc)
            {
                logger.Error($"Input LogEvent - LogDocumentElasticXml: {JsonConvert.SerializeObject(loggingEvent)} ");
                logger.Error(exc);
                return false;
            }
        }

        private static HttpResponseMessage CallLog(dynamic log)
        {
            DateTime time = DateTime.UtcNow;

            if (time.DayOfWeek >= DayOfWeek.Monday && time.DayOfWeek <= DayOfWeek.Wednesday)
                time = time.AddDays(3);

            int Week = CultureInfo.InvariantCulture.Calendar.GetWeekOfYear(time, CalendarWeekRule.FirstFourDayWeek, DayOfWeek.Monday);
            int Year = time.Year;

            var source = ((string)log.Source).ToLower().Replace(" ", "");
            //string IndexName = $"{ConfigurationManager.AppSettings["ElasticIndex"]}-{source}-{Year}.{Week}/{ConfigurationManager.AppSettings["ElasticType"]}/";

            ElasticClientMessage clientMessage = new ElasticClientMessage();
            clientMessage.EndPoint = $"{ConfigurationManager.AppSettings["ElasticEndpoint"]}/{source}";
            clientMessage.HttpMethod = HttpVerb.POST;
            clientMessage.AuthType = AuthenticationType.Basic;
            clientMessage.Credentials = System.Convert.ToBase64String(System.Text.Encoding.GetEncoding("ISO-8859-1").GetBytes(ConfigurationManager.AppSettings["ElasticUser"] + ":" + ConfigurationManager.AppSettings["ElasticPassword"]));

            clientMessage.Document = log;

            logger.Error($"Request Message: {JsonConvert.SerializeObject(clientMessage)} ");
            return WaspElasticHelper.LogEvent(clientMessage).Result;

        }
    }
