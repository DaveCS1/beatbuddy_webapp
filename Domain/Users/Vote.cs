﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BB.BL.Domain.Playlists;

namespace BB.BL.Domain.Users
{
    public class Vote
    {
        public int Id { get; set; }
        public int Score { get; set; }
        public User User { get; set; }
    }
}
