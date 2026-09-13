using EducationSystem.Domain.Entities.Common;
using System.Linq.Expressions;

namespace EducationSystem.Application.Abstarctions.Persistence.Specifications;

public interface ISpecification<TEntity> where TEntity : BaseAuditableEntity
{
    Expression<Func<TEntity, bool>>? Criteria { get; } // where condition
    List<Expression<Func<TEntity, object>>> Includes { get; } // navigation properties to include


}