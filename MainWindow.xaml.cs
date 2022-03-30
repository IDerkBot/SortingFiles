using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Forms;
using MessageBox = System.Windows.MessageBox;

namespace SortingFiles
{
	/// <summary>
	/// Логика взаимодействия для MainWindow.xaml
	/// </summary>
	public partial class MainWindow : Window
	{
		private readonly List<string> _files = new List<string>();
		private readonly List<string> _dirs = new List<string>();
		private string _path;
		private readonly Dictionary<string, string> _filesAndDirectories = new Dictionary<string, string>
		{
			#region Blender
			["blend"] = "blender",
			["blend1"] = "blender",
			#endregion
			#region Office
			["doc"] = "doc",
			["docx"] = "doc",
			["xlsx"] = "excel",
			["xls"] = "excel",
			["pptx"] = "pptx",
			["ppt"] = "pptx",
			#endregion
			#region Image
			["jpg"] = "img",
			["png"] = "img",
			["jpeg"] = "img",
			["webp"] = "webp",
			["svg"] = "svg",
			["gif"] = "gif",
			["hdr"] = "hdri",
			["bmp"] = "img",
			["tif"] = "img",
			["ico"] = "ico",
			#endregion
			#region Documents
			["pdf"] = "pdf",
			["epub"] = "epub",
			#endregion
			#region Acrhives
			["rar"] = "rar",
			["zip"] = "rar",
			#endregion
			#region Code
			["css"] = "code",
			["html"] = "code",
			["php"] = "code",
			["cs"] = "code",
			["xaml"] = "code",
			["sass"] = "code",
			["scss"] = "code",
			["js"] = "code",
			["sql"] = "sql",
			["py"] = "python",
			#endregion
			#region Text
			["txt"] = "txt",
			#endregion
			["exe"] = "exe",
			["ttf"] = "font",
			["otf"] = "font",
			["torrent"] = "torrent",
			["mp4"] = "video",
			["mp3"] = "mp3",
			["folder"] = "folder",
			["xmi"] = "xmi",
			["file"] = "file",
			["lss"] = "file",
			["vsix"] = "file",
			["bp1"] = "bp1",
			["inf"] = "inf",
			["jar"] = "jar",
			["psd"] = "psd",
			["vpd"] = "vpd",
			["siq"] = "sigames",
		};
		public MainWindow() => InitializeComponent();
		private void FastFindFilesAndDirs()
		{
			// Move files
			foreach (var file in _files)
					FastFileMove(file);
			// Move dirs
			foreach (var dir in _dirs)
					FastMoveDir(dir);

			Find.Text = "Файлы отсортированы";
		}
		private void FastFileMove(string filePath)
		{
			var fileName = filePath.Split('\\').ToList().Last();
			var fileFormat = fileName.Split('.').ToList().Last().ToLower();
			try
			{
				// Проверить формат файла
				if (_filesAndDirectories.All(x => x.Key != fileFormat)) return;

				// Создать директорию если её не существует
				if (!Directory.Exists(_filesAndDirectories.Single(x => x.Key == fileFormat).Value))
					Directory.CreateDirectory($@"{_path}\{_filesAndDirectories.Single(x => x.Key == fileFormat).Value}");

				var fileNewPath = $@"{_path}\{_filesAndDirectories.Single(x => x.Key == fileFormat).Value}\{fileName}";
				if (!File.Exists(fileNewPath))
				{
					File.Move(filePath, fileNewPath);
					ProgressSorting.Value++;
				}
				else
				{
					if (MessageBox.Show(
								$"Файл \"{fileName}\" уже существует в папке {_filesAndDirectories.Single(x => x.Key == fileFormat).Value.ToUpper()}\nПерезаписать?",
								"", MessageBoxButton.YesNo, MessageBoxImage.Question) != MessageBoxResult.Yes) return;
					File.Delete(fileNewPath);
					File.Move(filePath, fileNewPath);
				}
				ProgressSorting.Value++;
			}
			catch (Exception ex)
			{
				MessageBox.Show($"{ex.Message}");
			}
		}
		private void FastMoveDir(string dirPath)
		{
			var dirName = dirPath.Split('\\').ToList().Last();
			if (_filesAndDirectories.Any(x => x.Value == dirName.ToLower())) return;
			if (CheckNoExistFolder(dirName))
			{
				CheckExistMainFolder();
				Directory.Move(dirPath, $@"{_path}\folder\{dirName}");
				ProgressSorting.Value++;
			}
			else
			{
				if (MessageBox.Show(
							$"Папка {dirName} уже существует по пути {_path}\\folder\\\nПереместить все файлы в эту папку?", "",
							MessageBoxButton.YesNo, MessageBoxImage.Question) != MessageBoxResult.Yes) return;
				var files = Directory.GetFiles($"{_path}\\{dirName}");
				foreach (var file in files)
				{
					var fileName = file.Split('\\').ToList().Last();
					var newPath = $"{_path}\\FOLDER\\{dirName}\\";
					if (!File.Exists($"{newPath}{fileName}"))
					{
						File.Move(file, $"{newPath}{fileName}");
						ProgressSorting.Value++;
					}
					else
					{
						if (MessageBox.Show($"Файл \"{fileName}\" уже существует в папке {_path}\\folder\\{dirName}\nПерезаписать?",
									"", MessageBoxButton.YesNo, MessageBoxImage.Question) != MessageBoxResult.Yes) continue;
						File.Delete($"{newPath}{fileName}");
						File.Move(file, $"{newPath}{fileName}");
						ProgressSorting.Value++;
					}
				}
				Directory.Delete($"{_path}\\{dirName}");
			}
			ProgressSorting.Value++;
		}
		private bool CheckNoExistFolder(string directoryName)
		{
			return !Directory.Exists($@"{_path}\folder\{directoryName}");
		}
		private void CheckExistMainFolder()
		{
			if (!Directory.Exists($@"{_path}\folder"))
				Directory.CreateDirectory($@"{_path}\folder");
		}
		private void BtnSort_Click(object sender, RoutedEventArgs e)
		{
			FastFindFilesAndDirs();
		}
		private void OpenFileDialogBtn_Click(object sender, RoutedEventArgs e)
		{
			var dialog = new FolderBrowserDialog();
			dialog.ShowDialog();
			Path.Text = dialog.SelectedPath;
			CheckPath();
		}
		private void CheckPath()
		{
			_path = Path.Text;
			_files.AddRange(Directory.GetFiles(_path).ToList().Where(x => _filesAndDirectories.Keys.Contains(x.Split('\\').ToList().Last().Split('.').ToList().Last())));
			_dirs.AddRange(Directory.GetDirectories(_path).ToList().Where(x => !_filesAndDirectories.Values.Contains(x.Split('\\').ToList().Last())));
			
			Find.Text = (_files.Count + _dirs.Count).ToString();

			BtnSort.IsEnabled = _files.Count + _dirs.Count != 0;

			ProgressSorting.Maximum = _files.Count + _dirs.Count;
			Find.Text = $"Обнаружено {_files.Count} файлов и {_dirs.Count} папок готовых к сортировке";
		}
	}
}