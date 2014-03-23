/*
 * author: xiaofeng.li
 * mail: 453588006@qq.com
 * desc: 
 * */

using System.Diagnostics;

namespace ns_YAUtils
{
    public class Singleton<T> where T : new()
    {
        private static readonly object _lock = new object();
        private static T instance;

        protected Singleton()
        {
            Debug.Assert(instance == null);
        }

        public static bool Exists
        {
            get
            {
                return instance != null;
            }
        }

        public static T Instance
        {
            get
            {
                if (instance == null)
                {
                    lock (_lock)
                    {
                        if (instance == null)
                        {
                            instance = new T();
                        }
                    }
                }
                return instance;
            }
        }
    }
}
