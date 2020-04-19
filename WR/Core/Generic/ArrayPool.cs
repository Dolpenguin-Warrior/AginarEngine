using System;

namespace Aginar.Core.Generic
{
    public class ArrayPool<T>
    {
        private T[][] _pool;
        private bool[] _available;

        public ArrayPool(uint poolSize, uint arraySize)
        {
            _pool = new T[poolSize][];
            _available = new bool[poolSize];
            for (int i = 0; i < poolSize; i++)
            {
                _pool[i] = new T[arraySize];
                _available[i] = true;
            }
        }

        public bool ArrayAvailble
        {
            get
            {
                for (int i = 0; i < _available.Length; i++)
                {
                    if (_available[i])
                        return true;
                }
                return false;
            }
        }

        public T[] GetArray(ref int token)
        {
            for (int i = 0; i < _pool.Length; i++)
            {
                if (_available[i])
                {
                    token = i;
                    _available[i] = false;
                    return _pool[i];
                }
            }
            throw new Exception("No arrays left in the array pool!");
        }

        public void ReturnArray(int token)
        {
            _available[token] = true;
        }
    }
}