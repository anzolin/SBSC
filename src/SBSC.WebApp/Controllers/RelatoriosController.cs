using Rotativa;
using Rotativa.Options;
using SBSC.Business;
using SBSC.Lib.Helpers;
using SBSC.Model;
using SBSC.ViewModel.Devolucoes;
using SBSC.ViewModel.Emprestimos;
using SBSC.ViewModel.Livros;
using SBSC.ViewModel.Pessoas;
using SBSC.ViewModel.Reservas;
using SBSC.WebApp.Models;
using System;
using System.Linq;
using System.Web.Mvc;
using System.Web.Routing;

namespace SBSC.WebApp.Controllers
{
    [Authorize]
    public class RelatoriosController : ReportBaseController
    {
        protected override void Initialize(RequestContext requestContext)
        {
            base.Initialize(requestContext);

            var tipoPessoa = GetUserData().Item1;

            if (tipoPessoa == (int)Enumerations.Pessoa.TipoPessoa.Aluno)
            {
                requestContext.HttpContext.Response.RedirectToRoute(new RouteValueDictionary(new { action = "Index", controller = "Home" })); //.Result = new RedirectToRouteResult(new RouteValueDictionary(new { action = "Index", controller = "Home" }));
            }
        }

        public RelatoriosController()
        {
        }

        public override string GetModuleName()
        {
            return RelatoriosModel.ConstantesInternas.ModuleNamePlural;
        }

        public override string GetModuleNameSingular()
        {
            return RelatoriosModel.ConstantesInternas.ModuleNameSingular;
        }

        public override string GetModuleNamePlural()
        {
            return RelatoriosModel.ConstantesInternas.ModuleNamePlural;
        }

        public ActionResult Index()
        {
            ViewBag.Title = GetModuleNamePlural();
            ViewBag.Subtitle = string.Format("Lista de {0}", GetModuleNamePlural());

            return View();
        }

        #region Relatório de pessoas

        [HttpGet]
        public ActionResult EmitirReportPessoas()
        {
            ViewBag.Title = GetModuleNamePlural();
            ViewBag.Subtitle = string.Format(Constantes.GerarRelatorioDe_, "pessoas");
            ViewBag.Information = string.Format(Constantes.GerarRelatorioDe_AplicandoOFiltroAbaixoSeNadaForPreenchidoORelatorioPoderaDemorarParaSerGeradoE, "pessoas");
            ViewBag.SearchWaterMark = "Pesquisar pelos campos: nome, login";

            return View();
        }

        [HttpPost]
        public ActionResult EmitirReportPessoas(PessoaSearchModel searchModel)
        {
            var query = PessoasModel.GetBaseQuery(db, searchModel).OrderBy(q => q.Nome);

            var model = new PessoaSearchViewModel
            {
                SearchModel = searchModel,
                Termo = searchModel.Search,
                FoundObjects = query.Count(),
                Objetos = (from objeto in query.ToList()
                           let totalVinculos = PessoasModel.GetTotalByItemLinkedRecords(db, objeto.Id)
                           select new PessoaViewModel
                           {
                               Id = objeto.Id,
                               Nome = objeto.Nome,
                               CodigoMatricula = objeto.CodigoMatricula,
                               Login = objeto.Login,
                               DtCadastro = objeto.DtHrCadastro,
                               DtCadastroText = objeto.DtHrCadastro.ToShortDateString(),
                               DtEdicao = objeto.DtHrEdicao,
                               DtEdicaoText = objeto.DtHrEdicao.HasValue ? objeto.DtHrEdicao.Value.ToShortDateString() : string.Empty,
                               TotalReservas = totalVinculos.Item1,
                               TotalEmprestimos = totalVinculos.Item2
                           }).OrderBy(ob => ob.Nome).ToList()
            };

            ViewBag.Title = string.Format("Relatório de {0}", "pessoas");
            ViewBag.Subtitle = string.Format(Constantes.GeradoEm_, Genericos.GetDateTimeFromBrazil());

            return new ViewAsPdf("PessoaSearchResultList", model)
            {
                FileName = "RelatorioPessoas.pdf",
                PageSize = Size.A4,
                PageOrientation = Orientation.Landscape,
                PageMargins = { Top = 1, Left = 1, Right = 1, Bottom = 1 }
            };
        }

        #endregion

        #region Relatório de empréstimos

