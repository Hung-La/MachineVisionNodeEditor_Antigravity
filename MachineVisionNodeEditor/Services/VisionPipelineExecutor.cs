using MachineVisionNodeEditor.Extensions;
using MachineVisionNodeEditor.Models.NodeModels;
using MachineVisionNodeEditor.Registries;
using MachineVisionNodeEditor.ViewModels.NodeViewModels;
using OpenCvSharp;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;

namespace MachineVisionNodeEditor.Services
{
    /// <summary>
    /// Thực thi toàn bộ pipeline xử lý ảnh theo thứ tự topo (topological sort).
    /// Duyệt từ các node nguồn (không có input) → các node xử lý → node cuối.
    /// </summary>
    public class VisionPipelineExecutor
    {
        private readonly IReadOnlyList<NodeControl_NodeViewModel> _nodes;
        private readonly IReadOnlyList<Node_ConnectionViewModel> _connections;
        private readonly ModelRegistry _registry;

        public VisionPipelineExecutor(
            IReadOnlyList<NodeControl_NodeViewModel> nodes,
            IReadOnlyList<Node_ConnectionViewModel> connections,
            ModelRegistry registry)
        {
            _nodes = nodes;
            _connections = connections;
            _registry = registry;
        }

        /// <summary>
        /// Chạy toàn bộ pipeline. Trả về PipelineResult chứa thông tin kết quả.
        /// </summary>
        public PipelineResult Execute()
        {
            var result = new PipelineResult();
            var stopwatch = Stopwatch.StartNew();

            try
            {
                // Bước 1: Sắp xếp topological
                var sortedNodes = TopologicalSort();
                result.TotalNodes = sortedNodes.Count;

                // Bước 2: Thực thi từng node theo thứ tự
                foreach (var nodeVm in sortedNodes)
                {
                    try
                    {
                        ExecuteNode(nodeVm);
                        result.ProcessedNodes++;
                    }
                    catch (Exception ex)
                    {
                        result.Errors.Add($"Lỗi tại node \"{nodeVm.NodeModel.Title}\": {ex.Message}");
                    }
                }

                result.Success = result.Errors.Count == 0;
            }
            catch (Exception ex)
            {
                result.Success = false;
                result.Errors.Add($"Lỗi pipeline: {ex.Message}");
            }

            stopwatch.Stop();
            result.ElapsedMs = stopwatch.ElapsedMilliseconds;
            return result;
        }

        /// <summary>
        /// Thực thi một node cụ thể: lấy ảnh từ node trước → xử lý → ghi kết quả.
        /// </summary>
        private void ExecuteNode(NodeControl_NodeViewModel nodeVm)
        {
            switch (nodeVm)
            {
                case ImageImport_NodeViewModel imageImportVm:
                    ExecuteImageImport(imageImportVm);
                    break;

                case ConvertColor_NodeViewModel convertColorVm:
                    ExecuteConvertColor(convertColorVm);
                    break;

                // Test node: không làm gì, chỉ pass-through
                case Node_NodeViewModel testVm:
                    ExecutePassThrough(testVm);
                    break;

                default:
                    // Node không xác định — skip
                    break;
            }
        }

        /// <summary>
        /// ImageImport: Load ảnh từ file path đã cấu hình.
        /// </summary>
        private void ExecuteImageImport(ImageImport_NodeViewModel vm)
        {
            var prop = vm.NodePropertyModel;

            // Nếu chưa có ảnh và có file path → load
            if (prop.OutputImage == null && !string.IsNullOrWhiteSpace(prop.FilePath))
            {
                vm.OperationModel.Execute(prop);
            }
            // Nếu đã có OutputImage rồi (người dùng đã Browse trước đó) → giữ nguyên
        }

        /// <summary>
        /// ConvertColor: Lấy ảnh từ node trước qua connection → chuyển đổi màu.
        /// </summary>
        private void ExecuteConvertColor(ConvertColor_NodeViewModel vm)
        {
            var prop = vm.NodePropertyModel;

            // Lấy ảnh từ node trước (qua input port)
            if (vm.NodeModel.InputPorts.Count > 0)
            {
                var inputPort = vm.NodeModel.InputPorts[0].PortModel;
                foreach (var connection in inputPort.Connections)
                {
                    var sourceImage = ImageHelper.GetImageFromPreviousNode(connection);
                    if (sourceImage != null)
                    {
                        prop.InputImage = sourceImage;
                        break; // Chỉ lấy ảnh từ connection đầu tiên
                    }
                }
            }

            // Thực thi chuyển đổi màu
            if (prop.InputImage != null && prop.SelectedCode != null)
            {
                vm.OperationModel.Execute(prop);
            }
        }

