using LogService.Client.MVVM.ViewModel;
using System.Threading.Tasks;

namespace LogService.Client.MVVM.Services
{
	public static class NavigationServiceExtensions
	{
		public static async Task<T> RequestInputAsync<T>(this INavigationService service, AbstractPopupViewModel<T> requestViewModel)
		{
			var taskCompletionSource = new TaskCompletionSource<T>();
			requestViewModel.SetResult = taskCompletionSource.SetResult;
			await service.NavigateForward(requestViewModel);
			var result = await taskCompletionSource.Task;
			await service.NavigateBack();
			return result;
		}
	}
}