        [HttpGet]
        public ActionResult EmitirReportEmprestimos()
        {
            ViewBag.Title = GetModuleNamePlural();
            ViewBag.Subtitle = string.Format(Constantes.GerarRelatorioDe_, "empréstimos");
            ViewBag.Information = string.Format(Constantes.GerarRelatorioDe_AplicandoOFiltroAbaixoSeNadaForPreenchidoORelatorioPoderaDemorarParaSerGeradoE, "empréstimos");
            ViewBag.SearchWaterMark = "Pesquisar pelos campos: código do empréstimo, nome da pessoa, código do livro, título, gênero, autores, editora, local";

            return View();
        }

        [HttpPost]
        public ActionResult EmitirReportEmprestimos(EmprestimoSearchModel searchModel)
        {
            var query = EmprestimosModel.GetBaseQuery(db, searchModel).OrderByDescending(q => q.DtEmprestimo);

            var model = new EmprestimoSearchViewModel
            {
                SearchModel = searchModel,
                Termo = searchModel.Search,
                FoundObjects = query.Count(),
                Objetos = (from objeto in query.ToList()
                           let pessoa = objeto.Pessoa
                           let livros = objeto.ItensEmprestimo.ToList()
                           let dtDevolucao = (objeto.Status == (int)Enumerations.Emprestimo.StatusEmprestimo.Devolvido ? objeto.ItensEmprestimo.Select(s => s.DtDevolucao).FirstOrDefault() : (DateTime?)null)
                           select new EmprestimoViewModel
                           {
                               Id = objeto.Id,
                               PessoaId = pessoa.Id,
                               PessoaText = pessoa.Nome,
                               LivrosText = string.Join("; ", livros.Select(x => x.Livro.Titulo).ToList()),
                               StatusId = (int)objeto.Status,
                               StatusText = EnumsHelper.GetText(typeof(Enumerations.Emprestimo.StatusEmprestimo), (int)objeto.Status),
                               DtEmprestimo = objeto.DtEmprestimo,
                               DtEmprestimoText = objeto.DtEmprestimo.ToShortDateString(),
                               DtPrevisaoDevolucao = objeto.DtPrevisaoDevolucao,
                               DtPrevisaoDevolucaoText = objeto.DtPrevisaoDevolucao.HasValue ? objeto.DtPrevisaoDevolucao.Value.ToShortDateString() : string.Empty,
                               DtDevolucao = dtDevolucao,
                               DtDevolucaoText = dtDevolucao.HasValue ? dtDevolucao.Value.ToShortDateString() : string.Empty,
                               DtCadastro = objeto.DtHrCadastro,
                               DtCadastroText = objeto.DtHrCadastro.ToShortDateString(),
                           }).OrderByDescending(ob => ob.DtEmprestimo).ToList()
            };

            ViewBag.Title = string.Format("Relatório de {0}", "Empréstimos");
            ViewBag.Subtitle = string.Format(Constantes.GeradoEm_, Genericos.GetDateTimeFromBrazil());

            return new ViewAsPdf("EmprestimoSearchResultList", model)
            {
                FileName = "RelatorioEmprestimos.pdf",
                PageSize = Size.A4,
                PageOrientation = Orientation.Landscape,
                PageMargins = { Top = 1, Left = 1, Right = 1, Bottom = 1 }
            };
        }

        #endregion

        #region Relatório de empréstimos atrasados

        [HttpGet]
        public ActionResult EmitirReportEmprestimosAtrasados()
        {
            ViewBag.Title = GetModuleNamePlural();
            ViewBag.Subtitle = string.Format(Constantes.GerarRelatorioDe_, "empréstimos atrasados");
            ViewBag.Information = string.Format(Constantes.GerarRelatorioDe_AplicandoOFiltroAbaixoSeNadaForPreenchidoORelatorioPoderaDemorarParaSerGeradoE, "empréstimos atrasados");
            ViewBag.SearchWaterMark = "Pesquisar pelos campos: código do empréstimo, nome da pessoa, código do livro, título, gênero, autores, editora, local";

            return View();
        }

