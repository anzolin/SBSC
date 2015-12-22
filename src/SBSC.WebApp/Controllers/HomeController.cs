using SBSC.Business;
using SBSC.Model;
using SBSC.ViewModel.Emprestimos;
using SBSC.ViewModel.Livros;
using SBSC.ViewModel.Reservas;
using SBSC.ViewModel.Widgets;
using SBSC.WebApp.Models;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using System.Web.Routing;

namespace SBSC.WebApp.Controllers
{
    [Authorize]
    public class HomeController : BaseController
    {
        protected override void Initialize(RequestContext requestContext)
        {
            base.Initialize(requestContext);
        }

        public HomeController()
        {
        }

        public ActionResult Index()
        {
            return View();
        }

        public ActionResult WidgetLivros()
        {
            var widget = new WidgetViewModel
            {
                PanelStyle = "panel-info",
                Title = "Sobre Livros",
                Description = "Aqui temos informações interessantes sobre livros.",
                //Description = "Aqui temos informações interessantes sobre livros e seus números, como empréstimos e reservas, comparando o ano atual com o anterior, dentre outros dados.",
                ColumnTitle = "Índices",
                MessageNoItens = "Nenhum livro cadastrado.",
                Itens = GetWidgetNumerosLivros()
            };

            return View("Widget", widget);
        }

        private List<WidgetListaViewModel> GetWidgetNumerosLivros()
        {
            var searchModel = new LivroSearchModel();

            var livros = LivrosModel.GetBaseQuery(db, searchModel);

            var currentYear = Genericos.GetDateTimeFromBrazil().Year;
            var lastYear = currentYear - 1;

            var lista = new List<WidgetListaViewModel>();

            lista.Add(new WidgetListaViewModel()
            {
                Description = "Total de livros",
                Total = livros.Count()
            });

            lista.Add(new WidgetListaViewModel()
            {
                Description = "Livros emprestados até hoje",
                Total = livros.Count(c =>
                    c.ItensEmprestimo.Any(a => a.Emprestimo.Status == (int)Enumerations.Emprestimo.StatusEmprestimo.Emprestado))
            });

            lista.Add(new WidgetListaViewModel()
            {
                Description = string.Format("Livros emprestados este ano ({0})", currentYear),
                Total = livros.Count(c =>
                    c.ItensEmprestimo.Any(a => a.Emprestimo.Status == (int)Enumerations.Emprestimo.StatusEmprestimo.Emprestado
                    && a.Emprestimo.DtEmprestimo.Year == currentYear))
            });

            lista.Add(new WidgetListaViewModel()
            {
                Description = string.Format("Livros emprestados ano passado ({0})", lastYear),
                Total = livros.Count(c =>
                    c.ItensEmprestimo.Any(a => a.Emprestimo.Status == (int)Enumerations.Emprestimo.StatusEmprestimo.Emprestado
                    && a.Emprestimo.DtEmprestimo.Year == lastYear))
            });

            lista.Add(new WidgetListaViewModel()
            {
                Description = "Livros devolvidos",
                Total = livros.Count(c =>
                    c.ItensEmprestimo.Any(a => a.Emprestimo.Status == (int)Enumerations.Emprestimo.StatusEmprestimo.Devolvido))
            });

            lista.Add(new WidgetListaViewModel()
            {
                Description = "Livros reservados até hoje",
                Total = livros.Count(c =>
                    c.Reservas.Any(a => a.Status == (int)Enumerations.Reserva.StatusReserva.Atendido))
            });

            lista.Add(new WidgetListaViewModel()
            {
                Description = string.Format("Livros reservados este ano ({0})", currentYear),
                Total = livros.Count(c =>
                    c.Reservas.Any(a => a.Status == (int)Enumerations.Reserva.StatusReserva.Atendido
                        && a.DtPrevisaoEmprestimo.Value.Year == currentYear))
            });

            lista.Add(new WidgetListaViewModel()
            {
                Description = string.Format("Livros reservados ano passado ({0})", lastYear),
                Total = livros.Count(c =>
                    c.Reservas.Any(a => a.Status == (int)Enumerations.Reserva.StatusReserva.Atendido
                        && a.DtPrevisaoEmprestimo.Value.Year == lastYear))
            });

            return lista;
        }

