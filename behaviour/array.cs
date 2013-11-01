using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;

namespace ns_utils
{
	class array <T>
	{
        const int min_bit_stack = 4;
        public void push(T val)
        {
            m_top++;
            if (m_top == m_uper)
            {
                m_lower = m_lower << 1;
                m_uper = m_uper << 1;

                relloc();
            }

            m_base[m_top] = val;
        }

        public T pop()
        {
            T ret = m_base[m_top];

            m_top--;
            if (m_uper > min_size_stack)
            {
                if (m_top < m_lower)
                {
                    m_uper = m_uper >> 1;
                    m_lower = m_lower >> 1;

                    //m_base = (T*)realloc(m_base, m_uper * sizeof(T));
                    relloc();
                }
            }

            return ret;
        }
        
        public bool empty()
        {
            return m_top == -1;
        }

        public void clear()
        {
            T[] tmp = m_base;
            
            //m_base = (T*)malloc(sizeof(T) * min_size_stack);
            m_base = new T[min_size_stack];
            m_top = -1;
            m_uper = min_size_stack;
            m_lower = min_size_stack >> 2;
        }

        public int top_idx()
        {
            return m_top;
        }

        public int size()
        {
            return m_top + 1;
        }

        public void resize(int sz)//TODO, 
        {
            int top = sz + 1;
            m_top = sz - 1;
            if (sz < min_size_stack) return;
            if (m_uper < top)
            {
                do
                {
                    m_uper = m_uper << 1;
                } while (m_uper < top);
                m_lower = m_uper >> 2;
                //m_base = (T*)realloc(m_base, m_uper * sizeof(T));
                relloc();

                for (int i = m_top+1; i < sz; ++i)
                {
                    m_base[i] = default(T);
                }
                return;
            }
            else if (m_uper == top)
            {
                m_uper = m_uper << 1;
                m_lower = m_uper >> 2;
                //m_base = (T*)realloc(m_base, m_uper * sizeof(T));
                relloc();
            }
            if (m_lower > sz)
            {
                if (m_uper == min_size_stack)
                {
                    return;
                }

                do
                {
                    m_lower = m_lower >> 1;
                } while (m_lower > sz);
                m_uper = m_lower << 2;
                //m_base = (T*)realloc(m_base, m_uper * sizeof(T));
                relloc();
                return;
            }

        }

        public T at(int idx)
        {
            Debug.Assert(idx <= m_top);
            return m_base[idx];
        }

        public IEnumerator<T> GetEnumerator()
        {
            for (int index = 0; index < m_top+1; index++)
            {
                yield return m_base[index];
            }
        }

        public T this[int index]
        {
            get
            {
                Debug.Assert(index <= m_top);
                return m_base[index];
            }

            set
            {
                Debug.Assert(index <= m_top);
                m_base[index] = value;
            }
        }

		//T operator[](u32 idx);
        public T top()
        {
            return m_base[m_top];
        }

        public int find(T val)
        {
            if (m_top == -1) return -1;
            int top = m_top + 1;
            for (int i = 0; i != top; ++i)
            {
                if (m_base[i].Equals(val) )
                {
                    return i;
                }
            }

            return -1;
        }

        public bool insert(int idx, T elem)
        {
            if (idx < 0) return false;
            if (idx > m_top + 1) return false;

            m_top++;
            if (m_top == m_uper)
            {
                m_lower = m_lower << 1;
                m_uper = m_uper << 1;

                //m_base = (T*)realloc(m_base, m_uper * sizeof(T));
                relloc();
            }
            T tmp = elem;
            for (int i = idx; i < m_top+1; ++i)
            {
                T tmp1 = m_base[i];
                m_base[i] = tmp;
                tmp = tmp1;
            }
            return true;
        }

        public bool erase(int idx)
        {
		    if(idx < 0) return false;
		    if(idx > m_top) return false;

            int idxiter = idx;
            T ptr = m_base[idxiter];
		    T top = m_base[m_top];
            for (; idxiter != m_top; ++idxiter)
		    {
                m_base[idxiter] = m_base[idxiter+1];
		    }
		    m_top--;

            return true;
        }

        public array(int ntime)
        {
            if (ntime < min_bit_stack) ntime = min_bit_stack;
            int sz = 1 << ntime;
            //m_base = (T*)malloc(sizeof(T) * sz);new
            m_base = new T[sz];
            m_top = -1;
            m_uper = sz;
            m_lower = sz >> 2;
        }

        public array()
        {
            int sz = min_size_stack;
            //m_base = (T*)malloc(sizeof(T) * sz);
            m_base = new T[sz];
            m_top = -1;
            m_uper = sz;
            m_lower = sz >> 2;
        }
		
        /// <summary>
        /// ///////////////////////////////////
        /// </summary>
        /// 
		int min_size_stack = 1<<min_bit_stack;
        
		int m_uper;
		int m_lower;
		int m_top;

        T[] m_base;

        void relloc()
        {
            int sz = m_base.Count();
            if (sz > m_top) sz = m_top;
            T[] m_base_old = m_base;
            m_base = new T[m_uper];
            for (int i = 0; i < sz; ++i)
            {
                m_base[i] = m_base_old[i];
            }
        }
	};

    class test
    {
        public void t1()
        {
            var t = new array<int>();
            t.push(3);
            t.push(4);
            t.push(5);
            Console.WriteLine("pop:" + t.pop());
            Console.WriteLine("pop:" + t.pop());
            Console.WriteLine("pop:" + t.pop());
            Console.WriteLine("size:" + t.size());

            t.push(300);
            t.resize(100);
            Debug.Assert(t.size() == 100);


            for (int i = 0; i < 10000; ++i)
            {
                t.push(i+1);
            }
            Debug.Assert(t.size() == 10100);
            Console.WriteLine("size:" + t.size() );

            t.resize(102);
            Debug.Assert(t.size() == 102);
            Console.WriteLine("size:" + t.size());
            t.erase(100);
            t.erase(1000);
            Debug.Assert(t.size() == 101);
            Console.WriteLine("size:" + t.size());

            t.resize(0);
            t.push(3);
            t.push(4);
            t.push(5);
            t.insert(0, 2);
            t.insert(4, 6);
            int i0 = t.at(0);
            int i1 = t.find(3);
            Console.WriteLine("size:" + t.size() );

            foreach (var elem in t)
            {
                Console.WriteLine(elem);
            }
        }
    }
}
