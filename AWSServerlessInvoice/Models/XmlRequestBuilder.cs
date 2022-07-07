using System.Xml;

namespace AWSServerlessInvoice.Models
{
    public class XmlRequestBuilder
    {
        private XmlDocument _xmlDocument;
        private bool _hasFunction;
        public XmlRequestBuilder()
        {
            _xmlDocument = new XmlDocument();
            var root = _xmlDocument.CreateElement("request");
            _xmlDocument.AppendChild(root);
        }
        public XmlRequestBuilder WithSenderIdControlOnly(
            string senderId,
            string senderPassword,
            string controlId)
        {
            var root = _xmlDocument.FirstChild;
            var controlEle = _xmlDocument.CreateElement("control");

            AppendWithValue(controlEle, "senderid", senderId);
            AppendWithValue(controlEle, "password", senderPassword);
            AppendWithValue(controlEle, "controlid", controlId);
            AppendWithValue(controlEle, "uniqueid", "false");
            AppendWithValue(controlEle, "dtdversion", "3.0");
            AppendWithValue(controlEle, "includewhitespace", "false");

            root?.AppendChild(controlEle);
            return this;
        }
        public XmlRequestBuilder WithReadMore(string sessionId, string resultId)
        {
            if (_hasFunction)
                throw new ArgumentException("Cannot include multiple read functions");

            var queryEle = _xmlDocument.CreateElement("readMore");
            AppendWithValue(queryEle, "resultId", resultId);

            CreateFunctionOperation(sessionId, queryEle);

            _hasFunction = true;

            return this;
        }
        public XmlRequestBuilder WithReadByQuery(string sessionId, ReadByQueryParameters parameters)
        {
            if (_hasFunction)
                throw new ArgumentException("Cannot include multiple read functions");

            var queryEle = _xmlDocument.CreateElement("readByQuery");
            AppendWithValue(queryEle, "object", parameters.Object);
            AppendWithValue(queryEle, "fields", parameters.Fields);
            AppendWithValue(queryEle, "query", parameters.Query);
            AppendWithValue(queryEle, "pagesize", parameters.PageSize.ToString());

            CreateFunctionOperation(sessionId, queryEle);

            _hasFunction = true;

            return this;
        }
        public XmlDocument Build()
        {
            return _xmlDocument;
        }
        private void CreateFunctionOperation(string sessionId, XmlElement functionChildElement)
        {
            var root = _xmlDocument.FirstChild;
            var operationEle = _xmlDocument.CreateElement("operation");

            var authEle = _xmlDocument.CreateElement("authentication");
            AppendWithValue(authEle, "sessionid", sessionId);
            operationEle.AppendChild(authEle);

            var contentEle = _xmlDocument.CreateElement("content");

            var functionEle = _xmlDocument.CreateElement("function");
            functionEle.SetAttribute("controlid", "functionId");

            functionEle.AppendChild(functionChildElement);
            contentEle.AppendChild(functionEle);
            operationEle.AppendChild(contentEle);

            root?.AppendChild(operationEle);
        }
        private void AppendWithValue(XmlElement parent, string childName, string childValue)
        {
            var childEle = _xmlDocument.CreateElement(childName);
            childEle.InnerText = childValue;
            parent.AppendChild(childEle);
        }
    }
}
