﻿using Rubik.Identity.Share.Entity.BaseEntity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Rubik.Identity.Share.Entity
{
    /// <summary>
    /// 岗位&用户
    /// </summary>
    [Table(Name ="tb_relation_post_user")]
    public class TbRelationPostUser:BaseNewEntity
    {
        public int DepartmentID { get; set; }

        public int UserID { get; set; }
    }
}
