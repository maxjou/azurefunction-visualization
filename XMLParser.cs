using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Xml;
namespace Company.Function
{
  internal class XMLParser
  {
    internal static List<ServiceDiagram> parse(string provider)
    {
      XmlDocument doc = new XmlDocument();
      doc.PreserveWhitespace = false;
      var binDirectory = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
      // var rootDirectory = Path.GetFullPath(Path.Combine(binDirectory, "../../../../files/"));
      var rootDirectory = Path.GetFullPath(Path.Combine(binDirectory, "../../files/"));
      doc.Load(rootDirectory + "/service_list.xml");
      XmlNodeList elemList = doc.GetElementsByTagName("entry");
      List<ServiceDiagram> diagrams = new List<ServiceDiagram>();
      for (int i = 0; i < elemList.Count; i++)
      {
        XmlNode node = elemList[i];
        if (node.GetType() == typeof(XmlElement))
        {
          XmlElement element = (XmlElement)node;

          if (String.Equals(element.GetElementsByTagName("d:Provider_x0020_Lookup")[0].InnerText, provider))
          {
            ServiceDiagram diagram = new ServiceDiagram();
            string srvId = element.GetElementsByTagName("d:Service_x0020_ID")[0].InnerText;
            string srvName = element.GetElementsByTagName("d:Title")[0].InnerText;
            string sharepointId = element.GetElementsByTagName("d:Id")[0].InnerText;
            diagram.serviceId = srvId;
            diagram.serviceName = srvName;
            Console.WriteLine(diagram.serviceName);
            Console.WriteLine($"---------------------------------------\nService Id: {srvId}\nService Name: {srvName}\nSharepoint Id: {sharepointId}\n\n");
            XmlDocument consumerFile = new XmlDocument();
            consumerFile.Load(rootDirectory + "/contract_list.xml");
            XmlNodeList consumerList = consumerFile.GetElementsByTagName("entry");
            List<string> theConsumers = new List<string>();
            for (int j = 0; j < consumerList.Count; j++)
            {
              XmlNode cNode = consumerList[j];
              if (cNode.GetType() == typeof(XmlElement))
              {
                XmlElement cElement = (XmlElement)cNode;

                if (String.Equals(cElement.GetElementsByTagName("d:Service_x0020_IDId")[0].InnerText, sharepointId))
                {
                  string consumer = cElement.GetElementsByTagName("d:Consumer_x0020_Lookup")[0].InnerText;
                  string conId = cElement.GetElementsByTagName("d:Title")[0].InnerText;
                  theConsumers.Add(consumer);
                  Console.WriteLine($"    Consumer: {consumer}\n    Con Id: {conId}\n\n");
                }
              }
            }
            diagram.consumers = theConsumers;
            diagrams.Add(diagram);
          }
        }
      }
      return diagrams;
    }
  }
}