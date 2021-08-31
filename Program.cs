using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Reflection;
using System.Text;

namespace Company.Function
{
  class Program
  {
    public static string getDotFile(string provider)
    {
      //Parse and get  
      List<ServiceDiagram> diagrams = new List<ServiceDiagram>();
      try
      {
        diagrams = XMLParser.parse(provider);
      }
      catch (System.Exception)
      {
        throw;
      }
      StringBuilder dotCode = new StringBuilder();
      dotCode.Append("digraph service { rankdir=LR; ranksep=1.5; a [image=\"Component.png\", shape=none, label=\"");
      dotCode.Append(provider);
      dotCode.Append("\", fontcolor=\"#0070c0\", fontname=Calibri];\n");
      foreach (var diagram in diagrams)
      {
        dotCode.Append("\n");
        dotCode.Append(diagram.serviceId);
        if (diagram.consumers.Count > 0)
        {
          dotCode.Append(" [image=\"connector.png\", shape=none, label=\"\", width=0.31, height=0.31];\n");
        }
        else
        {
          dotCode.Append(" [image=\"socket.png\", shape=none, label=\"\", width=0.31, height=0.31];\n");
        }
        for (int i = 0; i < diagram.consumers.Count; i++)
        {
          dotCode.Append("\"");
          dotCode.Append(diagram.consumers[i]);
          dotCode.Append("\" [image=\"Component.png\", shape=none, label=\"");
          dotCode.Append(diagram.consumers[i]);
          dotCode.Append("\", fontcolor=\"#0070c0\", fontsize=11, fontname=Calibri, width=0.5, height=0.25];\n");
          dotCode.Append(diagram.serviceId);
          dotCode.Append(" -> \"");
          dotCode.Append(diagram.consumers[i]);
          dotCode.Append("\" [arrowhead = none, color=\"#0070c0\", penwidth = 1.5];\n");
        }
        dotCode.Append(diagram.serviceId);
        dotCode.Append("L [label=\"");
        dotCode.Append(diagram.serviceName);
        dotCode.Append("\", shape=\"box\", style = \"filled\", fillcolor = \"#FFFFFF\", color = \"#bfbfbf\", fontcolor=\"#000000\", fontsize=11, fontname=Calibri, width=0.1, height=0.1];\n");
        dotCode.Append("a -> ");
        dotCode.Append(diagram.serviceId);
        dotCode.Append("L [arrowhead = none, color=\"#0070c0\", penwidth = 1.5];\n");
        dotCode.Append(diagram.serviceId);
        dotCode.Append("L -> ");
        dotCode.Append(diagram.serviceId);
        dotCode.Append(" [arrowhead = none, color=\"#0070c0\", penwidth = 1.5];\n");
      }
      dotCode.Append("}");

      return Run(dotCode.ToString());
    }
    public static string Run(string dot)
    {
      var binDirectory = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
      var rootDirectory = Path.GetFullPath(Path.Combine(binDirectory, "../../../../files/"));
      // var rootDirectory = Path.GetFullPath(Path.Combine(binDirectory, "../../files/"));
      string executable = @rootDirectory + "dot.exe";
      string output = @rootDirectory + "tempGraph";


      ProcessStartInfo startInfo = new ProcessStartInfo();

      File.WriteAllText(output, dot);

      System.Diagnostics.Process process = new System.Diagnostics.Process();

      // Stop the process from opening a new window
      process.StartInfo.RedirectStandardOutput = true;
      process.StartInfo.RedirectStandardError = true;
      process.StartInfo.UseShellExecute = false;
      process.StartInfo.CreateNoWindow = true;

      // Setup executable and parameters
      process.StartInfo.FileName = executable;
      process.StartInfo.Arguments = string.Format(@"{0} -Tjpg -O", output);

      // Go
      process.Start();
      // and wait dot.exe to complete and exit
      process.WaitForExit();
      byte[] graphAsByteArray = null;
      string base64String = "";
      try
      {
        graphAsByteArray = File.ReadAllBytes(output + ".jpg");
        base64String = Convert.ToBase64String(graphAsByteArray);
      }
      catch (System.Exception)
      {

        throw;
      }
      File.Delete(output);
      File.Delete(output + ".jpg");
      return base64String;
    }
  }
}
