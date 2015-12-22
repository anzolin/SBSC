using SBSC.Lib.MVC.ModelState;
using SBSC.Model.Models;
using SBSC.ViewModel.Login;
using System;
using System.Linq;

namespace SBSC.Business
{
    public class Login
    {
        public class ConstantesInternas
        {
            public const string ModuleNameSingular = "Login";
            public const string ModuleNamePlural = "Login";
        }

        public static bool ValidarLogin(SBSCEntities db, ref ViewModelErrors viewModelError, LoginViewModel formModel, out Pessoa _pessoa)
        {
            if (formModel == null) throw new ArgumentNullException("formModel");

            var login = formModel.Login.Trim();
            var senha = formModel.Senha.Trim();

            var pessoa = db.Pessoa.FirstOrDefault(q => q.Login.Equals(login));

            if (pessoa == null)
            {
                viewModelError.AddModelError("Login", "O login informado não existe.");
            }
            else
            {
                if (!pessoa.Senha.Equals(senha))
                {
                    viewModelError.AddModelError("Senha", "A senha informada não está correta.");
                }
            }

            _pessoa = pessoa;

            return viewModelError.IsValid;
        }
    }
}
