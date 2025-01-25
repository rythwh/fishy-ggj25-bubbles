using System.Diagnostics.CodeAnalysis;
using Fishy.NState;
using RyUI;
using UnityEngine;
using UnityEngine.UI;

namespace Prefabs.UI.UIMainMenu
{
	public class UIMainMenuView : UIView
	{
		[SerializeField] private Button startButton;

		public override void OnOpen() {
			startButton.onClick.AddListener(OnStartButtonClicked);
		}

		[SuppressMessage("ReSharper", "AsyncVoidMethod")]
		private async void OnStartButtonClicked() {
			await GameManager.Get<StateManager>().TransitionToState(EState.LoadToGame);
			await GameManager.Get<StateManager>().TransitionToState(EState.Game);
		}

		public override void OnClose() {

		}
	}
}