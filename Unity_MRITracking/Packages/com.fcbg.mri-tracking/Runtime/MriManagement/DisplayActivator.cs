using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DisplayActivator : MonoBehaviour
{
	[Tooltip("The display index number that is used on the camera. First is 1.")]
	[SerializeField] private int _displayIndex = 0;

	void Awake()
	{
		if (_displayIndex != 0 && Display.displays.Length >= _displayIndex)
		{
			Display.displays[_displayIndex - 1].Activate();
		}
	}
}
