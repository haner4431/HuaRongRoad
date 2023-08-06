using System;
using System.Collections;
using System.Collections.Generic;

namespace  LFrame.Core.Tools
{
    /// <summary>
    /// 优先队列的排序比较器
    /// </summary>
    public class PriorityQueueComparer : IComparer<StateNode>
    {
        public static PriorityQueueComparer Comparer = new PriorityQueueComparer();
        public int Compare(StateNode node1, StateNode node2)
        {
            if (node1.cost == node2.cost)
            {
                return 0;
            }
            else if (node1.cost > node2.cost)
            {
                return 1;
            }
            else
            {
                return -1;
            }
        }
       
    }
    public class PriorityQueue<T>:IEnumerable<T>,IEnumerator<T>
    {
       
        private List<T> nodes;
        private IComparer<T> comparer;
        private int cursor = -1;

        public T this[int i]
        {
            get { return nodes[i]; }
            set { nodes[i] = value; }
        }

        public int Count
        {
            get
            {
                return nodes.Count;
            }
        }

        public void Clear()
        {
            nodes.Clear();
        }
        public T Current
        {
            get { return CurrentNode(); }
        }
        Object IEnumerator.Current
        {
            get { return Current; }
        }
        public void Dispose()
        {
            
        }
        T CurrentNode()
        {
            if (cursor < 0 || cursor > nodes.Count)
                return default(T);
            else
                return nodes[cursor];
        }

        public PriorityQueue(IComparer<T> comparer)
        {
            nodes = new List<T>(1000);
            this.comparer = comparer;
        }
        public void Reset()
        {
            cursor=-1;
        }
        
        public bool MoveNext()
        {
            cursor++;
            if(cursor<nodes.Count&&nodes[cursor]!=null)
                return true;
            return false;
        }
        public IEnumerator<T> GetEnumerator()
        {
            Reset();
            return this;
        }
        
        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }


      

        public void Enqueue(T t)
        {
            nodes.Add(t);
            nodes.Sort(comparer);
        }
        
        public T Dequeue()
        {
            if(nodes.Count != 0){
               
                var ele = nodes[0];
                nodes.RemoveAt(0);
                return ele;
            }

            return default(T);
            
        }

        public bool  Remove(T t)
        {
            var state = nodes.Remove(t);
            nodes.Sort(comparer);
            return state;
        }

        public T Peek()
        {
             if(nodes.Count != 0)
             {
                 return nodes[0];
             }

            return default(T);
        }
    }
}
