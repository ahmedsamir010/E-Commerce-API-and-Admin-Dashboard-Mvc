using Store.Core.Entities;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;

namespace Store.Core.Specification
{
    public interface ISpecification<T> where T : BaseEntity
    {
        Expression<Func<T, bool>> Criteria { get; }
        List<Expression<Func<T, object>>> Includes { get; }
        Expression<Func<T, object>> OrderBy { get; }
        Expression<Func<T, object>> OrderByDescending { get; }
        public int SkipCount { get; }
        public int TakeCount { get; }

        public bool IsPaginationEnabled { get;  }
    
    }
}
