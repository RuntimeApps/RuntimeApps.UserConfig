namespace RuntimeApps.UserConfig.EntityFrameworkCore {
    public class DbUserConfigModel: UserConfigModel<string> {
        public virtual int Id { get; set; }
        public virtual DateTime CreateDate { get; set; } = DateTime.Now;
        public virtual DateTime? LastUpdateDate { get; set; }
    }
}
