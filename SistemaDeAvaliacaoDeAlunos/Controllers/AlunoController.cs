using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using WebApplication1.Models;
using iTextSharp.text;
using iTextSharp.text.pdf;
using System.IO;
using OfficeOpenXml;
using System.ComponentModel;
//using ClosedXML.Excel;
using System.Drawing.Printing;
using System.Drawing;
using System.Xml.Linq;
using SistemaDeAvaliacaoDeAlunos.Models;
using iTextSharp.text.pdf.draw;


namespace WebApplication1.Controllers
{
    public class AlunoController : Controller
    {
        // GET: Aluno
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult Listar()
        {
            Aluno.GerarLista(Session);

            return View(Session["ListaAluno"] as List<Aluno>);
        }

        public ActionResult Exibir(int id)
        {
            ViewBag.Id = id;
            var aluno = Aluno.Procurar(Session, id);
            return View(aluno);
        }


        [HttpPost]
        public ActionResult DeleteAjax(int id)
        {
            var alunos = Session["ListaAluno"] as List<Aluno>;
            var aluno = alunos?.FirstOrDefault(a => a.Id == id);

            if (aluno == null)
                return Json(new { sucesso = false, mensagem = "Aluno não encontrado" });

            alunos.Remove(aluno);
            Session["ListaAluno"] = alunos;
            return Json(new { sucesso = true });
        }

        public ActionResult Create()
        {
            var disciplinasFixas = Disciplina.DisciplinasFixas;
            var aluno = new Aluno
            {
                Notas = disciplinasFixas.Select(d => new Nota
                {
                    Disciplina = d,
                    Valor = 0 // ou null, se preferir
                }).ToList()
            };

            return View(aluno);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(Aluno aluno)
        {
            var disciplinas = Disciplina.DisciplinasFixas;
            for (int i = 0; i < aluno.Notas.Count; i++)
            {
                aluno.Notas[i].Disciplina = disciplinas[i];
            }

            aluno.AtualizarStatus();

            aluno.Adicionar(Session);

            return RedirectToAction("Listar");
        }
        public ActionResult Editar(int id)
        {
            var aluno = Aluno.Procurar(Session, id);

            var disciplinas = Disciplina.DisciplinasFixas;
            for (int i = 0; i < aluno.Notas.Count; i++)
            {
                aluno.Notas[i].Disciplina = disciplinas[i];
            }

            return View(aluno);
        }



        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Editar(int id, Aluno aluno)
        {
            if (ModelState.IsValid)
            {
                aluno.Editar(Session, id);
                return RedirectToAction("Listar");
            }

            return View(aluno);
        }

        public ActionResult AdicionarNota(int id)
        {
            var aluno = Aluno.Procurar(Session, id);
            ViewBag.Id = id;
            return View(new Nota());
        }

        [HttpPost]
        public ActionResult AdicionarNota(int id, Nota nota)
        {
            var aluno = Aluno.Procurar(Session, id);
            aluno.Notas.Add(nota);
            return RedirectToAction("Exibir", new { id });
        }


        public ActionResult GerarPdf()
        {
            List<Aluno> listaAlunos = Session["ListaAluno"] as List<Aluno>;

            if (listaAlunos == null || !listaAlunos.Any())
            {
                return Content("Nenhum aluno encontrado para gerar o PDF");
            }

            using (MemoryStream memoryStream = new MemoryStream())
            {
                Document document = new Document(PageSize.A4, 50, 50, 25, 25);

                try
                {
                    PdfWriter writer = PdfWriter.GetInstance(document, memoryStream);
                    document.Open();

                    // Fontes
                    var titleFont = FontFactory.GetFont(FontFactory.HELVETICA_BOLD, 18);
                    var headerFont = FontFactory.GetFont(FontFactory.HELVETICA_BOLD, 12);
                    var subHeaderFont = FontFactory.GetFont(FontFactory.HELVETICA_BOLD, 11);
                    var normalFont = FontFactory.GetFont(FontFactory.HELVETICA, 10);

                    document.Add(new Paragraph("📚 Lista de Alunos com Notas", titleFont));
                    document.Add(Chunk.NEWLINE);

                    foreach (var aluno in listaAlunos)
                    {
                        // Informações do aluno
                        PdfPTable infoTable = new PdfPTable(3);
                        infoTable.WidthPercentage = 100;
                        infoTable.SpacingAfter = 10;

                        infoTable.AddCell(new Phrase("Nome:", headerFont));
                        infoTable.AddCell(new Phrase("RA:", headerFont));
                        infoTable.AddCell(new Phrase("Data de Nascimento:", headerFont));

                        infoTable.AddCell(new Phrase(aluno.Nome ?? "", normalFont));
                        infoTable.AddCell(new Phrase(aluno.RA ?? "", normalFont));
                        infoTable.AddCell(new Phrase(aluno.Data.ToString("dd/MM/yyyy"), normalFont));

                        document.Add(infoTable);

                        // Tabela de Notas
                        PdfPTable notaTable = new PdfPTable(6);
                        notaTable.WidthPercentage = 100;

                        // Cabeçalhos
                        notaTable.AddCell(new Phrase("Disciplina", subHeaderFont));
                        notaTable.AddCell(new Phrase("Bim. 1", subHeaderFont));
                        notaTable.AddCell(new Phrase("Bim. 2", subHeaderFont));
                        notaTable.AddCell(new Phrase("Bim. 3", subHeaderFont));
                        notaTable.AddCell(new Phrase("Bim. 4", subHeaderFont));
                        notaTable.AddCell(new Phrase("Média", subHeaderFont));

                        if (aluno.Notas != null && aluno.Notas.Any())
                        {
                            foreach (var nota in aluno.Notas)
                            {
                                notaTable.AddCell(new Phrase(nota.Disciplina?.Nome ?? "", normalFont));
                                notaTable.AddCell(new Phrase(nota.Bimestre1.ToString("F1"), normalFont));
                                notaTable.AddCell(new Phrase(nota.Bimestre2.ToString("F1"), normalFont));
                                notaTable.AddCell(new Phrase(nota.Bimestre3.ToString("F1"), normalFont));
                                notaTable.AddCell(new Phrase(nota.Bimestre4.ToString("F1"), normalFont));
                                notaTable.AddCell(new Phrase(nota.Media.ToString("F2"), normalFont));
                            }
                        }
                        else
                        {
                            PdfPCell cell = new PdfPCell(new Phrase("Nenhuma nota cadastrada", normalFont));
                            cell.Colspan = 6;
                            cell.HorizontalAlignment = Element.ALIGN_CENTER;
                            notaTable.AddCell(cell);
                        }

                        document.Add(notaTable);

                        // Status geral do aluno
                        document.Add(new Paragraph($"Status: {aluno.Status}", normalFont));
                        document.Add(new Paragraph(" "));
                        document.Add(new LineSeparator(1f, 100f, BaseColor.LIGHT_GRAY, Element.ALIGN_CENTER, -2));
                        document.Add(new Paragraph(" "));
                    }
                }
                finally
                {
                    if (document.IsOpen())
                        document.Close();
                }

                return File(memoryStream.ToArray(), "application/pdf", "ListaCompletaAlunos.pdf");
            }
        }


    }

}
