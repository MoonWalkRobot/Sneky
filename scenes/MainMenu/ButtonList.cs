using Godot;
using System.Collections.Generic;

public class ButtonList : VBoxContainer
{
	private List<Button> _buttonList = new List<Button>();
	private int _idx = 0;

	public override void _Ready()
	{
		_buttonList.Add(GetNode<Button>("Play"));
		_buttonList.Add(GetNode<Button>("Settings"));
		_buttonList.Add(GetNode<Button>("Quit"));
		_buttonList[_idx].GrabFocus();
	}

	public void NextFocusGrab()
	{
		_idx += 1;
		_buttonList[_idx % _buttonList.Count].GrabFocus();
	}
}
