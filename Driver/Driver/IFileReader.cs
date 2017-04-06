namespace Driver
{
    public interface IFileReader<out T>
    {
        T Read(string handler);
    }
}
