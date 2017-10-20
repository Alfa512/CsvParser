
namespace CsvParser
{
	public class AccountDetalization
	{
		public int ContractNumber { get; set; }
		public int AccountGroup { get; set; }
		public string AccountNumber { get; set; }
		public string Date { get; set; }
		public string Time { get; set; }
		public string Duration { get; set; }
		public double RoundDuration { get; set; }
		public double Size { get; set; }
		public string Initiator { get; set; }
		public string Acceptor { get; set; }
		public string ActionDescription { get; set; }
		public string ServiceDescription { get; set; }
		public string ServiceType { get; set; }
		public string BaseStationNumber { get; set; }
		public double ValumeMb { get; set; }
		public string ProviderDescription { get; set; }
	}
}