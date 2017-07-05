using System;
using System.Collections.Generic;
using System.Text;

namespace Guarddog.Modules
{
    public interface IModule
    {
        void Load();
        void Unload();
    }
}
