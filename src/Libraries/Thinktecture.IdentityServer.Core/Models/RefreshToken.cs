/*
 * Copyright (c) Dominick Baier, Brock Allen.  All rights reserved.
 * see license.txt
 */

using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Thinktecture.IdentityServer.Models
{
    public class RefreshToken
    {
        public string TokenIdentifier { get; set; }

        public string ClientId { get; set; }

        public string UserName { get; set; }

        public string Scope { get; set; }
    }
}
