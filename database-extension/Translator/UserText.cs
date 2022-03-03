namespace DatabaseExtension.Translator
{
    public class UserText
    {
        public string Text { get; init; } = string.Empty;

        public IEnumerable<string> InjectionWords => Text
            .Split(" ")
            .Where(s => s.Contains(SplitChar));

        private const char SplitChar = '@';
    }
}
