using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tool.CodeGen
{
    partial class MainWindow
    {
        private string generateServiceHostConfigNodes(Schema c)
        {
            StringBuilder sb = new StringBuilder();
            foreach (var t in c.Tables)
            {
                if (t.Selected)
                    if (!t.isSnapshotBase)
                    {
                        sb.AppendFormat(@"
            <service  name=""RDH.ServiceHost.{0}BLLService"">
                <host>
                  <baseAddresses>
                    <add baseAddress=""http://SyncHost:9000/{0}BLLService.svc""/>
                  </baseAddresses>
                </host>
                <endpoint address="""" bindingConfiguration=""NoneSecurity""  binding=""wsHttpBinding"" contract=""RDH.Data.BLL.I{0}BLL"" />
                <endpoint address=""mex"" binding=""mexHttpBinding"" contract=""IMetadataExchange""/>
          </service>  
"
        , t.AName);
                    }

            }
            return sb.ToString();
        }

        private void BuildServiceHostConfigFile(Schema c)
        {
            StringBuilder sb1 = new StringBuilder(string.Format(@"
<system.serviceModel>
    <bindings>
      <wsHttpBinding>
        <binding name=""NoneSecurity""  maxBufferPoolSize=""12000000"" maxReceivedMessageSize=""12000000"" useDefaultWebProxy=""false"">
          <readerQuotas maxStringContentLength=""12000000"" maxArrayLength=""12000000""/>
          <security mode=""None""/>
        </binding>
      </wsHttpBinding>
    </bindings>

    <client />
    <services>
         {0}
    </services>

    <behaviors>
      <serviceBehaviors>
        <behavior>
          <serviceMetadata httpGetEnabled=""True""/>
          <serviceDebug includeExceptionDetailInFaults=""False""/>
        </behavior>
      </serviceBehaviors>
    </behaviors> 
  </system.serviceModel>
", generateServiceHostConfigNodes(c)));


            string _fileName = "serviceHost.txt";
            if (!Directory.Exists("D:\\BLLHOST"))
            {
                Directory.CreateDirectory("D:\\BLLHOST");
            }

            using (StreamWriter sw = new StreamWriter("D:\\BLLHOST" + "\\" + _fileName))
            {
                sw.Write(sb1.ToString());
            }

        }


        private string generateServiceClientConfigNodes(Schema c)
        {
            StringBuilder sb = new StringBuilder();
            foreach (var t in c.Tables)
            {
                if (t.Selected)
                    if (!t.isSnapshotBase)
                    {
                        sb.AppendFormat(@"     <endpoint address=""http://SyncHost:9000/{0}BLLService.svc""  bindingConfiguration=""NoneSecurity""  binding=""wsHttpBinding""  contract=""RDH.Data.BLL.I{0}BLL"" name=""WSHttpBinding_I{0}BLL"" /> 
"
        , t.AName);
                    }

            }
            return sb.ToString();
        }

        private void BuildServiceClientConfigFile(Schema c)
        {
            StringBuilder sb1 = new StringBuilder(string.Format(@"
<system.serviceModel>
    <bindings>
      <wsHttpBinding>
        <binding name=""NoneSecurity""  maxBufferPoolSize=""12000000"" maxReceivedMessageSize=""12000000"" useDefaultWebProxy=""false"">
          <readerQuotas maxStringContentLength=""12000000"" maxArrayLength=""12000000""/>
          <security mode=""None""/>
        </binding>
      </wsHttpBinding>
    </bindings>

    <client> 
         {0}
    </client> 

  </system.serviceModel>
", generateServiceClientConfigNodes(c)));


            string _fileName = "serviceClient.txt";
            if (!Directory.Exists("D:\\BLLHOST"))
            {
                Directory.CreateDirectory("D:\\BLLHOST");
            }

            using (StreamWriter sw = new StreamWriter("D:\\BLLHOST" + "\\" + _fileName))
            {
                sw.Write(sb1.ToString());
            }

        }
    }
}
