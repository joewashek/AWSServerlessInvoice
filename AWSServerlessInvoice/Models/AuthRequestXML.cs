using System.Xml;

namespace AWSServerlessInvoice.Models
{
    public static class AuthRequestXML
    {
        public static string ParseSessionId(string xml)
        {
            var xDoc = new XmlDocument();
            xDoc.LoadXml(xml);

            var sessionNode = xDoc
                .DocumentElement?
                .SelectSingleNode("/response/operation/result/data/api/sessionid");

            if (sessionNode == null)
                throw new ArgumentException("Invalid Authentication Response XML");

            return sessionNode?.InnerText ?? "";
        }
        public static string GetXML(ClientConfig config)
        {
            string requestXML = @"
                                <?xml version=""1.0"" encoding=""UTF-8""?>
                <request>
                  <control>
                    <senderid>{{sender_id}}</senderid>
                    <password>{{sender_password}}</password>
                    <controlid>authRequestId</controlid>
                    <uniqueid>false</uniqueid>
                    <dtdversion>3.0</dtdversion>
                    <includewhitespace>false</includewhitespace>
                  </control>
                  <operation>
                    <authentication>
                      <login>
                        <userid>{{user_id}}</userid>
                        <companyid>{{company_id}}</companyid>
                        <password>{{user_password}}</password>
                      </login>
                    </authentication>
                    <content>
                      <function controlid=""authFunctionId"">
                        <getAPISession />
                      </function>
                    </content>
                  </operation>
                </request>
                ";

            return requestXML
                .Replace("{{sender_id}}", config.SenderId)
                .Replace("{{sender_password}}", config.SenderPassword)
                .Replace("{{company_id}}", config.CompanyId)
                .Replace("{{user_id}}", config.UserId)
                .Replace("{{user_password}}", config.UserPassword)
                .Trim();
        }
    }
}
