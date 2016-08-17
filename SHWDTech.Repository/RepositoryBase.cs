using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Linq.Expressions;
using System.Transactions;
using EntityFramework.BulkInsert.Extensions;
using Newtonsoft.Json;
using SHWDTech.RepositoryModel;

namespace SHWDTech.Repository
{
    public class RepositoryBase<T> : IRepositoryBase<T>, IDisposable where T : class, IRepositoryModelBase, new()
    {
        private readonly DbSet<T> _dbSet;

        /// <summary>
        /// 数据库上下文
        /// </summary>
        public DbContext DbContext { get; }

        /// <summary>
        /// 进行操作的数据实体
        /// </summary>
        protected IQueryable<T> EntitySet { get; set; }

        /// <summary>
        /// 数据实体检查条件
        /// </summary>
        protected Expression<Func<T, bool>> EntityFilter { get; set; }

        /// <summary>
        /// 创建一个新的数据仓库泛型基类对象
        /// </summary>
        protected RepositoryBase()
        {
            
        }

        /// <summary>
        /// 创建一个新的数据仓库泛型基类对象
        /// </summary>
        protected RepositoryBase(string connString) : this()
        {
            DbContext = new DbContext(connString);
            _dbSet = DbContext.Set<T>();
        }

        /// <summary>
        /// 创建一个新的数据仓库泛型基类对象
        /// </summary>
        protected RepositoryBase(DbContext dbContext) : this()
        {
            DbContext = dbContext;
            _dbSet = DbContext.Set<T>();
        }

        /// <summary>
        /// 初始化数据实体对象
        /// </summary>
        public virtual void InitEntitySet()
        {
            EntitySet = EntityFilter == null
                ? _dbSet
                : _dbSet.Where(EntityFilter);
        }

        public virtual IQueryable<T> GetAllModels()
            => EntitySet;

        public virtual IEnumerable<T> GetAllModelsSet()
            => EntitySet.AsEnumerable();

        public virtual IQueryable<T> GetModels(Expression<Func<T, bool>> exp)
            => EntitySet.Where(exp);

        public virtual IEnumerable<T> GetModelsSet(Expression<Func<T, bool>> exp)
            => EntitySet.Where(exp).AsEnumerable();

        public virtual T GetModel(Expression<Func<T, bool>> exp)
            => EntitySet.SingleOrDefault();

        public virtual int Count(Expression<Func<T, bool>> exp)
            => exp == null ? EntitySet.Count() : EntitySet.Where(exp).Count();

        public static T Create() 
            => new T();

        public virtual T ParseModel(string modelJsonString)
            => JsonConvert.DeserializeObject<T>(modelJsonString);

        public virtual int AddOrUpdate(T model)
        {
            AddOrUpdateNoCommit(model);
            return SaveChanges();
        }

        public virtual int AddOrUpdate(IEnumerable<T> models)
        {
            foreach (var model in models)
            {
                AddOrUpdateNoCommit(model);
            }

            return SaveChanges();
        }

        public virtual int PartialUpdate(T model, IEnumerable<string> propertyNames)
        {
            PartialUpdateNoCommit(model, propertyNames);

            return SaveChanges();
        }

        public virtual int PartialUpdate(IEnumerable<T> models, IEnumerable<string> propertyNames)
        {
            var properties = propertyNames as string[] ?? propertyNames.ToArray();
            foreach (var model in models)
            {
                PartialUpdateNoCommit(model, properties);
            }

            return SaveChanges();
        }

        public virtual void BulkInsert(IEnumerable<T> models)
        {
            using (var scope = new TransactionScope())
            {
                DbContext.BulkInsert(models);
                scope.Complete();
            }
        }

        public virtual int Delete(T model)
        {
            CheckModel(model);

            DeleteNoCommit(model);

            return SaveChanges();
        }

        public virtual int Delete(IEnumerable<T> models)
        {
            CheckModel(models);

            foreach (var model in models)
            {
                DeleteNoCommit(model);
            }

            return SaveChanges();
        }

        public virtual int MarkDeleted(T model)
        {
            MarkDeletedNoCommit(model);

            return SaveChanges();
        }

        public virtual int MarkDeleted(IEnumerable<T> models)
        {
            foreach (var model in models)
            {
                MarkDeletedNoCommit(model);
            }

            return SaveChanges();
        }

