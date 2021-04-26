using Caliburn.Micro;
using ServiceInterface.Model;
using ServiceLib.Service.StateManager;

namespace FastPosFrontend.ViewModels
{
    public class AdditiveDetailViewModel:Dialog
    {
        

        public AdditiveDetailViewModel(Additive source,AdditivesSettingsViewModel host)
        {
            _source = source;
            _host = host;
            Additive = _source.Clone();
        }

        private readonly Additive _source;
        private readonly AdditivesSettingsViewModel _host;
        private Additive _additive;

        public Additive Additive
        {
            get => _additive;
            set => Set(ref _additive, value);
        }

        public void SaveAdditive()
        {
            this._source.Description = Additive.Description;
            this._source.Background = Additive.Background;
            if (StateManager.Save(_source))
            {

            }
            else
            {
                if (_source.Id == null)
                {
                    _source.Description = null;
                    _source.Background = null;
                }
            }

            Additive = null;
            _host?.Close();


        }

        public void Cancel()
        {
            _host.Close();
            
        }
    }
}