        /// <summary>
        /// Pass-through: Chuyển ảnh từ input sang output mà không xử lý.
        /// </summary>
        private void ExecutePassThrough(Node_NodeViewModel vm)
        {
            if (vm.NodeModel.InputPorts.Count > 0)
            {
                var inputPort = vm.NodeModel.InputPorts[0].PortModel;
                foreach (var connection in inputPort.Connections)
                {
                    var sourceImage = ImageHelper.GetImageFromPreviousNode(connection);
                    if (sourceImage != null)
                    {
                        vm.NodePropertyModel.InputImage = sourceImage;
                        vm.NodePropertyModel.OutputImage = sourceImage;
                        break;
                    }
                }
            }
        }

        /// <summary>
        /// Topological Sort (Kahn's Algorithm) — sắp xếp node theo thứ tự phụ thuộc.
        /// Node nguồn (không có input connection) được xử lý trước.
        /// </summary>
        private List<NodeControl_NodeViewModel> TopologicalSort()
        {
            // Xây dựng adjacency list và in-degree map
            var inDegree = new Dictionary<NodeControl_NodeViewModel, int>();
            var adjacency = new Dictionary<NodeControl_NodeViewModel, List<NodeControl_NodeViewModel>>();

            foreach (var node in _nodes)
            {
                inDegree[node] = 0;
                adjacency[node] = new List<NodeControl_NodeViewModel>();
            }

            // Duyệt qua tất cả connections để xây dựng graph
            foreach (var conn in _connections)
            {
                var fromPort = conn.ConnectionModel.FromPort;
                var toPort = conn.ConnectionModel.ToPort;

                if (fromPort?.Owner == null || toPort?.Owner == null)
                    continue;

                var fromVm = FindNodeViewModel(fromPort.Owner);
                var toVm = FindNodeViewModel(toPort.Owner);

                if (fromVm != null && toVm != null)
                {
                    adjacency[fromVm].Add(toVm);
                    inDegree[toVm]++;
                }
            }

            // Kahn's Algorithm
            var queue = new Queue<NodeControl_NodeViewModel>();

            foreach (var kvp in inDegree)
            {
                if (kvp.Value == 0)
                    queue.Enqueue(kvp.Key);
            }

            var sorted = new List<NodeControl_NodeViewModel>();

            while (queue.Count > 0)
            {
                var current = queue.Dequeue();
                sorted.Add(current);

                foreach (var neighbor in adjacency[current])
                {
                    inDegree[neighbor]--;
                    if (inDegree[neighbor] == 0)
                        queue.Enqueue(neighbor);
                }
            }

            // Nếu sorted.Count != _nodes.Count → có cycle (vòng lặp)
            if (sorted.Count != _nodes.Count)
            {
                throw new InvalidOperationException(
                    "Pipeline chứa vòng lặp (cycle)! Vui lòng kiểm tra lại các kết nối.");
            }

            return sorted;
        }

        /// <summary>
        /// Tìm NodeControl_NodeViewModel tương ứng với NodeModel.
        /// </summary>
        private NodeControl_NodeViewModel? FindNodeViewModel(NodeModel nodeModel)
        {
            return _nodes.FirstOrDefault(n => n.NodeModel == nodeModel);
        }
    }

    /// <summary>
    /// Kết quả sau khi chạy pipeline.
    /// </summary>
    public class PipelineResult
    {
        public bool Success { get; set; }
        public int TotalNodes { get; set; }
        public int ProcessedNodes { get; set; }
        public long ElapsedMs { get; set; }
        public List<string> Errors { get; } = new();

        public string GetSummary()
        {
            var sb = new StringBuilder();

            if (Success)
            {
                sb.AppendLine("✅ Pipeline chạy thành công!");
            }
            else
            {
                sb.AppendLine("⚠️ Pipeline hoàn tất với lỗi:");
                foreach (var error in Errors)
                    sb.AppendLine($"  • {error}");
            }

            sb.AppendLine();
            sb.AppendLine($"📊 Đã xử lý: {ProcessedNodes}/{TotalNodes} nodes");
            sb.AppendLine($"⏱️ Thời gian: {ElapsedMs} ms");

            return sb.ToString();
        }
    }
}
