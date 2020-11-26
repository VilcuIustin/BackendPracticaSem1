﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Backend.Entities.Models;
using Microsoft.EntityFrameworkCore;


namespace Backend.Entities
{
    public class BackendContext : DbContext
    {
        public BackendContext(DbContextOptions options): base(options)
        {

        }
        public DbSet<User> Users { get; set; }
        public DbSet<Post> Posts { get; set; }
        public DbSet<Image> Images { get; set; }

    }
}
