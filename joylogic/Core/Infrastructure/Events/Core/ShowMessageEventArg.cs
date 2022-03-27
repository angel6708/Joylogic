using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace Core.Infrastructure.Events
{

    public enum MessageType 
    {
        Warnning,Info,Error

    }
    public class ShowMessageEventArg
    {
        public string Message { get; set; }
        public Action OkCommandHandler { get; set; }
        public Action CancelCommandHandler { get; set; } 
        public TimeSpan During { get; set; }
        public MessageType MessageType { get; set; }
    }
}
