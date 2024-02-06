namespace RuntimeApps.UserConfig.Exceptions {
    public class InvalidValueModelException: FormatException {
        public object? Value { get; private set; }

        public InvalidValueModelException(string? key, object? value) : base($"Config value of {key} is not valid") {
            Value = value;
        }
    }
}
