using MachineVisionNodeEditor.Interfaces;
using MachineVisionNodeEditor.Models.NodeModels;
using MachineVisionNodeEditor.ViewModels;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace MachineVisionNodeEditor.Services
{
    public class SelectionService : BaseViewModel
    {
        public ObservableCollection<ISelectable> SelectedItems { get; } = new();

        public bool HasSelection => SelectedItems.Count > 0;
        public bool HasSingleSelection { get { return SelectedItems.Count == 1; }  }
        public bool HasMultiSelection => SelectedItems.Count > 1;

        public NodeModel? SelectedNode
        {
            get
            {
                return SelectedItems
                    .OfType<NodeModel>()
                    .FirstOrDefault();
            }
        }

        public ConnectionModel? SelectedConnection
        {
            get
            {
                return SelectedItems
                    .OfType<ConnectionModel>()
                    .FirstOrDefault();
            }
        }

        public ISelectable? SelectedItem
        {
            get
            {
                return SelectedItems
                    .OfType<ISelectable>()
                    .FirstOrDefault();
            }
        }

        public void Clear()
        {
            foreach (var item in SelectedItems)
                item.IsSelected = false;

            SelectedItems.Clear();
            UpdateProperties();
        }

        public void Select(ISelectable item, bool ctrlPressed)
        {
            if (!ctrlPressed)
                Clear();

            if (!SelectedItems.Contains(item))
            {
                item.IsSelected = true;
                SelectedItems.Add(item);
            }
            UpdateProperties();
        }

        public void Toggle(ISelectable item)
        {
            if (SelectedItems.Contains(item))
            {
                item.IsSelected = false;
                SelectedItems.Remove(item);
            }
            else
            {
                item.IsSelected = true;
                SelectedItems.Add(item);
            }
            UpdateProperties();
        }

        /// <summary>
        /// Chọn tất cả item có position nằm trong rect (toạ độ canvas).
        /// items: danh sách (selectable, vị trí tâm trên canvas).
        /// </summary>
        public void SelectInRect(IEnumerable<(ISelectable item, Point position)> items, Rect rect)
        {
            Clear();
            foreach (var (item, pos) in items)
            {
                if (rect.Contains(pos))
                {
                    item.IsSelected = true;
                    if (!SelectedItems.Contains(item))
                        SelectedItems.Add(item);
                }
            }
            UpdateProperties();
        }

        private void UpdateProperties()
        {
            OnPropertyChanged(nameof(HasSelection));

            OnPropertyChanged(nameof(HasSingleSelection));

            OnPropertyChanged(nameof(HasMultiSelection));

            OnPropertyChanged(nameof(SelectedNode));

            OnPropertyChanged(nameof(SelectedConnection));

            OnPropertyChanged(nameof(SelectedItem));
        }

    }
}
