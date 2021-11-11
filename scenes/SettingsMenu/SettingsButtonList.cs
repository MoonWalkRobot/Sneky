using Godot;
using System.Collections.Generic;

public class SettingsButtonList : VBoxContainer
{
    private List<Button> _buttonList = new List<Button>();
	private int _idx = 0;

	public override void _Ready()
	{
		_buttonList.Add(GetNode<Button>("ThresholdContainer/Threshold"));
		_buttonList.Add(GetNode<Button>("InvertControlContainer/InvertControl"));
		_buttonList.Add(GetNode<Button>("Back"));
		_buttonList[_idx].GrabFocus();
	}

	public void NextFocusGrab()
	{
		_idx += 1;
		_buttonList[_idx % _buttonList.Count].GrabFocus();
	}
}
