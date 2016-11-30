﻿using GRA.Domain.Repository;
using GRA.Domain.Model;
using Microsoft.Extensions.Logging;
using System.Threading.Tasks;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using AutoMapper.QueryableExtensions;

namespace GRA.Data.Repository
{
    public class BranchRepository
        : AuditingRepository<Model.Branch, Branch>, IBranchRepository
    {
        public BranchRepository(ServiceFacade.Repository repositoryFacade,
            ILogger<BranchRepository> logger) : base(repositoryFacade, logger)
        {
        }
        public async Task<IEnumerable<Branch>> GetAllAsync(int systemId)
        {
            return await DbSet
                .AsNoTracking()
                .Where(_ => _.SystemId == systemId)
                .OrderBy(_ => _.Name)
                .ProjectTo<Branch>()
                .ToListAsync();
        }
    }
}
