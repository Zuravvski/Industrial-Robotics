namespace Driver
{
    public interface IFileWriter
    {
        void Write();
        void Write<T>(T file);
    }
}
