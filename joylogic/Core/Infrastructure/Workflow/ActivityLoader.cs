using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace Core.Infrastructure.Workflow
{
    public class ActivityLoader
    {

        public static Activity Load(string fileName)
        {
            StreamReader reader = new StreamReader(Path.Combine("Workflow", fileName + ".xml"), Encoding.UTF8);
            var xml = reader.ReadToEnd();
            reader.Close();

            NetDataContractSerializer serializer = new NetDataContractSerializer();
            MemoryStream ms = new MemoryStream();
            StreamWriter writer = new StreamWriter(ms);
            writer.Write(xml);
            writer.Flush();
            ms.Flush();
            ms.Seek(0, SeekOrigin.Begin);
            Object obj = serializer.ReadObject(ms);
            writer.Close();

            var activiy = obj as ComposedActivity;
            activiy.Init();
            //activiy.UpdateEvents();
            activiy.Config();

            return activiy;
        }

    }
}
