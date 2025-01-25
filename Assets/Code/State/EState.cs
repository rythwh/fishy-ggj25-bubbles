namespace Fishy.NState {
	public enum EState {
		Boot, // -> MainMenu
		MainMenu, // -> LoadToGame
		LoadToGame, // -> Game
		Game, // -> PauseMenu
		PauseMenu, // -> Game
	}
}