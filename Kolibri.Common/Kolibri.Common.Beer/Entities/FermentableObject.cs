using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kolibri.Common.Beer.Entities
{
    public class FermentableObject
    {
        public string Category { get; set; }
        public string Color { get; set; }
        public string Country { get; set; }
        public string Fermentable { get; set; }
        public string PPG { get; set; }
        public string Recipes { get; set; }
        public string Type { get; set; }
        public string Manufacturer { get; set; }
        public string Name { get; set; }
    }

    public class DynamicWrapper<T> : DynamicObject
    {
        public T Instance { get; private set; }
        public DynamicWrapper(T instance)
        {
            this.Instance = instance;
        }

        public override bool TryConvert(ConvertBinder binder, out object result)
        {
            if (binder.ReturnType == typeof(T))
            {
                result = Instance;
                return true;
            }
            if (binder.ReturnType == typeof(T[]) && binder.Explicit)
            {
                result = new[] { Instance };
                return true;
            }
            return base.TryConvert(binder, out result);
        }

        public override string ToString()
        {
            return Convert.ToString(Instance);
        }
    }

}
