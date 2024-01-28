namespace RuntimeApps.UserConfig {
    public class UserConfigOption {
        public bool ValidateKey { get; set; } = true;
        public IList<string> ValidKeys { get; set; } = [];
        public IList<string> ValidDefaultKeus { get; set; } = [];
        public IList<string> ValidReadonlyKeys { get; set; } = [];
        public bool UseCache { get; set; } = false;
        public string CachePrefix { get; set; } = "UserConfigs";
    }
}
