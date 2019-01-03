using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PZ3_NetworkService
{
    public interface IClosing
    {
        /// <summary>
        /// Executes when window is closing
        /// </summary>
        /// <returns>Whether the windows should be closed by the caller</returns>
        bool OnClosing();
    }
}
