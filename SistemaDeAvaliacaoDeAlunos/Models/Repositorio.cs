using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using WebApplication1.Models;

namespace SistemaDeAvaliacaoDeAlunos.Models
{
    public static class Repositorio
    {
        public static List<Aluno> Alunos { get; set; } = new List<Aluno>();

        public static Aluno ObterPorId(int id)
        {
            return Alunos.FirstOrDefault(a => a.Id == id);
        }

        public static void Atualizar(Aluno alunoEditado)
        {
            var alunoOriginal = ObterPorId(alunoEditado.Id);

            if (alunoOriginal != null)
            {
                alunoOriginal.Nome = alunoEditado.Nome;
                alunoOriginal.RA = alunoEditado.RA;
                alunoOriginal.Data = alunoEditado.Data;

                for (int i = 0; i < alunoOriginal.Notas.Count; i++)
                {
                    alunoOriginal.Notas[i].Bimestre1 = alunoEditado.Notas[i].Bimestre1;
                    alunoOriginal.Notas[i].Bimestre2 = alunoEditado.Notas[i].Bimestre2;
                    alunoOriginal.Notas[i].Bimestre3 = alunoEditado.Notas[i].Bimestre3;
                    alunoOriginal.Notas[i].Bimestre4 = alunoEditado.Notas[i].Bimestre4;
                }

                alunoOriginal.AtualizarStatus();
            }
        }

    }
}