        [HttpPost]
        public ActionResult EmitirReportEmprestimosAtrasados(DevolucaoSearchModel searchModel)
        {
            var currentDate = Genericos.GetDateTimeFromBrazil().Date;

            var query = DevolucoesModel.GetBaseQuery(db, searchModel).OrderByDescending(q => q.DtEmprestimo);

            var model = new DevolucaoSearchViewModel
            {
                SearchModel = searchModel,
                Termo = searchModel.Search,
                FoundObjects = query.Count(),
                Objetos = (from objeto in query.ToList()
                           let pessoa = objeto.Pessoa
                           let livros = objeto.ItensEmprestimo.ToList()
                           let dtDevolucao = objeto.ItensEmprestimo.Select(s => s.DtDevolucao).FirstOrDefault()
                           let situacao = (objeto.Status == (int)Enumerations.Emprestimo.StatusEmprestimo.Emprestado
                           && objeto.DtPrevisaoDevolucao < currentDate ? (int)Enumerations.Devolucao.SituacaoDevolucao.Atrasado :
                           (int)Enumerations.Devolucao.SituacaoDevolucao.NoPrazo)
                           select new DevolucaoViewModel
                           {
                               Id = objeto.Id,
                               PessoaId = pessoa.Id,
                               PessoaText = pessoa.Nome,
                               LivrosText = string.Join("; ", livros.Select(x => x.Livro.Titulo).ToList()),
                               StatusId = (int)objeto.Status,
                               StatusText = EnumsHelper.GetText(typeof(Enumerations.Emprestimo.StatusEmprestimo), (int)objeto.Status),
                               SituacaoId = situacao,
                               SituacaoText = EnumsHelper.GetText(typeof(Enumerations.Devolucao.SituacaoDevolucao), situacao),
                               DtEmprestimo = objeto.DtEmprestimo,
                               DtEmprestimoText = objeto.DtEmprestimo.ToShortDateString(),
                               DtPrevisaoDevolucao = objeto.DtPrevisaoDevolucao,
                               DtPrevisaoDevolucaoText = objeto.DtPrevisaoDevolucao.HasValue ? objeto.DtPrevisaoDevolucao.Value.ToShortDateString() : string.Empty,
                               DtDevolucao = dtDevolucao,
                               DtDevolucaoText = dtDevolucao.ToShortDateString(),
                               DtCadastro = objeto.DtHrCadastro,
                               DtCadastroText = objeto.DtHrCadastro.ToShortDateString(),
                           })
                           .Where(q => q.SituacaoId == (int)Enumerations.Devolucao.SituacaoDevolucao.Atrasado)
                           .OrderByDescending(ob => ob.SituacaoId)
                           .ThenBy(ob => ob.DtPrevisaoDevolucao).ToList()
            };

            ViewBag.Title = string.Format("Relatório de {0}", "Empréstimos atrasados");
            ViewBag.Subtitle = string.Format(Constantes.GeradoEm_, Genericos.GetDateTimeFromBrazil());

            return new ViewAsPdf("EmprestimoAtrasadoSearchResultList", model)
            {
                FileName = "RelatorioEmprestimosAtrasados.pdf",
                PageSize = Size.A4,
                PageOrientation = Orientation.Landscape,
                PageMargins = { Top = 1, Left = 1, Right = 1, Bottom = 1 }
            };
        }

        #endregion

        #region Relatório de livros

        [HttpGet]
        public ActionResult EmitirReportLivros()
        {
            ViewBag.Title = GetModuleNamePlural();
            ViewBag.Subtitle = string.Format(Constantes.GerarRelatorioDe_, "livros");
            ViewBag.Information = string.Format(Constantes.GerarRelatorioDe_AplicandoOFiltroAbaixoSeNadaForPreenchidoORelatorioPoderaDemorarParaSerGeradoE, "livros");
            ViewBag.SearchWaterMark = "Pesquisar pelos campos: código, título, gênero, autores, editora, local";

            return View();
        }

