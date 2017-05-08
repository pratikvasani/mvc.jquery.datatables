using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Http.ValueProviders;

namespace Mvc.JQuery.DataTables
{
    public class WebApiValueProvider : IGenericValueProvider
    {
        private IValueProvider _valueProvider;

        public WebApiValueProvider(IValueProvider valueProvider)
        {
            this._valueProvider = valueProvider;
        }
        public T GetValue<T>(string key)
        {
            ValueProviderResult valueResult = _valueProvider.GetValue(key);
            return (valueResult == null)
                ? default(T)
                : (T)valueResult.ConvertTo(typeof(T));
        }
    }
}
