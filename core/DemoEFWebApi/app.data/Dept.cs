using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace app.data
{
    [Table("Dept")]
    public class Dept
    {
        [Key]
        public int Deptno { get; set; }
        public string Dname { get; set; }        
    }
}
