using Cysharp.Threading.Tasks;
using RyUI;

namespace Fishy.NUI
{
	public class UIManager : IManager, IUIManager
	{
		private readonly IUIManager iuiManagerImplementation = new RyUI.UIManager {
			canvas = GameManager.SharedReferences.Canvas
		};

		public UniTask<IUIPresenter> OpenViewAsync<TUIConfig>() where TUIConfig : IUIConfig, new() {
			return iuiManagerImplementation.OpenViewAsync<TUIConfig>();
		}

		public UniTask<IUIPresenter> OpenViewAsync<TUIConfig>(IUIPresenter parent) where TUIConfig : IUIConfig, new() {
			return iuiManagerImplementation.OpenViewAsync<TUIConfig>(parent);
		}

		public UniTask<IUIPresenter> OpenViewAsync<TUIConfig>(IUIPresenter parent, bool showParent) where TUIConfig : IUIConfig, new() {
			return iuiManagerImplementation.OpenViewAsync<TUIConfig>(parent, showParent);
		}

		public UniTask<IUIPresenter> OpenViewAsync<TUIConfig>(IUIPresenter parent, IUIParameters parameters, bool showParent, bool useParentTransform) where TUIConfig : IUIConfig, new() {
			return iuiManagerImplementation.OpenViewAsync<TUIConfig>(parent, parameters, showParent, useParentTransform);
		}

		public UniTask<IUIPresenter> SwitchViewsAsync<TUIConfigToClose, TUIConfigToOpen>(IUIParameters parameters, bool showParent = true, bool useParentTransform = false) where TUIConfigToClose : IUIConfig where TUIConfigToOpen : IUIConfig, new() {
			return iuiManagerImplementation.SwitchViewsAsync<TUIConfigToClose, TUIConfigToOpen>(parameters, showParent, useParentTransform);
		}

		public UniTask<IUIPresenter> SwitchViewsOrOpenAsync<TUIConfigToClose, TUIConfigToOpen>(IUIPresenter parent = null, IUIParameters parameters = null, bool showParent = true, bool useParentTransform = false) where TUIConfigToClose : IUIConfig where TUIConfigToOpen : IUIConfig, new() {
			return iuiManagerImplementation.SwitchViewsOrOpenAsync<TUIConfigToClose, TUIConfigToOpen>(parent, parameters, showParent, useParentTransform);
		}

		public UniTask<IUIPresenter> ReopenView<TUIConfigToReopen>(IUIPresenter parent = null, IUIParameters parameters = null, bool showParent = true) where TUIConfigToReopen : IUIConfig, new() {
			return iuiManagerImplementation.ReopenView<TUIConfigToReopen>(parent, parameters, showParent);
		}

		public void CloseView<TP>(TP presenter) where TP : IUIPresenter {
			iuiManagerImplementation.CloseView(presenter);
		}

		public void CloseView<TC>() where TC : IUIConfig {
			iuiManagerImplementation.CloseView<TC>();
		}

		public void GoBack<TP>(TP presenter) where TP : IUIPresenter {
			iuiManagerImplementation.GoBack(presenter);
		}

		public void ToggleAllViews() {
			iuiManagerImplementation.ToggleAllViews();
		}

		public void CloseAllViews() {
			iuiManagerImplementation.CloseAllViews();
		}
	}
}