﻿using Rubik.Identity.Share.Entity.BaseEntity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Rubik.Identity.Share.Entity
{
    [Table(Name ="tb_department")]
    public class TbDepartment:BaseFullEntity
    {
        [Column(IsNullable =false,Position =2)]
        public string? Name { get; set; }

        [Column(IsNullable = true,Position =3)]
        public int? ParentID { get; set; }

        [Column(IsNullable =true)]
        public string? Description { get; set; }


        public List<TbDepartmentPost>? Posts { get; set; }
    }
}
