using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Core.Infrastructure.Utils
{
   public class SerializerFactory
    {
       private static ISerializer _serializer = null;
       public static ISerializer GetSerializer() 
       {
           if (_serializer == null) 
           {
               _serializer = new MyDataContractSerializer();
           }
           return _serializer;
       }
    }
}
