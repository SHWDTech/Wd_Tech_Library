namespace SHWDTech.RepositoryModel
{
    public interface IRepositoryModelBase
    {
        bool IsNew { get; }

        bool IsDeleted { get; set; }

        bool IsEnabled { get; set; }
    }
}