        [HttpPost]
        public ActionResult EmitirReportLivros(LivroSearchModel searchModel)
        {
            var query = LivrosModel.GetBaseQuery(db, searchModel).OrderBy(q => q.Titulo);

            var model = new LivroSearchViewModel
            {
                SearchModel = searchModel,
                Termo = searchModel.Search,
                FoundObjects = query.Count(),
                Objetos = (from objeto in query.ToList()
                           let status = LivrosModel.GetStatus(objeto)
                           let totalVinculos = LivrosModel.GetTotalByItemLinkedRecords(db, objeto.Id)
                           select new LivroViewModel
                           {
                               Id = objeto.Id,
                               Codigo = objeto.Codigo,
                               StatusId = (int)status,
                               StatusText = EnumsHelper.GetText(typeof(Enumerations.Livro.StatusLivro), (int)status),
                               Titulo = objeto.Titulo,
                               Autor = objeto.Autor,
                               Genero = objeto.Genero,
                               Editora = objeto.Editora,
                               EstadoConservacaoId = (int)objeto.EstadoConservacao,
                               EstadoConservacaoText = EnumsHelper.GetText(typeof(Enumerations.Livro.EstadoConservacaoLivro), (int)objeto.EstadoConservacao),
                               BaixadoId = objeto.IsBaixado ? (int)Enumerations.Generico.SimOuNao.Sim : (int)Enumerations.Generico.SimOuNao.Nao,
                               BaixadoText = objeto.IsBaixado ? EnumsHelper.GetText(Enumerations.Generico.SimOuNao.Sim) : EnumsHelper.GetText(Enumerations.Generico.SimOuNao.Nao),
                               DtBaixa = objeto.DtBaixa,
                               DtBaixaText = objeto.DtBaixa.HasValue ? objeto.DtBaixa.Value.ToShortDateString() : string.Empty,
                               DtCadastro = objeto.DtHrCadastro,
                               DtCadastroText = objeto.DtHrCadastro.ToShortDateString(),
                               DtEdicao = objeto.DtHrEdicao,
                               DtEdicaoText = objeto.DtHrEdicao.HasValue ? objeto.DtHrEdicao.Value.ToShortDateString() : string.Empty,
                               TotalReservas = totalVinculos.Item1,
                               TotalEmprestimos = totalVinculos.Item2
                           }).OrderBy(ob => ob.Titulo).ToList()
            };

            ViewBag.Title = string.Format("Relatório de {0}", "livros");
            ViewBag.Subtitle = string.Format(Constantes.GeradoEm_, Genericos.GetDateTimeFromBrazil());

            return new ViewAsPdf("LivroSearchResultList", model)
            {
                FileName = "RelatorioLivros.pdf",
                PageSize = Size.A4,
                PageOrientation = Orientation.Landscape,
                PageMargins = { Top = 1, Left = 1, Right = 1, Bottom = 1 }
            };
        }

        #endregion

        #region Relatório de reservas

        [HttpGet]
        public ActionResult EmitirReportReservas()
        {
            ViewBag.Title = GetModuleNamePlural();
            ViewBag.Subtitle = string.Format(Constantes.GerarRelatorioDe_, "reservas");
            ViewBag.Information = string.Format(Constantes.GerarRelatorioDe_AplicandoOFiltroAbaixoSeNadaForPreenchidoORelatorioPoderaDemorarParaSerGeradoE, "reservas");
            ViewBag.SearchWaterMark = "Pesquisar pelos campos: código da reserva, nome da pessoa, código do livro, título, gênero, autores, editora, local";

            return View();
        }

        [HttpPost]
        public ActionResult EmitirReportReservas(ReservaSearchModel searchModel)
        {
            var userData = GetUserData();

            var query = ReservasModel.GetBaseQuery(db, searchModel, userData).OrderByDescending(q => q.DtPrevisaoEmprestimo);

            var model = new ReservaSearchViewModel
            {
                SearchModel = searchModel,
                Termo = searchModel.Search,
                FoundObjects = query.Count(),
                Objetos = (from objeto in query.ToList()
                           let pessoa = objeto.Pessoa
                           let livro = objeto.Livro
                           select new ReservaViewModel
                           {
                               Id = objeto.Id,
                               PessoaId = pessoa.Id,
                               PessoaText = pessoa.Nome,
                               LivroId = livro.Id,
                               LivroText = livro.Titulo,
                               StatusId = (int)objeto.Status,
                               StatusText = EnumsHelper.GetText(typeof(Enumerations.Reserva.StatusReserva), (int)objeto.Status),
                               DtPrevisaoEmprestimo = objeto.DtPrevisaoEmprestimo,
                               DtPrevisaoEmprestimoText = objeto.DtPrevisaoEmprestimo.HasValue ? objeto.DtPrevisaoEmprestimo.Value.ToShortDateString() : string.Empty,
                               DtCadastro = objeto.DtHrCadastro,
                               DtCadastroText = objeto.DtHrCadastro.ToShortDateString(),
                           }).OrderBy(ob => ob.StatusId).ThenByDescending(ob => ob.DtPrevisaoEmprestimo).ToList()
            };

            ViewBag.Title = string.Format("Relatório de {0}", "reservas");
            ViewBag.Subtitle = string.Format(Constantes.GeradoEm_, Genericos.GetDateTimeFromBrazil());

            return new ViewAsPdf("ReservaSearchResultList", model)
            {
                FileName = "RelatorioReservas.pdf",
                PageSize = Size.A4,
                PageOrientation = Orientation.Landscape,
                PageMargins = { Top = 1, Left = 1, Right = 1, Bottom = 1 }
            };
        }

        #endregion
    }
}