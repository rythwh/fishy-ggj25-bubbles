using System;
using System.Collections.Generic;
using RyUI;
using Fishy;
using Fishy.NState;
using UnityEngine;

public class GameManager : MonoBehaviour {
	public static SharedReferences SharedReferences { get; private set; }

	private static readonly Dictionary<Type, IManager> managersMap = new();
	private static readonly List<IManager> managers = new();

	public void Awake() {

		SharedReferences = GetComponent<SharedReferences>();

		// Awakes
		CreateManagers();

		foreach (IManager manager in managers) {
			manager.OnCreate();
		}
	}

	private void CreateManagers() {
		Create<UIManager>();
		Create<StateManager>();
	}

	public void Start() {
		foreach (IManager manager in managers) {
			manager.OnGameSetupComplete();
		}
	}

	public void Update() {
		foreach (IManager manager in managers) {
			manager.OnUpdate();
		}
	}

	private static void Create<TManager>() where TManager : class, IManager, new() {
		TManager manager = new();
		managersMap.Add(typeof(TManager), manager);
		managers.Add(manager);
	}

	public static TManager Get<TManager>() where TManager : class, IManager {
		return managersMap[typeof(TManager)] as TManager;
	}
}