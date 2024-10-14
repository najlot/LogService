using LogService.Client.MVVM.ViewModel;
using System.Threading.Tasks;

namespace LogService.Client.MVVM.Services
{
	public interface INavigationService
	{
		Task NavigateBack();

		Task NavigateForward(AbstractViewModel newViewModel);
	}
}