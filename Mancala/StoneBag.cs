using System;
using System.Collections.Generic;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using System.Text;
using System.ComponentModel;

namespace Mancala
{
    public class StoneBag : ICollection<Stone>
    {

        //public List<Stone> StoneList;

        private List<Stone> _list { get; set; }

        private Random _rand;

        public event EventHandler OnContentsChanged;

        public void FireContentsChanged()
        {
            if (OnContentsChanged != null)
            {
                OnContentsChanged(this, EventArgs.Empty);
            }
        }
        
        public StoneBag()
        {
            _list = new List<Stone>();
            _rand = new Random(DateTime.Now.Millisecond);
        }

        public Stone[] Stones
        {
            get
            {
            Stone[] contents = _list.ToArray();
            return (contents);
            }
        }
        
        public void Add(Stone stone)
        {
            _list.Add(stone);
            FireContentsChanged();
        }

        public void Add(Stone[] stones)
        {
            foreach (Stone stone in stones)
            {
                _list.Add(stone);
            }
        }

        public bool IsEmpty()
        {
            return (_list.Count == 0);
        }

        public Stone TakeOne()
        {
            int count = _list.Count;
            int index = 0; // _rand.Next(count);
            Stone stone = _list[index];
            _list.RemoveAt(index);
            FireContentsChanged();
            return (stone);
        }

        public Stone[] TakeAll()
        {
            Stone[] contents = _list.ToArray();
            _list.Clear();
            FireContentsChanged();
            return (contents);
        }

        public void Clear()
        {
            _list.Clear();
            FireContentsChanged();
        }

        public bool Contains(Stone item)
        {
            throw new NotImplementedException();
        }

        public void CopyTo(Stone[] array, int arrayIndex)
        {
            throw new NotImplementedException();
        }

        public int Count
        {
            get
            {
                return (Stones.Length);
            }
        }

        public bool IsReadOnly
        {
            get { return (false); }
        }

        public bool Remove(Stone item)
        {
            throw new NotImplementedException();
        }

        public IEnumerator<Stone> GetEnumerator()
        {
            return (_list.GetEnumerator());
        }

        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            return (_list.GetEnumerator());
        }
    }

}
