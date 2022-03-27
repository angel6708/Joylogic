using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Core.Infrastructure.Utils
{
    public interface ISerializer
    {
        
        object DeserializeModel(byte[] msg);
        byte[] SerializeModel(object model);
        object DeepCopy(object from);
    }
}
