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

        public double Bimestre1 { get; set; }
        public double Bimestre2 { get; set; }
        public double Bimestre3 { get; set; }
        public double Bimestre4 { get; set; }

        public double Media
        {
            get
            {
                return (Bimestre1 + Bimestre2 + Bimestre3 + Bimestre4) / 4;
            }
        }

        [Range(0, 10, ErrorMessage = "A nota deve estar entre 0 e 10.")]
        public double Valor { get; set; }
    }
}