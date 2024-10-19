namespace LogService.Contracts.Commands
{
	public class UpdateUserSettings
	{
		public int LogRetentionDays { get; set; }

		private UpdateUserSettings() { }

		public UpdateUserSettings(int logRetentionDays)
        {
			LogRetentionDays = logRetentionDays;
		}
    }
}
