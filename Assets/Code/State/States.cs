using System;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using Fishy.NUI;
using Prefabs.UI.UIMainMenu;

namespace Fishy.NState
{
	public partial class StateManager
	{
		public Dictionary<EState, State> States => states;

		private static readonly Dictionary<EState, State> states = new() {
			{
				EState.Boot, new State(
					EState.Boot,
					new List<EState> { EState.MainMenu },
					null
				)
			}, {
				EState.MainMenu, new State(
					EState.MainMenu,
					new List<EState> { EState.LoadToGame },
					new List<Func<UniTask>> {
						async () => await GameManager.Get<UIManager>().OpenViewAsync<UIMainMenu>()
					}
				)
			}, {
				EState.LoadToGame, new State(
					EState.LoadToGame,
					new List<EState> { EState.Game },
					new List<Func<UniTask>> {
						// async () => await GameManager.Get<UIManager>().OpenViewAsync<UILoadingScreen>()
					}
				)
			}, {
				EState.Game, new State(
					EState.Game,
					new List<EState> { EState.PauseMenu },
					new List<Func<UniTask>> {
						// async () => await GameManager.Get<UIManager>().OpenViewAsync<UISimulation>()
					}
				)
			}, {
				EState.PauseMenu, new State(
					EState.PauseMenu,
					new List<EState> { EState.Game },
					new List<Func<UniTask>> {
						// async () => await GameManager.Get<UIManager>().OpenViewAsync<UIPauseMenu>()
					}
				)
			}
		};
	}
}