using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.Serialization;

namespace Core.Infrastructure.Workflow
{
    [DataContract]
    public class ForeachEnumerator : IEnumerator
    {
        [DataMember]
        private ArrayList ForeachListCopy { get; set; }
        private IList _foreachList;
        public IList ForeachList
        {
            get { return _foreachList; }
            set
            {
                _innerEnumerator = null;
                _currentCount = 0;
                _foreachList = value;
                if (_foreachList != null) { ForeachListCopy = new ArrayList(_foreachList); }
              
               
            }
        }

        [DataMember]
        public Action<object> MovedNext { get; set; }

        private int _currentCount = 0;
        private int ForeachTotalCount { get { if (ForeachList != null) { return ForeachList.Count; } return 0; } }

        private IEnumerator _innerEnumerator = null;
        private IEnumerator InnerEnumerator
        {
            get
            {
                if (_innerEnumerator == null)
                {
                    _innerEnumerator = ForeachList?.GetEnumerator();
                }
                return _innerEnumerator;
            }
        }

        public ForeachEnumerator()
        {

        }

        public object Current
        {
            get { return InnerEnumerator?.Current; }
        }

        public bool MoveNext()
        {
            if (InnerEnumerator == null) { return false; }
            var ret = InnerEnumerator.MoveNext();
            if (ret)
            {
                ForeachInfoStack.Instance.CurrentForeachInfo.CurrentItem=InnerEnumerator.Current;
                _currentCount++;
                var info = ForeachInfoStack.Instance.Peek();
                info.CurrentCount = _currentCount;
                info.CurrentItem = this.Current;
                if (ForeachList.Count == _currentCount)
                {
                    info.NextItem = null;
                }
                else
                {
                    info.NextItem = ForeachListCopy[_currentCount];
                }
                info.TotalCount = ForeachTotalCount;

                if (MovedNext != null)
                {
                    MovedNext(InnerEnumerator.Current);
                }
            }
            return ret;
        }

        public void SetMoveNextHandler(Action<object> handler)
        {
            MovedNext = handler;
        }


        public void Reset()
        {
            this.ForeachList = null;
            _currentCount = 0;
        }
    }
}
