﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Rubik.Identity.Share.Entity
{
    [Table(Name = "tb_relation_position_user")]
    public class TbRelationPositionUser : BaseNewEntity
    {
        public int PositionID { get; set; }

        public int UserID { get; set; }
    }
}