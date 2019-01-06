namespace ImageStoreAndAnalyze.Models
{
    public interface IUser
    {
        string Id { get; set; }

        bool IsFamilyAdmin { get; set; }
    }
}