using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Infrastructure.Workflow
{
    public class ForeachInfo : INotifyPropertyChanged
    {
        private object _current;
        public object CurrentItem
        {
            get
            {
                return _current;
            }

            internal set
            {
                _current = value;
                OnPropertyChanged("CurrentItem");
            }
        }

        private object _next;
        public object NextItem
        {
            get
            {
                return _next;
            }
            internal set
            {
                _next = value;
                OnPropertyChanged("NextItem");
            }
        }

        private int _currentCount;
        public int CurrentCount
        {
            get
            {
                return _currentCount;
            }
            internal set
            {
                _currentCount = value;
                OnPropertyChanged("CurrentCount");
            }
        }

        private int _totalCount;
        public int TotalCount
        {
            get
            {
                return _totalCount;
            }
            internal set
            {
                _totalCount = value;
                OnPropertyChanged("TotalCount");
            }
        }

        private void OnPropertyChanged(string pName)
        {
            var handler = PropertyChanged;
            if (handler != null)
            {
                handler(this, new PropertyChangedEventArgs(pName));
            }
        }

        public override string ToString()
        {
            return string.Format("当前:第{0}个，共{1}个", CurrentCount, TotalCount);
        }

        public event PropertyChangedEventHandler PropertyChanged;
    }
}
