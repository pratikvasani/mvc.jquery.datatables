using System;
using System.Web.Http.Controllers;
using System.Web.Http.ModelBinding;
using System.Web.Mvc;
using Mvc.JQuery.DataTables;

namespace Mvc.JQuery.DataTables
{
    /// <summary>
    /// Model binder for datatables.js parameters a la http://geeksprogramando.blogspot.com/2011/02/jquery-datatables-plug-in-with-asp-mvc.html
    /// </summary>
    public class DataTablesModelBinder : System.Web.Mvc.IModelBinder, System.Web.Http.ModelBinding.IModelBinder
    {
        public object BindModel(ControllerContext controllerContext, System.Web.Mvc.ModelBindingContext bindingContext)
        {
            IGenericValueProvider valueProvider = new MvcValueProvider(bindingContext.ValueProvider);
            return Bind(valueProvider);
        }

        private object Bind(IGenericValueProvider valueProvider)
        {
            int columns = valueProvider.GetValue<int>("iColumns");
            if (columns == 0)
            {
                return BindV10Model(valueProvider);
            }
            else
            {
                return BindLegacyModel(valueProvider, columns);
            }
        }

        private object BindV10Model(IGenericValueProvider valueProvider)
        {
            DataTablesParam obj = new DataTablesParam();
            obj.iDisplayStart = valueProvider.GetValue<int>( "start");
            obj.iDisplayLength = valueProvider.GetValue<int>( "length");
            obj.sSearch = valueProvider.GetValue<string>( "search[value]");
            obj.bEscapeRegex = valueProvider.GetValue<bool>( "search[regex]");
            obj.sEcho = valueProvider.GetValue<int>( "draw");

            int colIdx = 0;
            while (true)
            {
                string colPrefix = String.Format("columns[{0}]", colIdx);
                string colName = valueProvider.GetValue<string>( colPrefix+"[data]");
                if (String.IsNullOrWhiteSpace(colName)) {
                    break;
                }
                obj.sColumnNames.Add(colName);
                obj.bSortable.Add(valueProvider.GetValue<bool>( colPrefix+"[orderable]"));
                obj.bSearchable.Add(valueProvider.GetValue<bool>( colPrefix+"[searchable]"));
                obj.sSearchValues.Add(valueProvider.GetValue<string>( colPrefix+"[search][value]"));
                obj.bEscapeRegexColumns.Add(valueProvider.GetValue<bool>( colPrefix+"[searchable][regex]"));
                colIdx++;
            }
            obj.iColumns = colIdx;
            colIdx = 0;
            while (true)
            {
                string colPrefix = String.Format("order[{0}]", colIdx);
                int? orderColumn = valueProvider.GetValue<int?>( colPrefix+"[column]");
                if (orderColumn.HasValue)
                {
                    obj.iSortCol.Add(orderColumn.Value);
                    obj.sSortDir.Add(valueProvider.GetValue<string>( colPrefix+"[dir]"));
                    colIdx++;
                }
                else
                {
                    break;
                }
            }
            obj.iSortingCols = colIdx;
            return obj;
        }

        private DataTablesParam BindLegacyModel(IGenericValueProvider valueProvider, int columns)
        {
            DataTablesParam obj = new DataTablesParam(columns);

            obj.iDisplayStart = valueProvider.GetValue<int>( "iDisplayStart");
            obj.iDisplayLength = valueProvider.GetValue<int>( "iDisplayLength");
            obj.sSearch = valueProvider.GetValue<string>( "sSearch");
            obj.bEscapeRegex = valueProvider.GetValue<bool>( "bEscapeRegex");
            obj.iSortingCols = valueProvider.GetValue<int>( "iSortingCols");
            obj.sEcho = valueProvider.GetValue<int>( "sEcho");

            for (int i = 0; i < obj.iColumns; i++)
            {
                obj.bSortable.Add(valueProvider.GetValue<bool>( "bSortable_" + i));
                obj.bSearchable.Add(valueProvider.GetValue<bool>( "bSearchable_" + i));
                obj.sSearchValues.Add(valueProvider.GetValue<string>( "sSearch_" + i));
                obj.bEscapeRegexColumns.Add(valueProvider.GetValue<bool>( "bEscapeRegex_" + i));
                obj.iSortCol.Add(valueProvider.GetValue<int>( "iSortCol_" + i));
                obj.sSortDir.Add(valueProvider.GetValue<string>( "sSortDir_" + i));
            }
            return obj;
        }
       
        public bool BindModel(HttpActionContext actionContext, System.Web.Http.ModelBinding.ModelBindingContext bindingContext)
        {
            IGenericValueProvider valueProvider = new WebApiValueProvider(bindingContext.ValueProvider);
            bindingContext.Model = Bind(valueProvider);
            return true;

        }
    }
}