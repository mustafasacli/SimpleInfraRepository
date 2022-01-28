namespace SimpleInfra.Business.Core
{
    using SimpleFileLogging;
    using SimpleFileLogging.Enums;
    using SimpleFileLogging.Interfaces;
    using SimpleInfra.Business.Interfaces.Core;
    using SimpleInfra.Common.Core;
    using SimpleInfra.Common.Extensions;
    using SimpleInfra.Common.Response;
    using SimpleInfra.Data;
    using SimpleInfra.Dto.Core;
    using SimpleInfra.Entity.Core;
    using SimpleInfra.IoC;
    using SimpleInfra.Mapping;
    using SimpleInfra.UoW;
    using SimpleInfra.UoW.Interfaces;
    using SimpleInfra.Validation;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Linq.Expressions;

    /// <summary>
    /// Gsb Base Business class.
    /// </summary>
    /// <typeparam name="TDto"></typeparam>
    /// <typeparam name="TEntity"></typeparam>
    public abstract class SimpleBaseBusiness<TDto, TEntity/*, TRepository*/> : ISimpleBaseBusiness<TDto, TEntity>, IDisposable
            where TDto : SimpleBaseDto, new()
            where TEntity : SimpleBaseEntity, new()
        //where TRepository : ISimpleDataRepository<TEntity>, new()
    {
        private ISimpleDataRepository<TEntity> repository;

        private static Lazy<ISimpleUnitOfWork> lazyInstance = new Lazy<ISimpleUnitOfWork>(
            () =>
            {
                return new SimpleUnitOfWork();
            }, isThreadSafe: true
            );

        /// <summary>
        /// Base Repository
        /// </summary>
        protected ISimpleDataRepository<TEntity> Repository
        {
            get
            {
                if (repository == null)
                    repository = GetRepository();

                if (repository.IsContextDisposedOrNull())
                    repository = GetRepository();

                var isDisposed = false;

                try { var isDebug = repository.LogDebug; }
                catch (ObjectDisposedException dex)
                { isDisposed = true; Logger?.Error(dex); }

                if (isDisposed)
                {
                    repository = null;
                    repository = GetRepository();
                }

                return repository;
            }
        }

        /// <summary>
        /// </summary>
        protected ISimpleUnitOfWork SimpleUnitOfWork
        { get { return lazyInstance.Value; } }

        /// <summary>
        /// Creates new repository.
        /// </summary>
        /// <returns>TRepository instance</returns>
        protected ISimpleDataRepository<TEntity> GetRepository()
        {
            var repo = SimpleUnitOfWork.GetRepository<TEntity>();
            return repo;
        }

        /// <summary>
        /// protected constructor
        /// </summary>
        protected SimpleBaseBusiness()
        {
            Logger = SimpleFileLogger.Instance;
            Logger.LogDateFormatType = SimpleLogDateFormats.Hour;
            repository = GetRepository();
        }

        /// <summary>
        /// Gets Logger instance.
        /// </summary>
        public ISimpleLogger Logger
        { get; protected set; }

        /// <summary>
        /// Gets Instance of class.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        protected T GetInstance<T>() where T : class
        { return SimpleIoC.Instance.GetInstance<T>(); }

        /// <summary>
        /// Insert record internally and returns Dto object.
        /// Dto nesnesini, entity nesnesine dönüştürüp veritabanına
        /// kaydeder. sonuç olarak SimpleResponse{Dto} nesnesini döner.
        /// </summary>
        /// <param name="dto"></param>
        /// <param name="autoSave"></param>
        /// <returns></returns>
        protected virtual SimpleResponse<TDto> InsertInternal(TDto dto, bool autoSave = true)
        {
            if (dto == null)
                return new SimpleResponse<TDto> { ResponseCode = BusinessResponseValues.NullDtoValue };

            var response = new SimpleResponse<TDto>();
            var entity = MapReverse(dto);

            var validation = entity.Validate();
            if (validation.HasError)
            {
                return new SimpleResponse<TDto>
                {
                    Data = dto,
                    ResponseCode = BusinessResponseValues.ValidationErrorResult,
                    ResponseMessage = validation.AllValidationMessages
                };
            }

            this.Repository.Add(entity);
            if (autoSave)
                response.ResponseCode = this.Repository.SaveChanges();

            var result = Map(entity);
            response.Data = result;

            return response;
        }

        /// <summary>
        /// Insert record internally and returns save response.
        /// Dto nesnesini, entity nesnesine dönüştürüp veritabanına
        /// kaydeder. sonuç olarak SimpleResponse nesnesini döner.
        /// </summary>
        /// <param name="dto"></param>
        /// <param name="autoSave"></param>
        /// <returns></returns>
        protected virtual SimpleResponse InsertInternalDontReturnDto(TDto dto, bool autoSave = true)
        {
            if (dto == null)
                return new SimpleResponse<TDto> { ResponseCode = BusinessResponseValues.NullDtoValue };

            var response = new SimpleResponse();
            var entity = MapReverse(dto);

            var validation = entity.Validate();
            if (validation.HasError)
            {
                return new SimpleResponse<TDto>
                {
                    Data = dto,
                    ResponseCode = BusinessResponseValues.ValidationErrorResult,
                    ResponseMessage = validation.AllValidationMessages
                };
            }

            this.Repository.Add(entity);

            if (autoSave)
                response.ResponseCode = this.Repository.SaveChanges();

            return response;
        }

        /// <summary>
        /// Inserts records internally.
        /// </summary>
        /// <param name="dtoList"></param>
        /// <param name="autoSave"></param>
        /// <returns></returns>
        protected virtual SimpleResponse<List<TDto>> InsertRangeInternal(List<TDto> dtoList, bool autoSave = true)
        {
            var response = new SimpleResponse<List<TDto>>();
            if (dtoList == null || dtoList.Count < 1)
                return response;

            var entityList = MapListReverse(dtoList);

            this.Repository.AddRange(entityList);

            if (autoSave)
                response.ResponseCode = this.Repository.SaveChanges();

            var result = MapList(entityList);
            response.Data = result;

            return response;
        }

        /// <summary>
        /// Update record internally.
        /// </summary>
        /// <param name="dto"></param>
        /// <param name="autoSave"></param>
        /// <returns></returns>
        protected virtual SimpleResponse<TDto> UpdateInternal(TDto dto, bool autoSave = true)
        {
            if (dto == null)
                return new SimpleResponse<TDto> { ResponseCode = BusinessResponseValues.NullDtoValue };

            var response = new SimpleResponse<TDto>();
            var entity = MapReverse(dto);

            var validation = entity.Validate();
            if (validation.HasError)
            {
                return new SimpleResponse<TDto>
                {
                    Data = dto,
                    ResponseCode = BusinessResponseValues.ValidationErrorResult,
                    ResponseMessage = validation.AllValidationMessages
                };
            }

            this.Repository.Update(entity);

            if (autoSave)
                response.ResponseCode = this.Repository.SaveChanges();

            var result = Map(entity);
            response.Data = result;

            return response;
        }

        /// <summary>
        /// Updates records internally.
        /// </summary>
        /// <param name="dtoList"></param>
        /// <param name="autoSave"></param>
        /// <returns></returns>
        protected virtual SimpleResponse<List<TDto>> UpdateRangeInternal(List<TDto> dtoList, bool autoSave = true)
        {
            var response = new SimpleResponse<List<TDto>>();

            if (dtoList == null || dtoList.Count < 1)
                return response;

            var entList = MapListReverse(dtoList) ?? new List<TEntity>();

            this.Repository.UpdateRange(entList);

            if (autoSave)
                response.ResponseCode = this.Repository.SaveChanges();

            var result = MapList(entList);
            response.Data = result;

            return response;
        }

        /// <summary>
        /// Delete record internally.
        /// </summary>
        /// <param name="dto"></param>
        /// <param name="autoSave"></param>
        /// <returns></returns>
        protected virtual SimpleResponse<TDto> DeleteInternal(TDto dto, bool autoSave = true)
        {
            if (dto == null)
                return new SimpleResponse<TDto> { ResponseCode = BusinessResponseValues.NullDtoValue };

            var response = new SimpleResponse<TDto>();
            var tmpEnt = MapReverse(dto);

            var deleteProperty = typeof(TEntity).GetProperty(SimpleValues.DeletePropertyName);
            var keys = typeof(TEntity).GetKeysOfType().ToList();//.GetKeyNamesOfType(includeIdentityProperties: true);
            TEntity ent = null;

            if (keys.Count > 0)
            {
                var values = new object[keys.Count];
                int indx = 0;
                keys.ForEach(q => { values[indx++] = tmpEnt.GetPropertyValue(q); });
                //tmpEnt.GetPropertyValues(keys);

                if (values.Length > 0)
                    ent = this.Repository.Find(values);

                if (ent == null)
                {
                    return new SimpleResponse<TDto> { Data = dto, ResponseCode = BusinessResponseValues.NullEntityValue };
                }
            }
            else
                ent = tmpEnt;

            if (deleteProperty != null)
            {
                deleteProperty.SetValue(ent, 1, null);
                this.Repository.Update(ent);
            }
            else
            {
                this.Repository.Delete(ent);
            }

            if (autoSave)
                response.ResponseCode = this.Repository.SaveChanges();

            response.Data = dto;
            return response;
        }

        /// <summary>
        /// Deletes records internally.
        /// </summary>
        /// <param name="dtoList"></param>
        /// <param name="autoSave"></param>
        /// <returns></returns>
        protected virtual SimpleResponse<List<TDto>> DeleteRangeInternal(List<TDto> dtoList, bool autoSave = true)
        {
            var response = new SimpleResponse<List<TDto>>();

            if (dtoList == null || dtoList.Count < 1) return response;

            var deleteList = MapListReverse(dtoList);

            var keys = typeof(TEntity).GetKeysOfType().ToList();//GetKeyNamesOfType(includeIdentityProperties: true);

            if (keys.Count > 0)
            {
                var entList = new List<TEntity>();
                for (int counter = 0; counter < deleteList.Count; counter++)
                {
                    var values = new object[keys.Count];
                    int indx = 0;
                    keys.ForEach(q => { values[indx++] = deleteList[counter].GetPropertyValue(q); });
                    //var values = deleteList[counter].GetPropertyValues(keys);
                    if (values.Length == 0) continue;

                    var entity = this.Repository.Find(values);
                    if (entity == null) continue;

                    entList.Add(entity);
                }

                deleteList.Clear();
                deleteList.AddRange(entList);
            }

            var deleteProperty = typeof(TEntity).GetProperty(SimpleValues.DeletePropertyName);
            var isDeleteOrUpdate = true;
            if (deleteProperty != null)
            {
                for (int counter = 0; counter < deleteList.Count; counter++)
                {
                    var entity = deleteList[counter];
                    deleteProperty.SetValue(entity, 1, null);
                    deleteList[counter] = entity;
                }
                isDeleteOrUpdate = false;
            }

            if (isDeleteOrUpdate)
                this.Repository.DeleteRange(deleteList);
            else
                this.Repository.UpdateRange(deleteList);

            if (autoSave)
                response.ResponseCode = this.Repository.SaveChanges();

            response.Data = dtoList;
            return response;
        }

        /// <summary>
        /// Reads all records as internally.
        /// </summary>
        /// <returns></returns>
        protected SimpleResponse<List<TDto>> ReadAllInternal()
        {
            List<TDto> dtos;
            List<TEntity> entities;

            entities = this.Repository
                .GetAll(asNoTracking: true)
                .ToList() ?? new List<TEntity>();

            dtos = MapList(entities) ?? new List<TDto>();

            var response = new SimpleResponse<List<TDto>> { Data = dtos };

            response.ResponseCode = dtos.Count;
            response.RCode = response.ResponseCode.ToString();

            return response;
        }

        /// <summary>
        /// Return single entity for given predicate.
        /// </summary>
        /// <param name="predicate"></param>
        /// <param name="asNoTracking"></param>
        /// <returns>returns entity instance if predicate condition true.</returns>
        protected TEntity ReadSingle(Expression<Func<TEntity, bool>> predicate, bool asNoTracking = false)
        {
            TEntity entity = this.Repository.Single(predicate, asNoTracking: asNoTracking);

            return entity;
        }

        /// <summary>
        /// Return single dto for given predicate.
        /// </summary>
        /// <param name="predicate"></param>
        /// <returns>returns dto instance if predicate condition true.</returns>
        protected TDto ReadDtoSingle(Expression<Func<TEntity, bool>> predicate)
        {
            var entity = ReadSingle(predicate, asNoTracking: true);
            var dto = Map(entity);
            return dto;
        }

        /// <summary>
        /// Return first entity for given predicate.
        /// </summary>
        /// <param name="predicate"></param>
        /// <param name="asNoTracking"></param>
        /// <returns>returns first entity instance if predicate condition true.</returns>
        protected TEntity ReadFirst(Expression<Func<TEntity, bool>> predicate, bool asNoTracking = false)
        {
            TEntity entity = null;

            entity = this.Repository.FirstOrDefault(predicate, asNoTracking: asNoTracking);

            return entity;
        }

        /// <summary>
        /// Return first dto for given predicate.
        /// </summary>
        /// <param name="predicate"></param>
        /// <returns>returns first dto instance if predicate condition true.</returns>
        protected TDto ReadDtoFirst(Expression<Func<TEntity, bool>> predicate)
        {
            var entity = ReadFirst(predicate, asNoTracking: true);
            var dto = Map(entity);
            return dto;
        }

        /// <summary>
        /// Maps entity to data transfer object.
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        protected TDto Map(TEntity entity)
        {
            var dto = SimpleMapper.Map<TEntity, TDto>(entity);
            return dto;
        }

        /// <summary>
        /// Maps data transfer object to entity.
        /// </summary>
        /// <param name="dto"></param>
        /// <returns></returns>
        protected TEntity MapReverse(TDto dto)
        {
            var entity = SimpleMapper.Map<TDto, TEntity>(dto);
            return entity;
        }

        /// <summary>
        /// Maps entity to data transfer object.
        /// </summary>
        /// <param name="entity"></param>
        /// <param name="dto"></param>
        protected void MapTo(TEntity entity, TDto dto)
        {
            SimpleMapper.MapTo(entity, dto);
        }


        /// <summary>
        /// Maps data transfer object to entity.
        /// </summary>
        /// <param name="entity"></param>
        /// <param name="dto"></param>
        protected void MapToReverse(TDto dto, TEntity entity)
        {
            SimpleMapper.MapTo(dto, entity);
        }

        /// <summary>
        /// Maps entity list to data transfer object list.
        /// </summary>
        /// <param name="entityList"></param>
        /// <returns></returns>
        protected List<TDto> MapList(List<TEntity> entityList)
        {
            var dtos = SimpleMapper.MapList<TEntity, TDto>(entityList);
            return dtos;
        }

        /// <summary>
        /// Maps data transfer object list to entity list.
        /// </summary>
        /// <param name="entityList"></param>
        /// <returns></returns>
        protected List<TEntity> MapListReverse(List<TDto> entityList)
        {
            var entities = SimpleMapper.MapList<TDto, TEntity>(entityList);
            return entities;
        }

        /// <summary>
        /// Saves changes.
        /// </summary>
        /// <returns></returns>
        protected int SaveChanges()
        {
            var result = Repository.SaveChanges();
            return result;
        }

        #region IDisposable Members

        private bool disposed = false;

        /// <summary>
        /// Disposes object.
        /// </summary>
        /// <param name="disposing"></param>
        protected virtual void Dispose(bool disposing)
        {
            if (!disposed)
            {
                if (disposing && repository != null)
                {
                    repository.Dispose();
                }
            }

            disposed = true;
        }

        /// <summary>
        /// Disposes object.
        /// </summary>
        public virtual void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        #endregion IDisposable Members
    }
}