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
        /*
        /// <summary>
        ///
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        SimpleResponse<TDto> Create(TDto entity);

        /// <summary>
        ///
        /// </summary>
        /// <param name="oid"></param>
        /// <returns></returns>
        SimpleResponse<TDto> Read(object oid);

        /// <summary>
        ///
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        SimpleResponse Update(TDto entity);

        /// <summary>
        /// Deletes entity.
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        SimpleResponse Delete(TDto entity);

        /// <summary>
        /// Reads All Entity records.
        /// </summary>
        /// <returns></returns>
        SimpleResponse<List<TDto>> ReadAll();
        */
    }
}