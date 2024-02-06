namespace RuntimeApps.UserConfig.Exceptions {
    public class InvalidConfigKeyException: KeyNotFoundException {
        public string? Key { get; private set; }
        public InvalidConfigKeyException(string? key) : base($"Key {key} is not valid") {
            Key = key;
        }
    }
}
