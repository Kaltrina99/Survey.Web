using Microsoft.AspNetCore.Mvc.ModelBinding;
using System;
using System.Linq;
using System.Linq.Expressions;
using System.Web.Mvc;

namespace Survey.Web.Extension
{
    public static class ModelStateExtension
    {
        public static void RemoveFor<TModel>(this Microsoft.AspNetCore.Mvc.ModelBinding.ModelStateDictionary modelState,
                                       Expression<Func<TModel, object>> expression)
        {
            string expressionText = ExpressionHelper.GetExpressionText(expression);

            foreach (var ms in modelState.ToArray())
            {
                if (ms.Key.StartsWith(expressionText + ".") || ms.Key == expressionText||ms.Key.StartsWith(expressionText + "["))
                {
                    modelState.Remove(ms.Key);
                }
            }
        }
    }
}
