using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SBSC.Lib.MVC.ModelState;

namespace System.Web.Mvc
{
    public static class ModelStateDictionaryExtensions
    {
        public static string GetModelStateErrors(this ModelStateDictionary modelStateDictionary)
        {
            IEnumerable<ModelState> errorValues = modelStateDictionary.Values.Where(x => x.Errors.Count > 0);

            var vList = new List<string>();

            foreach (var value in errorValues)
                foreach (var error in value.Errors)
                    vList.Add(error.ErrorMessage);

            return string.Join("\n", vList.ToArray());
        }

        /// <summary>
        /// Adiciona todos os erros contidos no ViewModelErrors ao ModelState, e remove os errors
        /// cujas chaves foram inseridas para remoção com o método 'RemoveErrorKey'
        /// </summary>
        /// <param name="modelState"></param>
        /// <param name="viewModelError">ViewModelError que será mesclado ao ModelState.</param>
        public static void MergeErrors(this ModelStateDictionary modelState, ViewModelErrors viewModelError)
        {
            foreach (var removeKey in viewModelError.KeysToRemove)
            {
                modelState.Remove(removeKey);
            }

            foreach (var error in viewModelError.Errors)
            {
                modelState.AddModelError(error.Item1, error.Item2);
            }

        }
    }
}
