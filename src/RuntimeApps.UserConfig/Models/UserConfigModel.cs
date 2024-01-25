namespace RuntimeApps.UserConfig.Models {
    public class UserConfigModel<TValue> {
        public string Key { get; set; } = default!;
        public string? UserId { get; set; }
        public TValue? Value { get; set; }
    }
}
