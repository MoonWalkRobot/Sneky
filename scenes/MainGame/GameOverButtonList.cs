using Godot;
using System.Collections.Generic;

public class GameOverButtonList : VBoxContainer
{
    private List<Button> _buttonList = new List<Button>();
	private int _idx = 0;

	public override void _Ready()
	{
		_buttonList.Add(GetNode<Button>("RetryButton"));
		_buttonList.Add(GetNode<Button>("MainMenuButton"));
		_buttonList[_idx].GrabFocus();
	}

	public void NextFocusGrab()
	{
		_idx += 1;
		_buttonList[_idx % _buttonList.Count].GrabFocus();
	}
}
