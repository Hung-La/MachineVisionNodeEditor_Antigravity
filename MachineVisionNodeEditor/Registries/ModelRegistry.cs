using MachineVisionNodeEditor.Interfaces;
using MachineVisionNodeEditor.Interfaces.NodeInterfaces;
using MachineVisionNodeEditor.Models.NodeModels;
using MachineVisionNodeEditor.ViewModels.NodeViewModels;
using System;
using System.Collections.Generic;
using System.Text;

namespace MachineVisionNodeEditor.Registries
{
    public class ModelRegistry : INodeRegistry
    {
        private readonly Dictionary<ISelectable, ISelectableViewModel> _lookup = new();

        public void Register(ISelectableViewModel vm)
        {
            if (vm == null)
                throw new ArgumentNullException(nameof(vm));

            _lookup[vm.Model] = vm;
        }

        public void Unregister(ISelectableViewModel vm)
        {
            if (vm == null)
                return;

            _lookup.Remove(vm.Model);
        }

        public ISelectableViewModel? GetViewModel(ISelectable model)
        {
            if (model == null)
                return null;

            _lookup.TryGetValue(model, out var vm);

            return vm;
        }

        public TViewModel? GetViewModel<TViewModel>(ISelectable model) where TViewModel : class, ISelectableViewModel
        {
            return GetViewModel(model) as TViewModel;
        }

        public IEnumerable<ISelectableViewModel> GetAll()
        {
            return _lookup.Values;
        }

        public void Clear()
        {
            _lookup.Clear();
        }

    }
}
