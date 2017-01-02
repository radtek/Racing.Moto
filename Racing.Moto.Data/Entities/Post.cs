using Racing.Moto.Data.Enums;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Racing.Moto.Data.Entities
{
    [Table(nameof(Post))]
    public partial class Post
    {
        [Key]
        public int PostId { get; set; }

        [StringLength(1000)]
        public string Title { get; set; }

        public string PostContent { get; set; }

        /// <summary>
        /// 状态:0-待审, 1-通过, 2-驳回
        /// </summary>
        public PostStatus PostStatus { get; set; }

        public DateTime CreateTime { get; set; }


        [ForeignKey(nameof(User))]
        public int UserId { get; set; }
        public virtual User User { get; set; }
    }
}
