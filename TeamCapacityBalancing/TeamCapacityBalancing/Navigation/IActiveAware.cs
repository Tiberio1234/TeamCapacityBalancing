using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TeamCapacityBalancing.Navigation;

public interface IActiveAware
{
    public void OnActivated();
    public void OnDeactivated();
}
