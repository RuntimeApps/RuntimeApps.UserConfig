namespace RuntimeApps.UserConfig {
    public class UserConfigModel<TValue> {
        public string Key { get; set; } = default!;
        public string? UserId { get; set; }
        public TValue? Value { get; set; }
    }
}
