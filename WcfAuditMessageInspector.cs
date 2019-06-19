public class WcfAuditMessageInspector : IClientMessageInspector, IDispatchMessageInspector
{
   private static IAppCustomConfigurationManager _AppCustomConfigurationManager;
   protected static IAppCustomConfigurationManager AppCustomConfigurationManager
   {
      get { return _AppCustomConfigurationManager ?? (_AppCustomConfigurationManager = new AppCustomConfigurationManager()); }
   }

   private IApplicationLog _applicationLog;
   protected IApplicationLog ApplicationLog
   {
      get { return (_applicationLog ?? (_applicationLog = new ApplicationLog(new Guid()))); }
   }

   public void AfterReceiveReply(ref Message reply, object correlationState)
   {
      Uri requestUri = reply.Headers.To;
      var buffer = reply.CreateBufferedCopy(Int32.MaxValue);
      reply = buffer.CreateMessage();

      Task.Run(() =>
      {
          this.LogMessage(buffer, false, "WCF", "AfterReceiveReply");
      });
   }

   public object AfterReceiveRequest(ref Message request, IClientChannel channel, InstanceContext instanceContext)
   {
      Uri requestUri = request.Headers.To;
      var buffer = request.CreateBufferedCopy(Int32.MaxValue);
      request = buffer.CreateMessage();

      Task.Run(() =>
      {
         this.LogMessage(buffer, false, $"WCF {requestUri.AbsoluteUri}", "AfterReceiveRequest");
      });
      return requestUri;
   }

   public void BeforeSendReply(ref Message reply, object correlationState)
   {
            
   }

   public object BeforeSendRequest(ref Message request, IClientChannel channel)
   {
       Uri requestUri = request.Headers.To;
       var buffer = request.CreateBufferedCopy(Int32.MaxValue);
       request = buffer.CreateMessage();

       Task.Run(() =>
       {
          this.LogMessage(buffer, false, "WCF", "BeforeSendRequest");
       });
       return null;
   }

   private void LogMessage(MessageBuffer buffer, bool isRequest, string origin, string method)
   {
       var originalMessage = buffer.CreateMessage();
       string messageContent;

       using (StringWriter stringWriter = new StringWriter())
       {
           using (XmlTextWriter xmlTextWriter = new XmlTextWriter(stringWriter))
           {
                originalMessage.WriteMessage(xmlTextWriter);
                xmlTextWriter.Flush();
                xmlTextWriter.Close();
           }
           messageContent = stringWriter.ToString();
       }

       ApplicationLog.Debug($"{origin} - {method} : " + JsonConvert.SerializeObject(messageContent));
    }
}
