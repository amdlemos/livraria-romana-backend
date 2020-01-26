using System;
using System.Collections.Generic;
using System.Text;

namespace LivrariaRomana.Domain.DTO
{
    public class BookUpdateAmountDTO
    {
        public int id { get; set; }
        public int addToAmount { get; set; }
        public int removeToAmount { get; set; }
    }
}
