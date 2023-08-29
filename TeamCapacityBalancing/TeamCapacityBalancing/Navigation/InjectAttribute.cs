using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TeamCapacityBalancing.Navigation;

[AttributeUsage(AttributeTargets.Constructor,Inherited =false, AllowMultiple =false)]
public sealed class InjectAttribute : Attribute
{
}
