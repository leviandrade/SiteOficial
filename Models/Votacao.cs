using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SiteOficial.Models
{
    public class Votacao
    {
        public int Id { get; set; }
        public string Nome { get; set; }
        public int Idade { get; set; }
        public int Voto { get; set; }
    }
}