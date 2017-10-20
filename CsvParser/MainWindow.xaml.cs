using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows;
using Microsoft.Win32;

namespace CsvParser
{
	/// <summary>
	/// Interaction logic for MainWindow.xaml
	/// </summary>
	public partial class MainWindow : Window
	{
		public MainWindow()
		{
			InitializeComponent();
		}

		private string FileName;

		private void StartBtn_Click(object sender, RoutedEventArgs e)
		{
			var data = new List<AccountDetalization>();
			if (string.IsNullOrEmpty(FileName) || !File.Exists(FileName)) return;

			var csvParser = new CsvParse();

			var delimiter = ';';
			var qualifier = '\r';
			var content = File.ReadAllText(FileName);
			var encoding = csvParser.GetEncoding(FileName);
			var utf8 = Encoding.UTF8;
			

			var encodedBytes = encoding.GetBytes(content);
			var convertedBytes = Encoding.Convert(encoding, utf8, encodedBytes);
			if (!Equals(encoding, utf8))
				content = utf8.GetString(convertedBytes);


			var parser = csvParser.Parse(content, delimiter, qualifier).ToList();


			//parser.TextFieldType = FieldType.Delimited;
			//parser.SetDelimiters(";");
			var first = true;
			//while (!parser.EndOfData)
			foreach (var strings in parser)
			{
				if (strings == null) continue;
				if (first)
				{
					first = false;
					continue;
				}
				var fields = strings.ToList();
				//Processing row
				//string[] fields = parser.ReadFields();

				data.Add(new AccountDetalization
				{
					ContractNumber = fields[0] != null ? Convert.ToInt32(fields[0]) : 0,
					AccountGroup = fields[1] != null ? Convert.ToInt32(fields[0]) : 0,
					AccountNumber = fields[2] != null ? fields[0] : "",
					Date = fields[3] ?? "",
					Time = fields[4] ?? "",
					Duration = fields[5] ?? "",
					RoundDuration = fields[6] != null ? Convert.ToDouble(fields[6]) : 0,
					Size = fields[7] != null ? Convert.ToDouble(fields[7]) : 0,
					Initiator = fields[8] ?? "",
					Acceptor = fields[9] ?? "",
					ActionDescription = fields[10] ?? "",
					ServiceDescription = fields[11] ?? "",
					ServiceType = fields[12] ?? "",
					BaseStationNumber = fields[13] ?? "",
					ValumeMb = fields[14] != null ? Convert.ToDouble(fields[14]) : 0,
					ProviderDescription = fields[15] ?? ""
				});
				/*foreach (string field in fields)
				{
					//TODO: Process field
				}*/
			}
			MainGrid.ItemsSource = OnlyPaidCheck.IsChecked == true ? data.Where(r => r.Size > 0) : data;


		}

		private void OpenFileBtn_Click(object sender, RoutedEventArgs e)
		{
			OpenFileDialog openFileDialog = new OpenFileDialog();
			if (openFileDialog.ShowDialog() == true)
			{
				FileName = openFileDialog.FileName; // File.ReadAllText(openFileDialog.FileName);
				FileNameLbl.Content = FileName;
			}
		}
	}


}
