namespace Application.Common.Exceptions
{
    public class NotFoundException : Exception
    {
        public NotFoundException(string name, object key)
            : base($"Entity \"{name}\" ({key}) was not found.")
        {

        }

        public NotFoundException(List<string> messages)
            : base(string.Join(Environment.NewLine, messages))
        {

        }
    }
}
