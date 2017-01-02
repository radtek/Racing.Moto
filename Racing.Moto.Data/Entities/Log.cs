using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Racing.Moto.Data.Entities
{
    [Table(nameof(Log))]
    public class Log
    {
        [Key]
        public int LogId { get; set; }

        public DateTime CreateTime { get; set; }

        [StringLength(300)]
        public string Origin { get; set; }

        [StringLength(300)]
        public string LogLevel { get; set; }

        [StringLength(300)]
        public string Logger { get; set; }

        public string Message { get; set; }

        public string Exception { get; set; }

        public string StackTrace { get; set; }
    }
}