        public virtual int Enable(T model)
        {
            SetModelEnableStatus(model, true);

            return SaveChanges();
        }

        public virtual int Enable(IEnumerable<T> models)
        {
            foreach (var model in models)
            {
                SetModelEnableStatus(model, true);
            }

            return SaveChanges();
        }

        public virtual int Disable(T model)
        {
            SetModelEnableStatus(model, false);

            return SaveChanges();
        }

        public virtual int Disable(IEnumerable<T> models)
        {
            foreach (var model in models)
            {
                SetModelEnableStatus(model, false);
            }

            return SaveChanges();
        }

        public virtual int ReverseEnable(T model)
        {
            SetModelEnableStatus(model, !model.IsEnabled);

            return SaveChanges();
        }

        public virtual int ReverseEnable(IEnumerable<T> models)
        {
            foreach (var model in models)
            {
                SetModelEnableStatus(model, !model.IsEnabled);
            }

            return SaveChanges();
        }

        /// <summary>
        /// 判断数据类型是否是基础类型
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        private static bool IsPrimitive(Type type)
            => type.IsPrimitive
                || type == typeof(decimal)
                || type == typeof(string)
                || type == typeof(DateTime)
                || type == typeof(Guid)
                || (type.IsGenericType
                        && type.GetGenericTypeDefinition() == typeof(Nullable<>)
                        && type.GetGenericArguments().Any(t => t.IsValueType && IsPrimitive(t)));

        /// <summary>
        /// 检查模型是否符合要求
        /// </summary>
        /// <param name="models"></param>
        private void CheckModel(object models)
        {
            if (EntityFilter == null) return;

            if (models == null) throw new ArgumentNullException(nameof(models));

            var checkList = new List<T>();
            var item = models as T;
            if (item != null) checkList.Add(item);

            var items = models as IEnumerable<T>;
            if (items != null) checkList.AddRange(items);

            if (!checkList.Any(EntityFilter.Compile())) throw new ArgumentException("参数不符合要求");
        }

        /// <summary>
        /// 新增或更新数据但不提交
        /// </summary>
        /// <param name="model"></param>
        private void AddOrUpdateNoCommit(T model)
        {
            CheckModel(model);

            if (model.IsNew)
            {
                AddModel(model);
            }
            else
            {
                UpdateModel(model);
            }
        }

        private void PartialUpdateNoCommit(T model, IEnumerable<string> propertyNames)
        {
            _dbSet.Attach(model);
            var modelType = model.GetType();
            foreach (var propertyName in propertyNames)
            {
                if (!IsPrimitive(modelType.GetProperty(propertyName).PropertyType)) continue;

                DbContext.Entry(model).Property(propertyName).IsModified = true;
            }
        }

        /// <summary>
        /// 保存更新
        /// </summary>
        /// <returns></returns>
        private int SaveChanges()
        {
            return DbContext.SaveChanges();
        }

        /// <summary>
        /// 新增模型数据
        /// </summary>
        /// <param name="model"></param>
        private void AddModel(T model)
        {
            _dbSet.Add(model);
        }

        /// <summary>
        /// 更新模型数据
        /// </summary>
        /// <param name="model"></param>
        private void UpdateModel(T model)
        {
            _dbSet.Attach(model);
            DbContext.Entry(model).State = EntityState.Modified;
        }

        /// <summary>
        /// 删除模型数据
        /// </summary>
        /// <param name="model"></param>
        private void DeleteNoCommit(T model)
        {
            _dbSet.Remove(model);
        }

        /// <summary>
        /// 标记数据已经删除
        /// </summary>
        /// <param name="model"></param>
        private void MarkDeletedNoCommit(T model)
        {
            model.IsDeleted = true;
            AddOrUpdateNoCommit(model);
        }

        /// <summary>
        /// 设置模型启用状态
        /// </summary>
        /// <param name="model"></param>
        /// <param name="isEnabled"></param>
        private void SetModelEnableStatus(T model, bool isEnabled)
        {
            model.IsEnabled = isEnabled;
            AddOrUpdateNoCommit(model);
        }

        public void Dispose()
        {
            DbContext.Dispose();
        }
    }
}
