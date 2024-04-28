﻿using System;
using System.Collections.Generic;
using System.Text;
using Volo.Abp.Application.Dtos;

namespace ShopEcommerce.Admin.Roles
{
    public class RoleDto : EntityDto<Guid>
    {
        public string? Name { get; set; }
        public string? Description { get; set; }
    }
}