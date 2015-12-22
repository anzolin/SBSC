using SBSC.Business;
using SBSC.Lib.MVC.ModelState;
using SBSC.Model.Models;
using SBSC.ViewModel.Login;
using SBSC.WebApp.Extensions;
using SBSC.WebApp.Models;
using System.Web.Mvc;
using System.Web.Routing;
using System.Web.Security;

namespace SBSC.WebApp.Controllers
{
    [AllowAnonymous]
    public class LoginController : BaseController
    {
        protected override void Initialize(RequestContext requestContext)
        {
            base.Initialize(requestContext);
        }

        public LoginController()
        {
        }

        public ActionResult Index()
        {
            return RedirectToAction("Login");
        }

        [HttpGet]
        public ActionResult Login()
        {
            ViewBag.Title = "Login";

            return View();
        }

        [HttpPost]
        public ActionResult Login(LoginViewModel formModel)
        {
            var viewModelError = new ViewModelErrors();

            if (ModelState.IsValid)
            {
                var _pessoa = new Pessoa();

                if (LoginModel.ValidarLogin(db, ref viewModelError, formModel, out _pessoa))
                {
                    var cookieData = string.Format("Autorize-{0}-{1}-{2}", _pessoa.Tipo, _pessoa.Id, _pessoa.Login);

                    FormsAuthentication.SetAuthCookie(cookieData, false);

                    if (!string.IsNullOrEmpty(Request.QueryString["ReturnUrl"]))
                        Response.Redirect(Request.QueryString["ReturnUrl"]);
                    else
                        return RedirectToAction("Index", "Home");
                }
            }

            ModelState.MergeErrors(viewModelError);

            this.AddNotification(Constantes.NaoFoiPossivelExecutarAOperacaoPorFavorVerifiqueAsMensagensDeValidacao, NotificationType.ERROR);

            formModel.Senha = string.Empty;

            ViewBag.Title = "Login";

            return View(formModel);
        }

        public ActionResult Logout()
        {
            Session.Clear();
            Session.Abandon();
            FormsAuthentication.SignOut();

            return RedirectToAction("Login");
        }
    }
}