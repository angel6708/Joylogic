using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Infrastructure.Workflow
{
    public class ForeachInfoStack : INotifyPropertyChanged
    {
         
     
         

        static ForeachInfoStack _instance = null;
        public static ForeachInfoStack Instance
        {
            get
            {
                if (_instance == null)
                {
                    _instance = new ForeachInfoStack();
                }
                return _instance;
            }
        }

        private ForeachInfoStack() { }

        public ForeachInfo CurrentForeachInfo { get { return this.Peek(); } }

        public bool IsInProcess { get { return _stackDeep > 0; } }

        private ForeachInfo _foreachInfo1;
        public ForeachInfo ForeachInfo1
        {
            get { return _foreachInfo1; }
            private set
            {
                _foreachInfo1 = value;
                OnPropertyChanged("ForeachInfo1");
                OnPropertyChanged("IsInProcess");
            }
        }
        private ForeachInfo _foreachInfo2;
        public ForeachInfo ForeachInfo2
        {
            get { return _foreachInfo2; }
            private set
            {
                _foreachInfo2 = value;
                OnPropertyChanged("ForeachInfo2");
                OnPropertyChanged("IsInProcess");
            }
        }
        private ForeachInfo _foreachInfo3;
        public ForeachInfo ForeachInfo3
        {
            get { return _foreachInfo3; }
            private set
            {
                _foreachInfo3 = value;
                OnPropertyChanged("ForeachInfo3");
                OnPropertyChanged("IsInProcess");
            }
        }

        private int _stackDeep = 0;

        public ForeachInfo Pop()
        {
            if (_stackDeep == 0) { return null; }
            _stackDeep--;
            ForeachInfo item;
            switch (_stackDeep)
            {
                case 0:
                    {
                          item = ForeachInfo1;
                        ForeachInfo1 = null;
                        break;
                    }
                case 1:
                    {
                         item = ForeachInfo2;
                        ForeachInfo2 = null;
                        break;
                    }
                case 2:
                    {
                          item = ForeachInfo3;
                        ForeachInfo3 = null;
                        break;
                    }
                default: return null;
            }
            OnPropertyChanged("CurrentForeachInfo");
            OnPropertyChanged("IsInProcess");
            return item;
        }

        public void Push(ForeachInfo info)
        {
            if (_stackDeep >= 3) { return; }
            _stackDeep++;
            switch (_stackDeep)
            {
                case 1:
                    ForeachInfo1 = info;
                    break;
                case 2:
                    ForeachInfo2 = info;
                    break;
                case 3:
                    ForeachInfo3 = info;
                    break;
            }
            OnPropertyChanged("CurrentForeachInfo");
            OnPropertyChanged("IsInProcess");

        }

        public ForeachInfo Peek()
        {
            switch (_stackDeep)
            {
                case 1:
                    return ForeachInfo1;
                case 2:
                    return ForeachInfo2;
                case 3:
                    return ForeachInfo3;
                default: return null;
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

        public event PropertyChangedEventHandler PropertyChanged;
    }
}
