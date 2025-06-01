using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;
using WebApplication1.Models;


namespace WebApplication1.Models
{
    public class Aluno
    {
        public int Id { get; set; }


        [Required(ErrorMessage = "O nome é obrigatório.")]
        public string Nome { get; set; }

        [Required(ErrorMessage = "O RA é obrigatório. ")]
        public string RA { get; set; }

        [Required(ErrorMessage = "A data de nascimento é obrigatória.")]
        [Display(Name = "Data de Nascimento")]
        [DisplayFormat(DataFormatString = "{0:dd/MM/yyyy}", ApplyFormatInEditMode = true)]
        public DateTime Data { get; set; }

        public List<Nota> Notas { get; set; } = new List<Nota>();
        public string Status { get; set; }


        public static void GerarLista(HttpSessionStateBase session)
        {
            if (session["ListaAluno"] != null)
            {
                if (((List<Aluno>)session["ListaAluno"]).Count > 0)
                {
                    return;
                }
            }

            var disciplinasFixas = Disciplina.DisciplinasFixas;

            var lista = new List<Aluno>
            {
                new Aluno { Id = 1, Nome = "Lucas Martins", RA = "78945612301", Data = new DateTime(2025, 03, 01) },
                new Aluno { Id = 2, Nome = "Mariana Souza", RA = "12378945602", Data = new DateTime(2025, 03, 02) },
                new Aluno { Id = 3, Nome = "Carlos Eduardo", RA = "45612378903", Data = new DateTime(2025, 03, 03) },
                new Aluno { Id = 4, Nome = "Fernanda Lima", RA = "98765432104", Data = new DateTime(2025, 03, 04) },
                new Aluno { Id = 5, Nome = "João Pedro", RA = "32198765405", Data = new DateTime(2025, 03, 05) },
                new Aluno { Id = 6, Nome = "Ana Clara", RA = "65432198706", Data = new DateTime(2025, 03, 06) },
                new Aluno { Id = 7, Nome = "Rafael Silva", RA = "25836914707", Data = new DateTime(2025, 03, 07) },
                new Aluno { Id = 8, Nome = "Beatriz Costa", RA = "14725836908", Data = new DateTime(2025, 03, 08) },
                new Aluno { Id = 9, Nome = "Henrique Oliveira", RA = "36914725809", Data = new DateTime(2025, 03, 09) },
                new Aluno { Id = 10, Nome = "Camila Ribeiro", RA = "85274196310", Data = new DateTime(2025, 03, 10) }
            };

            foreach (var aluno in lista)
            {
                aluno.Notas = new List<Nota>();

                foreach (var disciplina in disciplinasFixas)
                {
                    double valorNota = 0;

                    if (aluno.Id == 1)
                        valorNota = disciplina.Nome == "Matemática" ? 8 : 7;

                    else if (aluno.Id == 2)
                        valorNota = disciplina.Nome == "Português" ? 5 : 6.5;

                    else if (aluno.Id == 3) 
                        valorNota = 4;

                    aluno.Notas.Add(new Nota
                    {
                        Disciplina = disciplina,
                        Valor = valorNota
                    });
                }
            }

            //session.Remove("ListaAluno");
            //session.Add("ListaAluno", lista);

            session["ListaAluno"] = lista;
        }

        public void Adicionar(HttpSessionStateBase session)
        {
            var lista = session["ListaAluno"] as List<Aluno> ?? new List<Aluno>();
            this.Id = lista.Count > 0 ? lista.Max(a => a.Id) + 1 : 1;

            // Só adiciona as notas com as disciplinas fixas se ainda não tiver notas
            if (this.Notas == null || this.Notas.Count == 0)
            {
                var disciplinasFixas = Disciplina.DisciplinasFixas;
                this.Notas = disciplinasFixas.Select(d => new Nota
                {
                    Disciplina = d,
                    Valor = 0
                }).ToList();
            }

            lista.Add(this);
            session["ListaAluno"] = lista;
        }



        public static Aluno Procurar(HttpSessionStateBase session, int id)
        {
            var lista = session["ListaAluno"] as List<Aluno>;
            if (lista != null)
            {
                return lista.FirstOrDefault(a => a.Id == id);
            }
            return null;
        }

        public void Excluir(HttpSessionStateBase session)
        {
            var lista = (List<Aluno>)session["ListaAluno"];
            lista.RemoveAll(a => a.Id == this.Id);
            session["ListaAluno"] = lista;
        }

        public void Editar(HttpSessionStateBase session, int id)
        {
            var lista = session["ListaAluno"] as List<Aluno>;
            var aluno = lista.FirstOrDefault(a => a.Id == id);

            if (aluno != null)
            {
                aluno.Nome = this.Nome;
                aluno.RA = this.RA;
                aluno.Data = this.Data;

                var disciplinas = Disciplina.DisciplinasFixas;

                for (int i = 0; i < aluno.Notas.Count; i++)
                {
                    aluno.Notas[i].Bimestre1 = this.Notas[i].Bimestre1;
                    aluno.Notas[i].Bimestre2 = this.Notas[i].Bimestre2;
                    aluno.Notas[i].Bimestre3 = this.Notas[i].Bimestre3;
                    aluno.Notas[i].Bimestre4 = this.Notas[i].Bimestre4;
                    aluno.Notas[i].Disciplina = disciplinas[i];
                }

                aluno.AtualizarStatus();
            }

            session["ListaAluno"] = lista;
        }


        public double CalcularMedia()
        {
            return Notas.Any() ? Notas.Average(n => n.Media) : 0.0;
        }

        public void AtualizarStatus()
        {
            double mediaGeral = CalcularMedia();
            Status = mediaGeral >= 5.0 ? "Aprovado" : "Reprovado";
        }



    }
}   