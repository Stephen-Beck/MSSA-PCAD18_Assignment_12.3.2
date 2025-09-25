using Assignment_12._3._2.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Assignment_12._3._2.Data
{
    public class TaskDataContext : DbContext
    {
        public TaskDataContext(DbContextOptions options) : base(options)
        {
        }

        public DbSet<TaskToDo> Tasks { get; set; }
    }
}
