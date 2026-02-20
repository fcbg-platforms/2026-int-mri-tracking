namespace MriManagement
{
	using GameLibrary.SOWorkflowCommon.RuntimeAnchors;
	using GameLibrary.Easings;
	using UnityAtoms.BaseAtoms;
	using UnityEngine;
	using TMPro;

	/// <summary>
	///  Specific action simulate the movement of the MRI table to enter it.
	///  You can use the motion tracking to have a real time movement or using a simulation based on a speed.
	/// </summary>
	public class MriEntererUI : MonoBehaviour
	{
		[Header("Settings")]
		[SerializeField] private string _moveInOutMriText = "Move MRI";
		[SerializeField] private string _pauseMriText = "Pause MRI";

		//The 2 conditions are in fact : MRI movement = Enter or Leave or MRI no movement = Pause MRI


		[Header("UI References")]
		// Note that I first thought of using a VariableSO pour this. However, the process is not instantaneous. Thus, it makes more sense to have a void event, and if needed another var that contains the current state.
		[SerializeField] private TextMeshProUGUI _enterMriUI;

		public void UpdateUI(bool isNotMoving)
		{
			// Update UI
			_enterMriUI.text = isNotMoving ? _pauseMriText : _moveInOutMriText;
		}
	}
}
