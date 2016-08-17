namespace SHWDTech.RepositoryModel
{
    public interface IRepositoryModelBase
    {
        /// <summary>
        /// 是否是新增数据（未持久化）
        /// </summary>
        bool IsNew { get; }

        /// <summary>
        /// 是否标记为删除
        /// </summary>
        bool IsDeleted { get; set; }

        /// <summary>
        /// 是否为启用状态
        /// </summary>
        bool IsEnabled { get; set; }
    }
}
