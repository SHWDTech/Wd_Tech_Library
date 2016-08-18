using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using SHWDTech.RepositoryModel;

namespace SHWDTech.Repository
{
    /// <summary>
    /// 数据仓库基类接口
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public interface IRepository<T> where T : class, IRepositoryModelBase, new()
    {
        /// <summary>
        /// 是否执行数据检查
        /// </summary>
        bool ExecuteCheck { get; set; }

        /// <summary>
        /// 默认连接字符串或连接字符串名称
        /// </summary>
        string DefaultConnectionNameOrString { get; set; }

        /// <summary>
        /// 初始化数据仓库实体集
        /// </summary>
        void InitEntitySet();

        /// <summary>
        /// 获取IQueryable实体数据集合
        /// </summary>
        /// <returns></returns>
        IQueryable<T> GetModels(Expression<Func<T, bool>> exp);

        /// <summary>
        /// 获取可枚举类型实体数据集合
        /// </summary>
        /// <param name="exp"></param>
        /// <returns></returns>
        ICollection<T> GetModelsSet(Expression<Func<T, bool>> exp);

        /// <summary>
        /// 获取集合中唯一的数据，如果集合为空则返回默认数据
        /// </summary>
        /// <param name="exp"></param>
        /// <returns></returns>
        T SingleOrDefault(Expression<Func<T, bool>> exp);

        /// <summary>
        /// 获取集合中的第一个数据，如果集合为空则返回默认数据
        /// </summary>
        /// <param name="exp"></param>
        /// <returns></returns>
        T FirstOrDefault(Expression<Func<T, bool>> exp);

        /// <summary>
        /// 获取集合中数据的总数
        /// </summary>
        /// <param name="exp"></param>
        /// <returns></returns>
        int Count(Expression<Func<T, bool>> exp);

        /// <summary>
        /// 从Json字符串中解析数据模型
        /// </summary>
        /// <param name="modelJsonString"></param>
        /// <returns></returns>
        T ParseModel(string modelJsonString);

        /// <summary>
        /// 新增或更新单条数据
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        int AddOrUpdate(T model);

        /// <summary>
        /// 新增或更新数据集合
        /// </summary>
        /// <param name="models"></param>
        /// <returns></returns>
        int AddOrUpdate(ICollection<T> models);

        /// <summary>
        /// 部分更新单条数据
        /// </summary>
        /// <param name="model"></param>
        /// <param name="propertyNames"></param>
        /// <returns></returns>
        int PartialUpdate(T model, ICollection<string> propertyNames);

        /// <summary>
        /// 部分更新数据集合
        /// </summary>
        /// <param name="models"></param>
        /// <param name="propertyNames"></param>
        /// <returns></returns>
        int PartialUpdate(ICollection<T> models, ICollection<string> propertyNames);

        /// <summary>
        /// 快速批量插入数据
        /// </summary>
        /// <param name="models"></param>
        void BulkInsert(ICollection<T> models);

        /// <summary>
        /// 删除单条数据
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        int Delete(T model);

        /// <summary>
        /// 批量删除数据
        /// </summary>
        /// <param name="models"></param>
        /// <returns></returns>
        int Delete(ICollection<T> models);

        /// <summary>
        /// 标记单条数据为已删除
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        int MarkDeleted(T model);

        /// <summary>
        /// 批量标记数据为已删除
        /// </summary>
        /// <param name="models"></param>
        /// <returns></returns>
        int MarkDeleted(ICollection<T> models);

        /// <summary>
        /// 标记数据为已启用
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        int Enable(T model);

        /// <summary>
        /// 批量标记数据为已启用
        /// </summary>
        /// <param name="models"></param>
        /// <returns></returns>
        int Enable(ICollection<T> models);

        /// <summary>
        /// 标记数据为未启用
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        int Disable(T model);

        /// <summary>
        /// 批量标记数据为未启用
        /// </summary>
        /// <param name="models"></param>
        /// <returns></returns>
        int Disable(ICollection<T> models);

        /// <summary>
        /// 反转数据启用状态
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        int ReverseEnable(T model);

        /// <summary>
        /// 批量反转数据启用状态
        /// </summary>
        /// <param name="models"></param>
        /// <returns></returns>
        int ReverseEnable(ICollection<T> models);
    }
}