        public ActionResult WidgetEmprestimos()
        {
            var widget = new WidgetViewModel
            {
                PanelStyle = "panel-success",
                Title = "Sobre Empréstimos e Devoluções",
                Description = "Aqui temos informações interessantes sobre empréstimos e devoluções.",
                ColumnTitle = "Índices",
                MessageNoItens = "Nenhum emprestimo/devolução cadastrada.",
                Itens = GetWidgetNumerosEmprestimos()
            };

            return View("Widget", widget);
        }

        private List<WidgetListaViewModel> GetWidgetNumerosEmprestimos()
        {
            var searchModel = new EmprestimoSearchModel();

            var emprestimos = EmprestimosModel.GetBaseQuery(db, searchModel);

            var currentYear = Genericos.GetDateTimeFromBrazil().Year;
            var lastYear = currentYear - 1;

            var lista = new List<WidgetListaViewModel>();

            lista.Add(new WidgetListaViewModel()
            {
                Description = "Total de empréstimos",
                Total = emprestimos.Count()
            });

            lista.Add(new WidgetListaViewModel()
            {
                Description = "Empréstimos em aberto (sem atraso)",
                Total = emprestimos.Count(c => c.Status == (int)Enumerations.Emprestimo.StatusEmprestimo.Emprestado)
            });

            lista.Add(new WidgetListaViewModel()
            {
                Description = "Empréstimos em aberto (com atraso)",
                Total = emprestimos.Count(c => c.Status == (int)Enumerations.Emprestimo.StatusEmprestimo.Emprestado)
            });

            lista.Add(new WidgetListaViewModel()
            {
                Description = "Empréstimos realizados",
                Total = emprestimos.Count(c => c.Status == (int)Enumerations.Emprestimo.StatusEmprestimo.Devolvido)
            });

            lista.Add(new WidgetListaViewModel()
            {
                Description = string.Format("Empréstimos realizados este ano ({0})", currentYear),
                Total = emprestimos.Count(c => c.Status == (int)Enumerations.Emprestimo.StatusEmprestimo.Devolvido
                    && c.DtEmprestimo.Year == currentYear)
            });

            lista.Add(new WidgetListaViewModel()
            {
                Description = string.Format("Empréstimos realizados ano passado ({0})", lastYear),
                Total = emprestimos.Count(c => c.Status == (int)Enumerations.Emprestimo.StatusEmprestimo.Devolvido
                    && c.DtEmprestimo.Year == lastYear)
            });

            //lista.Add(new WidgetListaViewModel()
            //{
            //    Description = "Devoluções em atraso",
            //    Total = 0
            //});

            return lista;
        }

        public ActionResult WidgetReservas()
        {
            var widget = new WidgetViewModel
            {
                PanelStyle = "panel-warning",
                Title = "Sobre Reservas",
                Description = "Aqui temos informações interessantes sobre reservas.",
                ColumnTitle = "Índices",
                MessageNoItens = "Nenhuma reserva cadastrada.",
                Itens = GetWidgetNumerosReservas()
            };

            return View("Widget", widget);
        }

        private List<WidgetListaViewModel> GetWidgetNumerosReservas()
        {
            var searchModel = new ReservaSearchModel();

            var userData = GetUserData();

            var reservas = ReservasModel.GetBaseQuery(db, searchModel, userData);

            var currentYear = Genericos.GetDateTimeFromBrazil().Year;
            var lastYear = currentYear - 1;

            var lista = new List<WidgetListaViewModel>();

            lista.Add(new WidgetListaViewModel()
            {
                Description = "Total de reservas",
                Total = reservas.Count()
            });

            lista.Add(new WidgetListaViewModel()
            {
                Description = "Reservas pendentes",
                Total = reservas.Count(c => c.Status == (int)Enumerations.Reserva.StatusReserva.Pendente)
            });

            lista.Add(new WidgetListaViewModel()
            {
                Description = "Reservas atendidas",
                Total = reservas.Count(c => c.Status == (int)Enumerations.Reserva.StatusReserva.Atendido)
            });

            lista.Add(new WidgetListaViewModel()
            {
                Description = string.Format("Reservas atendidas este ano ({0})", currentYear),
                Total = reservas.Count(c => c.Status == (int)Enumerations.Reserva.StatusReserva.Atendido
                    && c.DtPrevisaoEmprestimo.Value.Year == currentYear)
            });

            lista.Add(new WidgetListaViewModel()
            {
                Description = string.Format("Reservas atendidas ano passado ({0})", lastYear),
                Total = reservas.Count(c => c.Status == (int)Enumerations.Reserva.StatusReserva.Atendido
                    && c.DtPrevisaoEmprestimo.Value.Year == lastYear)
            });

            return lista;
        }
    }
}