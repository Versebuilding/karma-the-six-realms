using UnityEngine;
using UnityEngine.UIElements;
using System.Collections;
using System.Collections.Generic;

public class UI : MonoBehaviour
{
	private UIDocument ui;
	private VisualElement root;

	public System.Action<string> uiHandlerCallback;

	void Start()
	{
		this.ui = GetComponent<UIDocument>();
		this.root = this.ui.rootVisualElement;
	}

	public virtual void End()
	{
		this.root.style.display = DisplayStyle.None;
		uiHandlerCallback?.Invoke("");
	}
}
