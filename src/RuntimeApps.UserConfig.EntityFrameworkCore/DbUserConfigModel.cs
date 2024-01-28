namespace RuntimeApps.UserConfig.EntityFrameworkCore {
    public class DbUserConfigModel: UserConfigModel<string> {
        public int Id { get; set; }
        public DateTime CreateDate { get; set; } = DateTime.Now;
        public DateTime? LastUpdateDate { get; set; }
    }
}
