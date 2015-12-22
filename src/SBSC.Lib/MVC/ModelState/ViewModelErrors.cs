using System;
using System.Collections.Generic;
using System.Linq;

namespace SBSC.Lib.MVC.ModelState
{
    public class ViewModelErrors
    {
        public List<Tuple<string, string>> Errors { get; private set; }
        public List<string> KeysToRemove { get; private set; }

        /// <summary>
        /// Cria uma nova instância de ViewModelErrors.
        /// </summary>
        public ViewModelErrors()
        {
            Errors = new List<Tuple<string, string>>();
            KeysToRemove = new List<string>();
        }

        /// <summary>
        /// Remove o item inserido no ViewModelErrors, e caso este objeto
        /// seja aplicado a um ModelState, estes itens também serão removidos
        /// do ModelState
        /// </summary>
        /// <param name="key"></param>
        public void RemoveErrorKey(string key)
        {
            var itensToRemove = Errors.Where(e => e.Item1 == key);

            foreach (var itemToRemove in itensToRemove)
                Errors.Remove(itemToRemove);

            KeysToRemove.Add(key);
        }

        /// <summary>
        /// Adiciona um novo erro ao ViewModelErrors, deve ser utilizado da mesma maneira
        /// que o ModelState.AddModelError(string, string).
        /// </summary>
        /// <param name="key">Nome da propriedade do erro.</param>
        /// <param name="value">Descrição do erro.</param>
        public void AddModelError(string key, string value)
        {
            Tuple<string, string> t = new Tuple<string, string>(key, value);

            Errors.Add(t);
        }

        /// <summary>
        /// Verifica se o ViewModelErrors está válido.
        /// </summary>
        /// <returns>retorna true se o não houverem erros registrados, false caso contrário.
        /// </returns>
        public bool IsValid
        {
            get
            {
                return (Errors == null || Errors.Count == 0);
            }
        }
    }
}
