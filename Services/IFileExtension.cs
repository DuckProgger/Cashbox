using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cashbox.Services
{
    public interface IFileExtension
    {
        public string Description { get; }
        public string Extension { get; }
    }
}
