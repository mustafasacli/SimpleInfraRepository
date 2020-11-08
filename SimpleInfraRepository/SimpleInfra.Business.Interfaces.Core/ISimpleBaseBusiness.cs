namespace SimpleInfra.Business.Interfaces.Core
{
    using SimpleInfra.Dto.Core;
    using SimpleInfra.Entity.Core;

    /// <summary>
    /// Base Generic Business Interface.
    /// </summary>
    /// <typeparam name="TDto"></typeparam>
    /// <typeparam name="TEntity"></typeparam>
    public interface ISimpleBaseBusiness<TDto, TEntity>
        where TDto : SimpleBaseDto, new()
           where TEntity : SimpleBaseEntity, new()
    {
    }
}