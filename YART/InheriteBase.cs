using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using ns_YAUtils;

namespace ns_YART
{
    public abstract class InheriteBase
    {
        static InheriteBase getTypeInstance(Type t)
        {
            try
            {
                var o = (Activator.CreateInstance(t) as InheriteBase);
                return o;
            }
            catch (Exception ex)
            {
                return null;
            }
        }

        public abstract Type[] inheritFrom();
        public virtual string stringForm(int space) 
        {
            if (space == 0) return ""; 
            else 
                return Enumerable.Repeat(" ", space).Aggregate((s1, s2) => s1 + s2); 
        }

        Dictionary<Type, InheriteBase> mInheritesInstance = new Dictionary<Type, InheriteBase>();
        public InheriteBase self;
        public InheriteBase()
        {
            self = this;
        }

        public bool isInherited(Type t, out InheriteBase instance, InheriteBase final)
        {
            if (mInheritesInstance.TryGetValue(t, out instance) )
            {
                return true;
            }
            var ls = inheritFrom();
            foreach (Type elem in ls)
            {
                if (elem == t)
                {
                    instance = getTypeInstance(elem);
                    instance.self = final;
                    mInheritesInstance.Add(t, instance);
                    return true;
                }
                else
                {
                    InheriteBase o;
                    if (!mInheritesInstance.TryGetValue(elem, out o))
                    {
                        o = getTypeInstance(elem);
                        o.self = final;
                        mInheritesInstance.Add(elem, o);
                    }

                    if (o.isInherited(t, out instance, final))
                    {
                        mInheritesInstance.Add(t, instance);
                        instance.self = final;
                        return true;
                    }
                }
            }
            return false;
        }

        public T cast<T>() where T : InheriteBase
        {
            var t = typeof(T);
            return self.castImpl<T>();
        }

        T castImpl<T>() where T : InheriteBase
        {
            var t = typeof(T);

            if (t == this.GetType()) return this as T;
            InheriteBase ins;
            if (isInherited(t, out ins, this))
            {
                return ins as T;
            }

            return null;
        }


    }
}