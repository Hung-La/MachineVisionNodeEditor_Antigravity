using MachineVisionNodeEditor.Extensions;
using MachineVisionNodeEditor.Models.NodeModels;
using MachineVisionNodeEditor.ViewModels.NodeViewModels;
using OpenCvSharp;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;

namespace MachineVisionNodeEditor.Services
{
    public class VisionPipelineExecutor
    {
        private readonly IEnumerable<NodeControl_NodeViewModel> _nodes;
        private readonly IEnumerable<Node_ConnectionViewModel> _connections;

        public VisionPipelineExecutor(IEnumerable<NodeControl_NodeViewModel> nodes, IEnumerable<Node_ConnectionViewModel> connections)
        {
            _nodes = nodes;
            _connections = connections;
        }

        public PipelineResult Execute()
        {
            var result = new PipelineResult();
            var stopwatch = Stopwatch.StartNew();

            foreach (var node in _nodes)
            {
                node.NodeModel.HasError = false;
                node.NodeModel.ExecutionState = NodeExecutionState.None;
            }

            try
            {
                var sortedNodes = TopologicalSort();
                result.TotalNodes = sortedNodes.Count;

                foreach (var nodeVm in sortedNodes)
                {
                    try
                   {
                        ExecuteNode(nodeVm);
                        nodeVm.NodeModel.ExecutionState = NodeExecutionState.Success;
                        nodeVm.NodeModel.HasError = false;
                        result.ProcessedNodes++;
                    }
                    catch (Exception ex)
                    {
                        nodeVm.NodeModel.ExecutionState = NodeExecutionState.Failed;
                        nodeVm.NodeModel.HasError = true;
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

        private void ExecuteNode(NodeControl_NodeViewModel nodeVm)
        {
            if (nodeVm is ImageImport_NodeViewModel impVM)
            {
                if (string.IsNullOrWhiteSpace(impVM.NodePropertyModel.FilePath))
                {
                    throw new InvalidOperationException("Đường dẫn ảnh đang trống. Vui lòng nhấp đúp vào node và chọn đường dẫn ảnh hợp lệ.");
                }
                if (!System.IO.File.Exists(impVM.NodePropertyModel.FilePath))
                {
                    throw new System.IO.FileNotFoundException($"Không tìm thấy tệp ảnh tại đường dẫn: {impVM.NodePropertyModel.FilePath}");
                }
                
                impVM.OperationModel.Execute(impVM.NodePropertyModel);
                
                if (impVM.NodePropertyModel.Context.OutputImage == null || impVM.NodePropertyModel.Context.OutputImage.Empty())
                {
                    throw new InvalidOperationException("Không thể tải ảnh. Định dạng ảnh không hợp lệ hoặc tệp bị hỏng.");
                }
                impVM.NodePropertyModel.Context.OutputImages = new List<Mat> { impVM.NodePropertyModel.Context.OutputImage };
            }
            else
            {
                var incomingConnections = _connections.Where(c => c.ConnectionModel.ToPort.Owner == nodeVm.NodeModel).ToList();
                var inputImages = new List<Mat>();

                foreach (var conn in incomingConnections)
                {
                    var sourceNodeModel = conn.ConnectionModel.FromPort.Owner;
                    var sourceNodeVM = _nodes.FirstOrDefault(n => n.NodeModel == sourceNodeModel);
                    if (sourceNodeVM != null)
                    {
                        var srcOutputs = sourceNodeVM.NodePropertyModel.Context.OutputImages;
                        if (srcOutputs != null && srcOutputs.Count > 0)
                        {
                            foreach (var img in srcOutputs)
                            {
                                if (img != null && !img.IsDisposed && !img.Empty())
                                {
                                    inputImages.Add(img);
                                }
                            }
                        }
                        else if (sourceNodeVM.NodePropertyModel.Context.OutputImage != null &&
                                 !sourceNodeVM.NodePropertyModel.Context.OutputImage.IsDisposed &&
                                 !sourceNodeVM.NodePropertyModel.Context.OutputImage.Empty())
                        {
                            inputImages.Add(sourceNodeVM.NodePropertyModel.Context.OutputImage);
                        }
                    }
                }

                if (inputImages.Count == 0)
                {
                    throw new InvalidOperationException("Ảnh đầu vào trống hoặc không hợp lệ. Vui lòng kết nối node này với một node hợp lệ khác.");
                }

                nodeVm.NodePropertyModel.Context.InputImages = inputImages;
                nodeVm.NodePropertyModel.Context.InputImage = inputImages[0];

                var outputImages = new List<Mat>();

                foreach (var inputImg in inputImages)
                {
                    nodeVm.NodePropertyModel.Context.InputImage = inputImg;
                    ExecuteSingleNodeOperation(nodeVm);
                    if (nodeVm.NodePropertyModel.Context.OutputImage != null && !nodeVm.NodePropertyModel.Context.OutputImage.Empty())
                    {
                        outputImages.Add(nodeVm.NodePropertyModel.Context.OutputImage);
                    }
                }

                nodeVm.NodePropertyModel.Context.OutputImages = outputImages;
                if (outputImages.Count > 0)
                {
                    nodeVm.NodePropertyModel.Context.OutputImage = outputImages[0];
                }
            }

            if (nodeVm.NodePropertyModel.Context.OutputImage != null)
            {
                nodeVm.NodePropertyModel.Width = nodeVm.NodePropertyModel.Context.OutputImage.Width;
                nodeVm.NodePropertyModel.Height = nodeVm.NodePropertyModel.Context.OutputImage.Height;
            }
        }

        private void ExecuteSingleNodeOperation(NodeControl_NodeViewModel nodeVm)
        {
            if (nodeVm is ConvertColor_NodeViewModel ccVM)
                ccVM.OperationModel.Execute(ccVM.NodePropertyModel);
            else if (nodeVm is Threshold_NodeViewModel threshVM)
                threshVM.OperationModel.Execute(threshVM.NodePropertyModel);
            else if (nodeVm is GaussianBlur_NodeViewModel gbVM)
                gbVM.OperationModel.Execute(gbVM.NodePropertyModel);
            else if (nodeVm is MedianBlur_NodeViewModel mbVM)
                mbVM.OperationModel.Execute(mbVM.NodePropertyModel);
            else if (nodeVm is BilateralFilter_NodeViewModel bfVM)
                bfVM.OperationModel.Execute(bfVM.NodePropertyModel);
            else if (nodeVm is Canny_NodeViewModel cannyVM)
                cannyVM.OperationModel.Execute(cannyVM.NodePropertyModel);
            else if (nodeVm is Erode_NodeViewModel erodeVM)
                erodeVM.OperationModel.Execute(erodeVM.NodePropertyModel);
            else if (nodeVm is Dilate_NodeViewModel dilateVM)
                dilateVM.OperationModel.Execute(dilateVM.NodePropertyModel);
            else if (nodeVm is MorphologyEx_NodeViewModel morphVM)
                morphVM.OperationModel.Execute(morphVM.NodePropertyModel);
        }

        private List<NodeControl_NodeViewModel> TopologicalSort()
        {
            var inDegree = new Dictionary<NodeControl_NodeViewModel, int>();
            var adjacency = new Dictionary<NodeControl_NodeViewModel, List<NodeControl_NodeViewModel>>();

            foreach (var node in _nodes)
            {
                inDegree[node] = 0;
                adjacency[node] = new List<NodeControl_NodeViewModel>();
            }

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

            if (sorted.Count != _nodes.Count())
            {
                throw new InvalidOperationException("Pipeline chứa vòng lặp (cycle)! Vui lòng kiểm tra lại các kết nối.");
            }

            return sorted;
        }

        private NodeControl_NodeViewModel? FindNodeViewModel(NodeModel nodeModel)
        {
            return _nodes.FirstOrDefault(n => n.NodeModel == nodeModel);
        }
    }

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
