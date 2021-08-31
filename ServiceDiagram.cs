using System.Collections.Generic;

namespace Company.Function
{
  internal class ServiceDiagram
  {

    public string serviceName { get; set; }
    public string serviceId { get; set; }
    public List<string> consumers { get; set; }
  }
}