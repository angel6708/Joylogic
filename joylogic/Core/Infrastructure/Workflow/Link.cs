using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace Core.Infrastructure.Workflow
{
    [DataContract]
    public class Link : INotifyPropertyChanged
    {
        [Browsable(false)]
        [DataMember]
        public Activity Source { get; private set; }
        [DataMember]
        [Browsable(false)]
        public PortKinds SourcePort { get; private set; }
        [Browsable(false)]
        [DataMember]
        public Activity Target { get; private set; }
        [Browsable(false)]
        [DataMember]
        public PortKinds TargetPort { get; private set; }

        private string _text;
        [DataMember]
        public string Text
        {
            get { return _text; }
            set
            {
                _text = value;
                OnPropertyChanged("Text");
            }
        }

        public Link(Activity source, PortKinds sourcePort, Activity target, PortKinds targetPort)
        {
            Source = source;
            SourcePort = sourcePort;
            Target = target;
            TargetPort = targetPort;
        }

        #region INotifyPropertyChanged Members

        public event PropertyChangedEventHandler PropertyChanged;

        protected void OnPropertyChanged(string name)
        {
            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(name));
        }

        #endregion


        public static Link CreateLink(Activity start, Activity end, bool cancel = false)
        {
            PortKinds startKind = PortKinds.BottomLeft;
            if (cancel) { startKind = PortKinds.BottomRight; }

            if (start is StartActivity) { startKind = PortKinds.Bottom; }

            PortKinds endKind = PortKinds.Top;

            if (end is EndActivity)
            {
                if (cancel)
                {
                    endKind = PortKinds.TopRight;
                }
                else
                {
                    endKind = PortKinds.TopLeft;
                }
            }

            var link = new Link(start, startKind, end, endKind);
            return link;
        }
    }

    [DataContract]
    public enum PortKinds
    {
        [EnumMember]
        Top,
        [EnumMember]
        Bottom,
        [EnumMember]
        Left,
        [EnumMember]
        Right,
        [EnumMember]
        TopLeft,
        [EnumMember]
        TopRight,
        [EnumMember]
        BottomLeft,
        [EnumMember]
        BottomRight
    }
}
