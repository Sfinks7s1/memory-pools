﻿namespace MemoryPools.Collections.Linq
{
    internal class UnionExprEnumerable<T> : IPoolingEnumerable<T>
    {
        private IPoolingEnumerable<T> _src, _second;
        private int _count;

        public UnionExprEnumerable<T> Init(IPoolingEnumerable<T> src, IPoolingEnumerable<T> second)
        {
            _src = src;
            _count = 0;
            _second = second;
            return this;
        }

        public IPoolingEnumerator<T> GetEnumerator()
        {
            _count++;
            return Pool.Get<UnionExprEnumerator>().Init(this, _src.GetEnumerator(), _second.GetEnumerator());
        }

        private void Dispose()
        {
            if (_count == 0) return;
            _count--;
            if (_count == 0)
            {
                _src = default;
                _second = default;
                Pool.Return(this);
            }
        }

        internal class UnionExprEnumerator : IPoolingEnumerator<T>
        {
            private UnionExprEnumerable<T> _parent;
            private IPoolingEnumerator<T> _src, _second;
            private bool _first;

            public UnionExprEnumerator Init(
                UnionExprEnumerable<T> parent, IPoolingEnumerator<T> src, IPoolingEnumerator<T> second) 
            {
                _parent = parent;
                _src = src;
                _second = second;
                _first = true;
                return this;
            }

            public bool MoveNext()
            {
                if (_first)
                {
                    if (_src.MoveNext())
                    {
                        return true;
                    }

                    _first = false;
                }

                return _second.MoveNext();
            }

            public void Reset()
            {
                _first = true;
                _src.Reset();
                _second.Reset();
            }

            object IPoolingEnumerator.Current => Current;

            public T Current => _first ? _src.Current : _second.Current;

            public void Dispose()
            {
                _parent?.Dispose();
                _parent = default;
                _src?.Dispose();
                _src = default;
                _second?.Dispose();
                _second = default;
                Pool.Return(this);
            }
        }

        IPoolingEnumerator IPoolingEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }
}