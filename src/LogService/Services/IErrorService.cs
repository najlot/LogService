namespace LogService.Services;

public interface IErrorService
{
	void ShowError(string title, string message);
	void HideError();
}