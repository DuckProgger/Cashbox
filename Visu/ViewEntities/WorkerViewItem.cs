using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cashbox.Visu.ViewEntities
{
    public class WorkerViewItem
    {
        /// <summary>
        /// Отмечен в смене.
        /// </summary>
        public bool Checked { get; set; }

        /// <summary>
        /// Имя работника.
        /// </summary>
        public string Name { get; set; }
    }
}