using System.IO;
using Microsoft.Win32;
using BatteryLogix.Services;

namespace BatteryLogix.ViewModels;

public sealed class MainViewModel : ObservableObject
{
    private string _folderPath = "请选择文件夹";
    private string _status = "选择包含 LOG 文件的文件夹，程序将递归扫描并生成同名 Excel 文档。";
    private string _footerStatus = "就绪 — 可选择文件夹开始转换。";
    private string _progressText = "0 / 0 个";
    private double _progress;
    private int _logCount;
    private int _recordCount;
    private int _failedCount;
    private bool _isBusy;

    public MainViewModel()
    {
        SelectFolderCommand = new RelayCommand(SelectFolder, () => !IsBusy);
        ConvertCommand = new RelayCommand(() => _ = ConvertAsync(), () => !IsBusy && Directory.Exists(FolderPath));
    }

    public string FolderPath { get => _folderPath; private set { if (SetProperty(ref _folderPath, value)) ConvertCommand.RaiseCanExecuteChanged(); } }
    public string Status { get => _status; private set => SetProperty(ref _status, value); }
    public string FooterStatus { get => _footerStatus; private set => SetProperty(ref _footerStatus, value); }
    public string ProgressText { get => _progressText; private set => SetProperty(ref _progressText, value); }
    public double Progress { get => _progress; private set => SetProperty(ref _progress, value); }
    public int LogCount { get => _logCount; private set => SetProperty(ref _logCount, value); }
    public int RecordCount { get => _recordCount; private set => SetProperty(ref _recordCount, value); }
    public int FailedCount { get => _failedCount; private set => SetProperty(ref _failedCount, value); }
    public bool IsBusy { get => _isBusy; private set { if (SetProperty(ref _isBusy, value)) { SelectFolderCommand.RaiseCanExecuteChanged(); ConvertCommand.RaiseCanExecuteChanged(); } } }
    public RelayCommand SelectFolderCommand { get; }
    public RelayCommand ConvertCommand { get; }

    private void SelectFolder()
    {
        var dialog = new OpenFolderDialog { Title = "选择 LOG 文件所在文件夹", Multiselect = false };
        if (dialog.ShowDialog() != true) return;
        FolderPath = dialog.FolderName;
        LogCount = CountLogs(FolderPath);
        RecordCount = FailedCount = 0;
        Progress = 0;
        ProgressText = $"0 / {LogCount} 个";
        Status = LogCount == 0 ? "当前文件夹中没有找到 LOG 文件。" : $"扫描完成，共发现 {LogCount} 个 LOG 文件。";
        FooterStatus = "扫描完成 — 点击“开始转换”。";
    }

    private async Task ConvertAsync()
    {
        IsBusy = true; Progress = 0; RecordCount = FailedCount = 0;
        FooterStatus = "正在转换，请稍候…";
        try
        {
            var progress = new Progress<ProgressInfo>(p =>
            {
                Progress = p.Total == 0 ? 100 : p.Done * 100d / p.Total;
                ProgressText = $"{p.Done} / {p.Total} 个";
                RecordCount = p.Records; FailedCount = p.Failed;
                Status = $"正在处理：{p.Name}";
            });
            var result = await Task.Run(() => ConvertFolder(FolderPath, progress));
            Progress = result.Total == 0 ? 0 : 100;
            ProgressText = $"{result.Total} / {result.Total} 个";
            Status = result.Total == 0 ? "未找到 LOG 文件。" : $"转换完成：成功 {result.Success} 个，失败 {result.Failed} 个，共提取 {result.Records} 条数据。";
            FooterStatus = result.Failed == 0 ? "转换完成 — Excel 已保存到各 LOG 文件所在目录。" : "转换完成 — 部分文件转换失败，请确认文件未被占用。";
        }
        catch { Status = "转换未完成，请确认文件夹可以访问。"; FooterStatus = "转换异常。"; }
        finally { IsBusy = false; }
    }

    private static ConvertResult ConvertFolder(string folder, IProgress<ProgressInfo> progress)
    {
        var files = Directory.EnumerateFiles(folder, "*.log", SearchOption.AllDirectories).ToArray();
        var success = 0; var failed = 0; var records = 0;
        for (var i = 0; i < files.Length; i++)
        {
            try { records += ExcelWriter.Write(Path.ChangeExtension(files[i], ".xlsx"), LogParser.Parse(files[i])); success++; }
            catch { failed++; }
            progress.Report(new(i + 1, files.Length, Path.GetFileName(files[i]), records, failed));
        }
        return new(files.Length, success, failed, records);
    }

    private static int CountLogs(string folder)
    {
        try { return Directory.EnumerateFiles(folder, "*.log", SearchOption.AllDirectories).Count(); }
        catch { return 0; }
    }

    private sealed record ProgressInfo(int Done, int Total, string Name, int Records, int Failed);
    private sealed record ConvertResult(int Total, int Success, int Failed, int Records);
}
