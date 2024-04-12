namespace TPLDataFlowDemo
{
    public interface INamesDb
    {
        Task SaveName(string name);
    }

    public class NamesDb : INamesDb
    {
        public async Task SaveName(string name)
        {
            await Task.Delay(TimeSpan.FromSeconds(1));
        }
    }

}
