﻿using iTest.Data.Models.Contracts;
using iTest.Data.Repository.Contracts;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using System;
using System.Linq;

namespace iTest.Data.Repository.Implementations
{
    public class EfRepository<T> : IRepository<T>
        where T : class, IDeletable
    {
        private readonly iTestDbContext context;

        public EfRepository(iTestDbContext context)
        {
            this.context = context;
        }

        public IQueryable<T> All => this.context.Set<T>().Where(x => !x.IsDeleted);

        public IQueryable<T> AllAndDeleted => this.context.Set<T>();

        public void Add(T entity)
        {
            EntityEntry entry = context.Entry(entity);

            if (entry.State != EntityState.Detached)
            {
                entry.State = EntityState.Added;
            }
            else
            {
                this.context.Set<T>().Add(entity);
            }
        }

        public void Delete(T entity)
        {
            entity.IsDeleted = true;
            entity.DeletedOn = DateTime.Now;

            var entry = this.context.Entry(entity);
            entry.State = EntityState.Modified;
        }

        public void Update(T entity)
        {
            EntityEntry entry = this.context.Entry(entity);
            if (entry.State == EntityState.Detached)
            {
                this.context.Set<T>().Attach(entity);
            }

            entry.State = EntityState.Modified;
        }
    }
}
