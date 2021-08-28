using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cashbox.Model
{
    class Permissions
    {
    }

    public Permissions GetAccesses(Accesses access)
    {
        switch (access)
        {
            case Accesses.Usual:
                return new Permissions()
                {

                }
            case Accesses.Administrator:
                break;
            default:
                break;
        }

    }

    public enum Accesses : int
    {
        Administrator = 0,
        Usual = 1,
    }
}
