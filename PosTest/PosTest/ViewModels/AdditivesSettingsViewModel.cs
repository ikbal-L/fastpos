using Caliburn.Micro;
using ServiceInterface.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PosTest.ViewModels
{
    public class AdditivesSettingsViewModel: Screen
    {

        public BindableCollection<Additive> Additives { get; set; }

        public AdditivesSettingsViewModel( )
        {
            Additives = new BindableCollection<Additive>();
            Additives.Add(new Additive());
            Additives.Add(new Additive());
            Additives.Add(new Additive());
            Additives.Add(new Additive());
            Additives.Add(new Additive());
            Additives.Add(new Additive());
            Additives.Add(new Additive());
            Additives.Add(new Additive());
            Additives.Add(new Additive());
            Additives.Add(new Additive());
            Additives.Add(new Additive());
            Additives.Add(new Additive());
            Additives.Add(new Additive());
            Additives.Add(new Additive());
            Additives.Add(new Additive());
            Additives.Add(new Additive());
            Additives.Add(new Additive());
            Additives.Add(new Additive());
            Additives.Add(new Additive());
            Additives.Add(new Additive());
            Additives.Add(new Additive());
            Additives.Add(new Additive());
            Additives.Add(new Additive());
            Additives.Add(new Additive());
            Additives.Add(new Additive());
            Additives.Add(new Additive());
            Additives.Add(new Additive());
            Additives.Add(new Additive());
            Additives.Add(new Additive());
            Additives.Add(new Additive());
            Additives.Add(new Additive());
        }
    }
}
