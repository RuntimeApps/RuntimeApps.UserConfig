namespace RuntimeApps.UserConfig {
    public class UserConfigOption {
        public virtual bool ValidateKey { get; set; } = true;
        public virtual IList<string> ValidKeys { get; set; } = [];
        public virtual IList<string> ValidDefaultKeus { get; set; } = [];
        public virtual IList<string> ValidReadonlyKeys { get; set; } = [];
        public virtual bool UseCache { get; set; } = false;
        public virtual string CachePrefix { get; set; } = "UserConfigs";
    }
}
