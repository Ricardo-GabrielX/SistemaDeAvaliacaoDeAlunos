using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WebApplication1.Models
{
    public class Disciplina
    {
        public int Id { get; set; }
        public string Nome { get; set; }

        public static readonly List<Disciplina> DisciplinasFixas = new List<Disciplina>
        {
        new Disciplina { Id = 1, Nome = "Português" },
        new Disciplina { Id = 2, Nome = "Matemática" },
        new Disciplina { Id = 3, Nome = "História" },
        new Disciplina { Id = 4, Nome = "Geografia" },
        };
    }
}