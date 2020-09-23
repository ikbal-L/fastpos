using Caliburn.Micro;
using ServiceInterface.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PosTest.ViewModels
{
    class AdditivesOfProductViewModel: Screen
    {
        public AdditivesOfProductViewModel()
        {
            Additives = new BindableCollection<Additive>();
            Additives.Add(new Additive{Id=1 ,Description="Addtives1 " });
            Additives.Add(new Additive{Id=2 ,Description="Addtives2 "});
            Additives.Add(new Additive{Id=3 ,Description="Addtives3 "});
            Additives.Add(new Additive{Id=4 ,Description="Addtives4 "});
            Additives.Add(new Additive{Id=5 ,Description="Addtives5 "});
            Additives.Add(new Additive{Id=6 ,Description="Addtives6 "});
            Additives.Add(new Additive{Id=7 ,Description="Addtives7 "});
            Additives.Add(new Additive{Id=8 ,Description="Addtives8 "});
            Additives.Add(new Additive{Id=9 ,Description="Addtives9 "});
            Additives.Add(new Additive{Id=10,Description="Addtives10"});
            Additives.Add(new Additive{Id=11,Description="Addtives11"});
            Additives.Add(new Additive{Id=12,Description="Addtives12"});
            Additives.Add(new Additive{Id=13,Description="Addtives13"});
            Additives.Add(new Additive{Id=14,Description="Addtives14"});
            Additives.Add(new Additive{Id=15,Description="Addtives15"});
            Additives.Add(new Additive{Id=16,Description="Addtives16"});
            Additives.Add(new Additive{Id=17,Description="Addtives17"});
            Additives.Add(new Additive{Id=18,Description="Addtives18"});
            Additives.Add(new Additive{Id=19,Description="Addtives19"});
            Additives.Add(new Additive{Id=20,Description="Addtives20"});
            Additives.Add(new Additive{Id=21,Description="Addtives21"});
            Additives.Add(new Additive{Id=22,Description="Addtives22"});
            Additives.Add(new Additive{Id=23,Description="Addtives23"});
            Additives.Add(new Additive{Id=24,Description="Addtives24"});
            Additives.Add(new Additive{Id=25,Description="Addtives25"});
            Additives.Add(new Additive{Id=26,Description="Addtives26"});
            Additives.Add(new Additive{Id=27,Description="Addtives27"});
            Additives.Add(new Additive{Id=28,Description="Addtives28"});
            Additives.Add(new Additive{Id= 29, Description="Addtives29"});
            Additives.Add(new Additive());
            
        }

        public BindableCollection<Additive> Additives { get; }
    }
}
