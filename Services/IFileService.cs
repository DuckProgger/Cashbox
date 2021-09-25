using System.Collections.Generic;

namespace Cashbox.Services
{
    public interface IFileService<T>
    {
        //public List<T> ReadFile(string filename);
        public void SaveFile(string filename, List<T> data);
    }
}
