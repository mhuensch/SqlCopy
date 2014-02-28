using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Run00.SqlCopy
{
    public interface IEntityFilter
    {
        Type EntityType { get; }
        IQueryable Filter(IQueryable entities, IDbRepository repository);
    }

    public interface IEntityFilter<TEntity> : IEntityFilter
    {
        IQueryable<TEntity> Filter(IQueryable<TEntity> entities);
    }

    public interface IEntityFilter<TEntity, TReleatedEntity> : IEntityFilter where TReleatedEntity : class
    {
        IQueryable<TEntity> Filter(IQueryable<TEntity> entities, IQueryable<TReleatedEntity> releatedEntities);
    }

    public interface IEntityFilter<TEntity, TReleatedEntity, TOtherEntity> : IEntityFilter where TReleatedEntity : class
    {
        IQueryable<TEntity> Filter(IQueryable<TEntity> entities, IQueryable<TReleatedEntity> releatedEntities, IQueryable<TOtherEntity> otherEntities);
    }

    public interface IEntityFilter<TEntity, TReleatedEntity, TOtherEntity, TTHirdEntity> : IEntityFilter where TReleatedEntity : class
    {
        IQueryable<TEntity> Filter(IQueryable<TEntity> entities, IQueryable<TReleatedEntity> releatedEntities, IQueryable<TOtherEntity> otherEntities, IQueryable<TTHirdEntity> moreEntities);
    }

    public abstract class BaseFilter : IEntityFilter
    {
        public abstract Type EntityType { get; }

        public abstract IQueryable RunFilter(IQueryable entities, IDbRepository repository);

        IQueryable IEntityFilter.Filter(IQueryable entities, IDbRepository repository)
        {
            var origElementType = entities.ElementType;

            if (EntityType.IsAssignableFrom(origElementType) == false)
                return entities;

            var result = RunFilter(entities, repository);
            var recastResult = ReCast(result, origElementType);
            return recastResult;
        }

        private static IQueryable ReCast(IQueryable source, Type toType)
        {
            if (castMethod == null)
                castMethod = typeof(Queryable).GetMethod("Cast");

            var typeCast = castMethod.MakeGenericMethod(new Type[] { toType });
            return typeCast.Invoke(null, new object[] { source }) as IQueryable;
        }

        private static MethodInfo castMethod;
    }

    public abstract class BaseEntityFilter<TEntity> : BaseFilter, IEntityFilter<TEntity>
    {
        public abstract IQueryable<TEntity> Filter(IQueryable<TEntity> entities);

        public override Type EntityType { get { return typeof(TEntity); } }

        public override IQueryable RunFilter(IQueryable entities, IDbRepository repository)
        {
            return Filter((IQueryable<TEntity>)entities);
        }
    }

    public abstract class BaseReleatedEntityFilter<TEntity, TReleatedEntity> : BaseFilter, IEntityFilter<TEntity, TReleatedEntity> where TReleatedEntity : class
    {
        public abstract IQueryable<TEntity> Filter(IQueryable<TEntity> entities, IQueryable<TReleatedEntity> releatedEntities);

        public abstract string ReleatedEntityName { get; }

        public override Type EntityType { get { return typeof(TEntity); } }

        public override IQueryable RunFilter(IQueryable entities, IDbRepository repository)
        {
            return Filter((IQueryable<TEntity>)entities, (IQueryable<TReleatedEntity>)repository.GetEntities(ReleatedEntityName));
        }
    }

    //public interface IEntityFilter<
    //public abstract class BaseChainedFilter<TEntity, TUpstreamFilter> : BaseFilter where TUpstreamFilter : class
    //{
    //}

    public abstract class BaseThreeWayFilter<TEntity, TFirstEntity, TSecondEntity> : BaseFilter, IEntityFilter<TEntity, TFirstEntity, TSecondEntity>
        where TEntity : class
        where TFirstEntity : class
    {        
        public abstract IQueryable<TEntity> Filter(IQueryable<TEntity> entities, IQueryable<TFirstEntity> releatedEntities, IQueryable<TSecondEntity> otherEntities);

        public abstract string ReleatedEntityName { get; }

        public abstract string SecondEntityName { get; }

        public override Type EntityType { get { return typeof(TEntity); } }

        public override IQueryable RunFilter(IQueryable entities, IDbRepository repository)
        {
            return Filter((IQueryable<TEntity>)entities, (IQueryable<TFirstEntity>)repository.GetEntities(ReleatedEntityName), (IQueryable<TSecondEntity>)repository.GetEntities(SecondEntityName));
        }
    }

    public abstract class BaseFourWayFilter<TEntity, TFirstEntity, TSecondEntity, TTHirdEntity> : BaseFilter, IEntityFilter<TEntity, TFirstEntity, TSecondEntity, TTHirdEntity>
        where TEntity : class
        where TFirstEntity : class
        where TTHirdEntity : class
    {
        public abstract IQueryable<TEntity> Filter(IQueryable<TEntity> entities, IQueryable<TFirstEntity> releatedEntities, IQueryable<TSecondEntity> otherEntities, IQueryable<TTHirdEntity> moreEntities);

        public abstract string ReleatedEntityName { get; }

        public abstract string SecondEntityName { get; }

        public abstract string ThirdEntityName { get; }

        public override Type EntityType { get { return typeof(TEntity); } }

        public override IQueryable RunFilter(IQueryable entities, IDbRepository repository)
        {
            return Filter((IQueryable<TEntity>)entities, (IQueryable<TFirstEntity>)repository.GetEntities(ReleatedEntityName), (IQueryable<TSecondEntity>)repository.GetEntities(SecondEntityName), (IQueryable<TTHirdEntity>)repository.GetEntities(ThirdEntityName));
        }
    }
}
