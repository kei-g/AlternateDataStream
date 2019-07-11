namespace AlternateDataStream
{
    public class UNCPath
    {
        public static implicit operator string(UNCPath path) => path.content.Length < 256 ? path.content : @"\\?\" + path.content;

        private readonly string content;

        public UNCPath(string content) => this.content = content;
    }
}
