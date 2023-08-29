using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TeamCapacityBalancing.Navigation;

public class PageService
{
    public Dictionary<Type, PageData> Pages { get; }= new ();

    public void RegisterPage<P, VM>(string pageName) 
    {
        Pages.Add(typeof(P), new PageData()
        {
            Name = pageName,
            ViewModelType = typeof(VM),
            Type= typeof(P),
        });

    }

}
