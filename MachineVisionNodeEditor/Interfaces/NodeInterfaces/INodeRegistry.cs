using MachineVisionNodeEditor.Models.NodeModels;
using MachineVisionNodeEditor.ViewModels.NodeViewModels;
using System;
using System.Collections.Generic;
using System.Text;

namespace MachineVisionNodeEditor.Interfaces.NodeInterfaces
{
    public interface INodeRegistry
    {
        void Register(ISelectableViewModel vm);

        void Unregister(ISelectableViewModel vm);

        ISelectableViewModel? GetViewModel(ISelectable model);

        TViewModel? GetViewModel<TViewModel>(ISelectable model)
            where TViewModel : class, ISelectableViewModel;

        IEnumerable<ISelectableViewModel> GetAll();

        void Clear();
    }
}
