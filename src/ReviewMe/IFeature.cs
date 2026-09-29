namespace ReviewMe;

public interface IFeature
{
    bool Accepts(string[] args);
    Task<int> RunAsync(string[] args);
}
