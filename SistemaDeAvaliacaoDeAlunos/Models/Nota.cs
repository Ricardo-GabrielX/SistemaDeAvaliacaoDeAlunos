using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace WebApplication1.Models
{
    public class Nota
    {
        public Disciplina Disciplina { get; set; }

        [Range(0, 10, ErrorMessage = "A nota deve estar entre 0 e 10.")]
        public double Valor { get; set; }
    }
}